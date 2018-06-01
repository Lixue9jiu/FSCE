using UnityEngine;
using System.Collections.Generic;

public class TerrainMesh
{   
	readonly List<Vector3> vertices = new List<Vector3>();
	readonly List<int> triangles = new List<int>();
	readonly List<Vector2> uvs = new List<Vector2>();
	readonly List<Color> colors = new List<Color>();

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

    public void TwoSidedQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int texSlot, Color color)
    {
        //int count = vertices.Count;
        //vertices.Add(a);
        //vertices.Add(b);
        //vertices.Add(c);
        //vertices.Add(d);

        //Vector2 uvPos = new Vector2((texSlot % 16) / 16f, -((texSlot >> 4) + 1) / 16f);
        //uvs.Add(uvPos);
        //uvs.Add(uvPos + new Vector2(0.0625f, 0));
        //uvs.Add(uvPos + new Vector2(0.0625f, 0.0625f));
        //uvs.Add(uvPos + new Vector2(0, 0.0625f));

        //colors.Add(color);
        //colors.Add(color);
        //colors.Add(color);
        //colors.Add(color);

        NormalQuad(a, b, c, d, texSlot, color);
        NormalQuad(b, a, d, c, texSlot, color);
    }

	public void NormalQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int texSlot, Color color)
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

        Vector2 uvPos = new Vector2((texSlot % 16) / 16f, -((texSlot >> 4) + 1) / 16f);
        uvs.Add(uvPos);
		uvs.Add(uvPos + new Vector2(0.0625f, 0));
		uvs.Add(uvPos + new Vector2(0.0625f, 0.0625f));
		uvs.Add(uvPos + new Vector2(0, 0.0625f));

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

	public void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int texSlot, Color color)
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

		Vector2 uvPos = new Vector2((texSlot % 16) / 16f, -((texSlot >> 4) + 1) / 16f);
        uvs.Add(uvPos);
        uvs.Add(uvPos);
        uvs.Add(uvPos);
        uvs.Add(uvPos);

		colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

	public void FurnitureQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int u, int v, int du, int dv, float blockSize, int texSlot, Color color)
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

        Vector2 uvPos = new Vector2((texSlot % 16) / 16f, -((texSlot >> 4) + 1) / 16f);
		uvs.Add(uvPos + new Vector2(u * blockSize, v * blockSize));
		uvs.Add(uvPos + new Vector2((u + du) * blockSize, v * blockSize));
		uvs.Add(uvPos + new Vector2((u + du) * blockSize, (v + dv) * blockSize));
		uvs.Add(uvPos + new Vector2(u * blockSize, (v + dv) * blockSize));

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

	public void Mesh(int x, int y, int z, MeshData mesh)
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

		uvs.AddRange(mesh.uv);
		colors.AddRange(mesh.colors);
	}

	public void Mesh(int x, int y, int z, MeshData mesh, Color color)
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

        uvs.AddRange(mesh.uv);
		for (int i = 0; i < mesh.vertices.Length; i++)
        {
            colors.Add(color);
        }
    }
}
