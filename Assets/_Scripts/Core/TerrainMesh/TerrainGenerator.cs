using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator
{
	public readonly TerrainMesh MainMesh = new TerrainMesh();
	public readonly TerrainMesh AlphaTest = new TerrainMesh();

	public void GenerateChunkMesh(BlockTerrain.Chunk chunk, out MeshData mesh)
	{
		bool[] isTrans = BlocksData.IsTransparent;
		Block[] blocks = BlocksData.Blocks;

		CellFace[] mask;
		int u, v, n, w, h, j, i, l, k;

		int[] off;
		int[] x;
		int[] dim = new int[] { 16, 128, 16 };

		for (int d = 0; d < 3; d++)
		{
			off = new int[] { 0, 0, 0 };
			x = new int[] { 0, 0, 0 };
			u = (d + 1) % 3;
			v = (d + 2) % 3;

			off[d] = 1;

			//face = d;

			mask = new CellFace[dim[u] * dim[v]];
			for (x[d] = -1; x[d] < dim[d];)
			{
				//Debug.LogFormat("x[d]: {0}", x[d]);
				for (n = 0; n < mask.Length; n++)
				{
					mask[n].TextureSlot = -1;
				}

				n = 0;
				for (x[v] = 0; x[v] < dim[v]; x[v]++)
				{
					for (x[u] = 0; x[u] < dim[u]; x[u]++)
					{
						int va = chunk.GetCellValue(x[0], x[1], x[2]);
						int vb = chunk.GetCellValue(x[0] + off[0], x[1] + off[1], x[2] + off[2]);
						int ca = BlockTerrain.GetContent(va);
						int cb = BlockTerrain.GetContent(vb);
						//Debug.LogFormat("{0} and {1}: {2}, {3}", new Point3(x[0], x[1], x[2]), new Point3(x[0] + off[0], x[1] + off[1], x[2] + off[2]), a.Name, b.Name);
						if (isTrans[ca])
						{
							if (!isTrans[cb] && (((x[0] + off[0])) & 16) == 0)
							{
								mask[n].IsOpposite = true;
								blocks[cb].GenerateTerrain(x[0] + off[0], x[1] + off[1], x[2] + off[2], vb, CellFace.opposite[d], chunk, ref mask[n], this);
							}
						}
						else
						{
							if (isTrans[cb])
							{
								mask[n].IsOpposite = false;
								blocks[ca].GenerateTerrain(x[0], x[1], x[2], va, d, chunk, ref mask[n], this);
							}
						}
						n++;
					}
				}

				++x[d];
				n = 0;
				for (j = 0; j < dim[v]; j++)
				{
					for (i = 0; i < dim[u];)
					{
						if (mask[n].TextureSlot != -1)
						{
							for (w = 1; i + w < dim[u] && mask[w + n] == mask[n]; w++)
							{
							}
							for (h = 1; h + j < dim[v]; h++)
							{
								for (k = 0; k < w; k++)
								{
									if (mask[n + k + h * dim[u]] != mask[n])
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

							int textureSlot = mask[n].TextureSlot;
							if (!mask[n].IsOpposite)
							{
								MainMesh.Quad(
                                    new Vector3(x[0], x[1], x[2]),
                                    new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
                                    new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
                                    new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]),
                                    textureSlot,
									mask[n].Color
                                );
							}
							else
							{
								MainMesh.Quad(
									new Vector3(x[0], x[1], x[2]),
                                    new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]),
                                    new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
                                    new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
                                    textureSlot,
                                    mask[n].Color
                                );
							}

							for (l = 0; l < h; l++)
							{
								for (k = 0; k < w; k++)
								{
									mask[n + k + l * dim[u]].TextureSlot = -1;
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

		MainMesh.PushToMesh(out mesh);
	}

	public void GenerateFurnitureMesh(FurnitureManager.Furniture furniture, out MeshData mesh)
	{
		Block[] blocks = BlocksData.Blocks;

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
						int ia = BlockTerrain.GetContent(va);
						int ib = BlockTerrain.GetContent(vb);
						//Debug.LogFormat("{0} and {1}: {2}, {3}", new Point3(x[0], x[1], x[2]), new Point3(x[0] + off[0], x[1] + off[1], x[2] + off[2]), a.Name, b.Name);
						if (ia == 0 && ib != 0)
						{
							Block b = blocks[ib];
							mask[n] = b.TextureSlot;
							mask[n] |= BlocksData.GetBlockColorInt(b, vb) << 8;
							mask[n] |= 4096;
						}
						else if (ia != 0 && ib == 0)
						{
							Block a = blocks[ia];
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
								MainMesh.Quad(
									new Vector3(x[0], x[1], x[2]),
									new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
									new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
									new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]),
									textureSlot,
									BlocksData.DEFAULT_COLORS[(mask[n] >> 8) & 15]
								);
							}
							else
							{
								MainMesh.Quad(
									new Vector3(x[0], x[1], x[2]),
								    new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]),
									new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
									new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
									textureSlot,
									BlocksData.DEFAULT_COLORS[(mask[n] >> 8) & 15]
								);
							}

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

		MainMesh.PushToMesh(out mesh);
		MeshData.Transform(mesh, matrix);
	}
}

public struct CellFace
{
	public const int FRONT = 0;
	public const int TOP = 1;
	public const int RIGHT = 2;
	public const int BACK = 3;
	public const int BOTTOM = 4;
	public const int LEFT = 5;

	public int TextureSlot;
	public bool IsOpposite;
	public Color Color;

	public static int[] opposite = { 3, 4, 5, 0, 1, 2 };

	public override bool Equals(object obj)
	{
		return (obj is CellFace) && Equals((CellFace)obj);
	}

	public bool Equals(CellFace face)
	{
		return face.TextureSlot == TextureSlot && face.IsOpposite == IsOpposite && face.Color == Color;
	}

	public static bool operator ==(CellFace a, CellFace b)
	{
		return Equals(a, b);
	}

	public static bool operator !=(CellFace a, CellFace b)
	{
		return !Equals(a, b);
	}
}