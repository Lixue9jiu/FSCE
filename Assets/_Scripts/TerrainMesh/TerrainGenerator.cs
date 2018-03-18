using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    List<Color> colors = new List<Color>();

    public void MeshFromChunk(BlockTerrain.Chunk chunk, out MeshData mesh)
    {
        for (int x = 0; x < chunk.sizeX; x++)
        {
            for (int y = 0; y < chunk.sizeY; y++)
            {
                for (int z = 0; z < chunk.sizeZ; z++)
                {
                    int value = chunk.GetCellValue(x, y, z);
                    int content = BlockTerrain.GetContent(value);
                    Block b = BlocksData.GetBlock(content);
                    if (!b.IsTransparent)
                    {
                        b.GenerateTerrain(x, y, z, value, chunk, this);
                    }
                }
            }
        }

        PushToMesh(out mesh);
    }

    public void MeshFromChunk(BlockTerrain.Chunk chunk, Mesh mesh)
    {
        MeshData data;
        MeshFromChunk(chunk, out data);
        data.ToMesh(mesh);
    }

    public void MeshFromTransparent(BlockTerrain.Chunk chunk, out MeshData mesh)
    {
        for (int x = 0; x < chunk.sizeX; x++)
        {
            for (int y = 0; y < chunk.sizeY; y++)
            {
                for (int z = 0; z < chunk.sizeZ; z++)
                {
                    int value = chunk.GetCellValue(x, y, z);
                    int content = BlockTerrain.GetContent(value);
                    Block b = BlocksData.GetBlock(content);
                    if (b.IsTransparent)
                    {
                        b.GenerateTerrain(x, y, z, value, chunk, this);
                    }
                }
            }
        }

        PushToMesh(out mesh);
    }

    public void MeshFromTransparent(BlockTerrain.Chunk chunk, Mesh mesh)
    {
        MeshData data;
        MeshFromTransparent(chunk, out data);
        data.ToMesh(mesh);
    }

    public void MeshFromFurniture(FurnitureManager.Furniture furniture, out Mesh mesh)
    {
        int res = furniture.Resolution;
        Matrix4x4 matrix = Matrix4x4.Scale(Vector3.one / res);
        float uvBlockSize = 0.0625f / res;

        int[] mask = new int[res * res];
        int u, v, n, w, h, j, i, l, k;

        int[] off;
        int[] x;

        for (int d = 0; d < 3; d++)
        {
            off = new int[] { 0, 0, 0 };
            x = new int[] { 0, 0, 0 };
            u = (d + 1) % 3;
            v = (d + 2) % 3;

            off[d] = 1;

            for (x[d] = -1; x[d] < res;)
            {
                //Debug.LogFormat("x[d]: {0}", x[d]);
                mask = new int[res * res];
                for (n = 0; n < mask.Length; n++)
                {
                    mask[n] = -1;
                }

                n = 0;
                for (x[v] = 0; x[v] < res; x[v]++)
                {
                    for (x[u] = 0; x[u] < res; x[u]++)
                    {
                        int va = furniture.GetCellValue(x[0], x[1], x[2]);
                        int vb = furniture.GetCellValue(x[0] + off[0], x[1] + off[1], x[2] + off[2]);
                        Block a = BlocksData.GetBlock(BlockTerrain.GetContent(va));
                        Block b = BlocksData.GetBlock(BlockTerrain.GetContent(vb));
                        //Debug.LogFormat("{0} and {1}: {2}, {3}", new Point3(x[0], x[1], x[2]), new Point3(x[0] + off[0], x[1] + off[1], x[2] + off[2]), a.Name, b.Name);
                        if (a.Index == 0 && b.Index != 0)
                        {
                            mask[n] = b.TextureSlot;
                            mask[n] |= BlocksData.GetBlockColorInt(b, vb) << 8;
                            mask[n] |= 4096;
                        }
                        else if (a.Index != 0 && b.Index == 0)
                        {
                            mask[n] = a.TextureSlot;
                            mask[n] |= BlocksData.GetBlockColorInt(a, va) << 8;
                        }
                        n++;
                    }
                }

                ++x[d];
                n = 0;
                for (j = 0; j < res; j++)
                {
                    for (i = 0; i < res;)
                    {
                        if (mask[n] != -1)
                        {
                            for (w = 1; i + w < res && mask[w + n] == mask[n]; w++)
                            {
                            }
                            for (h = 1; h + j < res; h++)
                            {
                                for (k = 0; k < w; k++)
                                {
                                    if (mask[n + k + h * res] != mask[n])
                                    {
                                        goto Done;
                                    }
                                }
                            }
                        Done:
                            //Debug.LogFormat("quard: {0}, {1}, {2}, {3}; {4}", j, i, h, w, x[d]);

                            x[u] = i;
                            x[v] = j;
                            int[] du = new int[] { 0, 0, 0 };
                            int[] dv = new int[] { 0, 0, 0 };
                            du[u] = w;
                            dv[v] = h;

                            int textureSlot = mask[n] & 255;
                            if (mask[n] >> 12 == 0)
                            {
                                MeshFromRect(new Vector3(x[0], x[1], x[2]),
                                             new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
                                             new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
                                             new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]));
                                GenerateFurnitureUVs(textureSlot, x[u], x[v], uvBlockSize);
                                GenerateFurnitureUVs(textureSlot, x[u] + w, x[v], uvBlockSize);
                                GenerateFurnitureUVs(textureSlot, x[u] + w, x[v] + h, uvBlockSize);
                                GenerateFurnitureUVs(textureSlot, x[u], x[v] + h, uvBlockSize);
                            }
                            else
                            {
                                MeshFromRect(new Vector3(x[0], x[1], x[2]),
                                             new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]),
                                             new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
                                             new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]));
                                GenerateFurnitureUVs(textureSlot, x[u], x[v], uvBlockSize);
                                GenerateFurnitureUVs(textureSlot, x[u], x[v] + h, uvBlockSize);
                                GenerateFurnitureUVs(textureSlot, x[u] + w, x[v] + h, uvBlockSize);
                                GenerateFurnitureUVs(textureSlot, x[u] + w, x[v], uvBlockSize);
                            }

                            GenerateBlockColors(BlocksData.DEFAULT_COLORS[(mask[n] >> 8) & 15]);

                            for (l = 0; l < h; l++)
                            {
                                for (k = 0; k < w; k++)
                                {
                                    mask[n + k + l * res] = -1;
                                }
                            }
                            i += w;
                            n += w;
                        }
                        else
                        {
                            i++;
                            n++;
                        }
                    }
                }
            }
        }

        //Debug.LogFormat("{0}, {1}, {2}", vertices.Count, colors.Count, uvs.Count);
        for (i = 0; i < vertices.Count; i++)
        {
            vertices[i] = matrix.MultiplyPoint3x4(vertices[i]);
        }
        mesh = new Mesh();
        PushToMesh(mesh);
    }

    //从矩形顶点生成uv
    void GenerateFurnitureUVs(int slotIndex, int x, int y, float blockSize)
    {
        float v = (float)(slotIndex & 15) / 16;
        float u = (float)(15 - (slotIndex >> 4)) / 16;

        uvs.Add(new Vector2(v + x * blockSize, u + y * blockSize));
    }

    public void PushToMesh(Mesh mesh)
    {
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();

        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        colors.Clear();
    }

    public void PushToMesh(out MeshData mesh)
    {
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();

        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        colors.Clear();
    }

    public void MeshFromMesh(int x, int y, int z, MeshData mesh, bool raw = false)
    {
        Vector3 tran = new Vector3(x, y, z);
        int count = vertices.Count;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            vertices.Add(mesh.vertices[i] + tran);
        }
        for (int i = 0; i < mesh.triangles.Length; i++)
        {
            triangles.Add(mesh.triangles[i] + count);
        }
        if (raw)
        {
            uvs.AddRange(mesh.uv);
            if (mesh.colors.Length != 0)
            {
                colors.AddRange(mesh.colors);
            }
            else
            {
                for (int i = 0; i < mesh.vertices.Length; i++)
                {
                    colors.Add(Color.white);
                }
            }
        }
    }

    public void MeshFromRect(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        int count = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);

        triangles.Add(count);
        triangles.Add(count + 1);
        triangles.Add(count + 2);
        triangles.Add(count + 2);
        triangles.Add(count + 3);
        triangles.Add(count);
    }

    public void GenerateBlockUVs(int slotIndex)
    {
        float v = slotIndex % 16;
        float u = 15 - (slotIndex / 16);

        Vector2 v00 = new Vector2(v / 16, u / 16);
        Vector2 v01 = new Vector2(v / 16, u / 16 + 0.0625f);
        Vector2 v10 = new Vector2(v / 16 + 0.0625f, u / 16);
        Vector2 v11 = new Vector2(v / 16 + 0.0625f, u / 16 + 0.0625f);

        uvs.Add(v00);
        uvs.Add(v01);
        uvs.Add(v11);
        uvs.Add(v10);
    }

    public void GenerateBlockColors(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    public void GenerateTextureForMesh(MeshData mesh, int textureSlot, Color color)
    {
        float v = (float)(textureSlot % 16) / 16;
        float u = (float)(15 - (textureSlot / 16)) / 16;

        Vector2 vec = new Vector2(v, u);
        Vector2[] uv = mesh.uv;

        for (int i = 0; i < uv.Length; i++)
        {
            uvs.Add(vec + uv[i]);
            colors.Add(color);
        }
    }

}
