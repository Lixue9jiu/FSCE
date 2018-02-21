﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class BlocksData : MonoBehaviour
{
	struct BlockData
	{
		public int Index;
		public string Name;
		public int TextureSlot;

		public BlockData (string src)
		{
			string[] strs = src.Split (';');
			Index = int.Parse (strs [0]);
			Name = strs [1];
			TextureSlot = int.Parse (strs [2]);
		}
	}

	static Block[] blocks;
	static Dictionary<System.Type, int[]> definedBlocks = new Dictionary<System.Type, int[]> () {
		{ typeof(XBlock), new int[] { 19, 20, 24, 25, 28, 174, 204 } },
		{ typeof(FlatBlock), new int[] { 197 } },
		{ typeof(AirBlock), new int[] { 0 } },
		{ typeof(GrassBlock), new int[] { 8 } },
		{ typeof(TreeBlock), new int[] { 9, 10, 11 } },
		{ typeof(TransparentBlock), new int[] { 12, 13, 14 } }
	};

	public static Block GetBlock (int content)
	{
		if (content < blocks.Length) {
			return blocks [content];
		}
		return blocks [0];
	}

	public static readonly Color[] DEFAULT_COLORS = new Color[] {
		new Color (1, 1, 1),
		new Color (0.7109375f, 1, 1),
		new Color (1, 0.7109375f, 1),
		new Color (0.625f, 0.7109375f, 1),
		new Color (1, 0.9375f, 0.625f),
		new Color (0.7109375f, 1, 0.7109375f),
		new Color (1, 0.7109375f, 0.625f),
		new Color (0.7109375f, 0.7109375f, 0.7109375f),
		new Color (0.4375f, 0.4375f, 0.4375f),
		new Color (0.125f, 0.4375f, 0.4375f),
		new Color (0.4375f, 0.125f, 0.4375f),
		new Color (0.1015625f, 0.203125f, 0.5f),
		new Color (0.33984375f, 0.2109375f, 0.125f),
		new Color (0.09375f, 0.453125f, 0.09375f),
		new Color (0.53125f, 0.125f, 0.125f),
		new Color (0.09375f, 0.09375f, 0.09375f)
	};

	public static Color GetBlockColor (Block b, int value)
	{
		if (b is PaintableBlock) {
			return ((PaintableBlock)b).GetColorC (value);
		}
		return Color.white;
	}

	void Awake ()
	{
		ParseBlocksData ("Assets/Resources/out.txt");
	}

	void ParseBlocksData (string path)
	{
		Dictionary<int, BlockData> blockData = new Dictionary<int, BlockData> ();
		using (StreamReader reader = new StreamReader (path)) {
			reader.ReadLine ();
			string line;
			while ((line = reader.ReadLine ()) != null) {
				BlockData data = new BlockData (line);
				blockData [data.Index] = data;
			}
		}

		Dictionary<int, Block> blockClass = new Dictionary<int, Block> ();
		foreach (System.Type t in definedBlocks.Keys) {
			foreach (int index in definedBlocks[t]) {
				blockClass [index] = (Block)System.Activator.CreateInstance (t);
			}
		}

		List<Block> b = new List<Block> ();
		int i = 0;
		int count = blockData.Count;
		while (i < count) {
			if (blockData.ContainsKey (i)) {
				Block block;
				if (blockClass.ContainsKey (i)) {
					block = blockClass [i];
				} else {
					block = new CubeBlock ();
				}
				LoadBlockClass (block, blockData [i]);
				b.Add (block);
				i++;
			} else {
				Block block = new CubeBlock ();
				LoadBlockClass (block, blockData [0]);
				b.Add (block);
			}
		}

		blocks = b.ToArray ();
		for (int k = 0; k < blocks.Length; k++) {
			blocks [k].Initialize (gameObject);
		}
	}

	void LoadBlockClass (Block block, BlockData data)
	{
		block.Index = data.Index;
		block.Name = data.Name;
		block.TextureSlot = data.TextureSlot;
	}

	public static bool IsTransparent (BlockTerrain.Chunk chunk, int x, int y, int z)
	{
		return GetBlock (chunk.GetCellContent (x, y, z)).IsTransparent;
	}
}

public abstract class Block
{
	public const int XminusOne = 1;
	public const int YminusOne = 2;
	public const int ZminusOne = 4;
	public const int XplusOne = 8;
	public const int YplusOne = 16;
	public const int ZplusOne = 32;

	public const int TOP = 0;
	public const int FRONT = 1;
	public const int RIGHT = 2;
	public const int BOTTOM = 3;
	public const int BACK = 4;
	public const int LEFT = 5;

	public int Index;
	public bool IsTransparent;
	public int TextureSlot;

	public string Name;

	public virtual void Initialize (GameObject game)
	{
	}

	public virtual int GetTextureSlot (int value, int face)
	{
		return TextureSlot;
	}

	protected void DrawCubeBlock (int x, int y, int z, int value, int neighborData, Color color, TerrainGenerator g)
	{
		Vector3 v000 = new Vector3 (x, y, z);
		Vector3 v001 = new Vector3 (x, y, z + 1.0f);
		Vector3 v010 = new Vector3 (x, y + 1.0f, z);
		Vector3 v011 = new Vector3 (x, y + 1.0f, z + 1.0f);
		Vector3 v100 = new Vector3 (x + 1.0f, y, z);
		Vector3 v101 = new Vector3 (x + 1.0f, y, z + 1.0f);
		Vector3 v110 = new Vector3 (x + 1.0f, y + 1.0f, z);
		Vector3 v111 = new Vector3 (x + 1.0f, y + 1.0f, z + 1.0f);

		if ((neighborData & XminusOne) == XminusOne) {
			g.MeshFromRect (v001, v011, v010, v000);
			g.GenerateBlockUVs (GetTextureSlot (value, BACK));
			g.GenerateBlockColors (color);
		}
		if ((neighborData & YminusOne) == YminusOne) {
			g.MeshFromRect (v000, v100, v101, v001);
			g.GenerateBlockUVs (GetTextureSlot (value, BOTTOM));
			g.GenerateBlockColors (color);
		}
		if ((neighborData & ZminusOne) == ZminusOne) {
			g.MeshFromRect (v000, v010, v110, v100);
			g.GenerateBlockUVs (GetTextureSlot (value, LEFT));
			g.GenerateBlockColors (color);
		}
		if ((neighborData & XplusOne) == XplusOne) {
			g.MeshFromRect (v100, v110, v111, v101);
			g.GenerateBlockUVs (GetTextureSlot (value, FRONT));
			g.GenerateBlockColors (color);
		}
		if ((neighborData & YplusOne) == YplusOne) {
			g.MeshFromRect (v111, v110, v010, v011);
			g.GenerateBlockUVs (GetTextureSlot (value, TOP));
			g.GenerateBlockColors (color);
		}
		if ((neighborData & ZplusOne) == ZplusOne) {
			g.MeshFromRect (v101, v111, v011, v001);
			g.GenerateBlockUVs (GetTextureSlot (value, RIGHT));
			g.GenerateBlockColors (color);
		}
	}

	protected void DrawMeshBlock (int x, int y, int z, int value, Mesh mesh, Color color, TerrainGenerator g)
	{
		g.MeshFromMesh (x, y, z, mesh);
		g.GenerateTextureForMesh (mesh, TextureSlot, color);
	}

	public abstract void GenerateTerrain (int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g);
}

public abstract class PaintableBlock : Block
{
	int paintTextureSlot;

	public override void Initialize (GameObject game)
	{
		switch (TextureSlot) {
		case 4:
			paintTextureSlot = 23;
			break;
		case 1:
			paintTextureSlot = 24;
			break;
		case 70:
			paintTextureSlot = 39;
			break;
		case 6:
			paintTextureSlot = 40;
			break;
		case 176:
			paintTextureSlot = 64;
			break;
		case 16:
			paintTextureSlot = 69;
			break;
		case 7:
			paintTextureSlot = 51;
			break;
		case 54:
			paintTextureSlot = 50;
			break;
		}
	}

	public abstract int? GetColor (int data);

	public Color GetColorC (int value)
	{
		int? i = GetColor (BlockTerrain.GetData (value));
		if (i.HasValue) {
			return BlocksData.DEFAULT_COLORS [i.Value];
		}
		return Color.white;
	}

	public override int GetTextureSlot (int value, int face)
	{
		if (GetColor (BlockTerrain.GetData (value)).HasValue) {
			return paintTextureSlot;
		}
		return TextureSlot;
	}
}

public class AirBlock : Block
{
	public override void Initialize (GameObject game)
	{
		IsTransparent = true;
	}

	public override void GenerateTerrain (int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
	{
	}
}

public class CubeBlock : Block
{
	public override void GenerateTerrain (int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
	{
		int neighborData = 0;
		neighborData += (BlocksData.IsTransparent (chunk, x - 1, y, z)) ? XminusOne : 0;
		neighborData += (BlocksData.IsTransparent (chunk, x, y - 1, z)) ? YminusOne : 0;
		neighborData += (BlocksData.IsTransparent (chunk, x, y, z - 1)) ? ZminusOne : 0;
		neighborData += (BlocksData.IsTransparent (chunk, x + 1, y, z)) ? XplusOne : 0;
		neighborData += (BlocksData.IsTransparent (chunk, x, y + 1, z)) ? YplusOne : 0;
		neighborData += (BlocksData.IsTransparent (chunk, x, y, z + 1)) ? ZplusOne : 0;

		DrawCubeBlock (x, y, z, value, neighborData, Color.white, g);
	}
}

public class TransparentBlock : Block
{
	public override void Initialize (GameObject game)
	{
		IsTransparent = true;
	}

	public override void GenerateTerrain (int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
	{
		int neighborData = 0;
		neighborData += GetNeightbor (chunk, x - 1, y, z, XminusOne);
		neighborData += GetNeightbor (chunk, x, y - 1, z, YminusOne);
		neighborData += GetNeightbor (chunk, x, y, z - 1, ZminusOne);
		neighborData += GetNeightbor (chunk, x + 1, y, z, XplusOne);
		neighborData += GetNeightbor (chunk, x, y + 1, z, YplusOne);
		neighborData += GetNeightbor (chunk, x, y, z + 1, ZplusOne);

		DrawCubeBlock (x, y, z, value, neighborData, Color.white, g);
	}

	int GetNeightbor (BlockTerrain.Chunk chunk, int x, int y, int z, int mask)
	{
		int content = chunk.GetCellContent (x, y, z);
		if (content != Index && BlocksData.GetBlock (content).IsTransparent) {
			return mask;
		}
		return 0;
	}
}

public class GrassBlock : CubeBlock
{
	public override int GetTextureSlot (int value, int face)
	{
		if (face == TOP)
			return 0;
		return 3;
	}
}

public class TreeBlock : CubeBlock
{
	public override int GetTextureSlot (int value, int face)
	{
		if (face == TOP || face == BOTTOM)
			return 21;
		return TextureSlot;
	}
}

public class PaintableCubeBlock : PaintableBlock
{
	public override int? GetColor (int data)
	{
		if ((data & 1) != 0) {
			return new int? (data >> 1 & 15);
		}
		return null;
	}

	public override void GenerateTerrain (int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
	{
		int neighborData = 0;
		neighborData += (BlocksData.IsTransparent (chunk, x - 1, y, z)) ? XminusOne : 0;
		neighborData += (BlocksData.IsTransparent (chunk, x, y - 1, z)) ? YminusOne : 0;
		neighborData += (BlocksData.IsTransparent (chunk, x, y, z - 1)) ? ZminusOne : 0;
		neighborData += (BlocksData.IsTransparent (chunk, x + 1, y, z)) ? XplusOne : 0;
		neighborData += (BlocksData.IsTransparent (chunk, x, y + 1, z)) ? YplusOne : 0;
		neighborData += (BlocksData.IsTransparent (chunk, x, y, z + 1)) ? ZplusOne : 0;

		Color color = GetColorC (BlockTerrain.GetData (value));

		DrawCubeBlock (x, y, z, value, neighborData, color, g);
	}
}

public class XBlock : Block
{
	public override void Initialize (GameObject game)
	{
		IsTransparent = true;
	}

	public override void GenerateTerrain (int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
	{
		Vector3 v000 = new Vector3 (x, y, z);
		Vector3 v001 = new Vector3 (x, y, z + 1.0f);
		Vector3 v010 = new Vector3 (x, y + 1.0f, z);
		Vector3 v011 = new Vector3 (x, y + 1.0f, z + 1.0f);
		Vector3 v100 = new Vector3 (x + 1.0f, y, z);
		Vector3 v101 = new Vector3 (x + 1.0f, y, z + 1.0f);
		Vector3 v110 = new Vector3 (x + 1.0f, y + 1.0f, z);
		Vector3 v111 = new Vector3 (x + 1.0f, y + 1.0f, z + 1.0f);

		int textureSlot = TextureSlot;
		Color color = Color.white;

		g.MeshFromRect (v101, v111, v010, v000);
		g.GenerateBlockUVs (textureSlot);
		g.GenerateBlockColors (color);
		g.MeshFromRect (v000, v010, v111, v101);
		g.GenerateBlockUVs (textureSlot);
		g.GenerateBlockColors (color);
		g.MeshFromRect (v100, v110, v011, v001);
		g.GenerateBlockUVs (textureSlot);
		g.GenerateBlockColors (color);
		g.MeshFromRect (v001, v011, v110, v100);
		g.GenerateBlockUVs (textureSlot);
		g.GenerateBlockColors (color);
	}
}

public class FlatBlock : Block
{
	public override void Initialize (GameObject game)
	{
		IsTransparent = true;
	}

	public override void GenerateTerrain (int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
	{
		int face = FurnitureManager.GetRotation (BlockTerrain.GetData (value));

		Vector3 v0;
		Vector3 v1;
		Vector3 v2;
		Vector3 v3;
		switch (face) {
		case 3:
			v0 = new Vector3 (x, y, z);
			v1 = new Vector3 (x, y + 1.0f, z);
			v2 = new Vector3 (x, y + 1.0f, z + 1.0f);
			v3 = new Vector3 (x, y, z + 1.0f);
			break;
		case 0:
			v0 = new Vector3 (x, y, z + 1.0f);
			v1 = new Vector3 (x, y + 1.0f, z + 1.0f);
			v2 = new Vector3 (x + 1.0f, y + 1.0f, z + 1.0f);
			v3 = new Vector3 (x + 1.0f, y, z + 1.0f);
			break;
		case 1:
			v0 = new Vector3 (x + 1.0f, y, z + 1.0f);
			v1 = new Vector3 (x + 1.0f, y + 1.0f, z + 1.0f);
			v2 = new Vector3 (x + 1.0f, y + 1.0f, z);
			v3 = new Vector3 (x + 1.0f, y, z);
			break;
		case 2:
			v0 = new Vector3 (x + 1.0f, y, z);
			v1 = new Vector3 (x + 1.0f, y + 1.0f, z);
			v2 = new Vector3 (x, y + 1.0f, z);
			v3 = new Vector3 (x, y, z);
			break;
		default:
			throw new UnityException ("undefined face: " + face);
		}

		Color color = Color.white;

		g.MeshFromRect (v0, v1, v2, v3);
		g.GenerateBlockUVs (TextureSlot);
		g.GenerateBlockColors (color);
		g.MeshFromRect (v3, v2, v1, v0);
		g.GenerateBlockUVs (TextureSlot);
		g.GenerateBlockColors (color);
	}
}

public class StairBlock : PaintableBlock
{
	Mesh[] stairs = new Mesh[24];

	public override void Initialize (GameObject game)
	{
		base.Initialize (game);
		BlockMeshes meshes = game.GetComponent<BlockMeshes> ();

		float y;
		Matrix4x4 m;
		Mesh mesh;
		for (int i = 0; i < 24; i++) {
			y = 0;

			int rotation = FurnitureManager.GetRotation (i);
			y -= rotation * 90;

			m = Matrix4x4.TRS (Vector3.zero, Quaternion.Euler (0, y, 0), Vector3.one);
			switch ((i >> 3) & 3) {
			case 1:
				mesh = meshes.stair0;
				break;
			case 0:
				mesh = meshes.stair1;
				break;
			case 2:
				mesh = meshes.stair2;
				break;
			default:
				throw new UnityException ("unknown stair module: " + ((i >> 3) & 3));
			}

			stairs [i] = BlockMeshes.TranslateMesh (mesh, m, (i & 4) != 0);
		}
	}

	public override int? GetColor (int data)
	{
		if ((data & 32) != 0) {
			return new int? (data >> 6 & 15);
		}
		return null;
	}

	public static int GetVariant (int data)
	{
		return data & 31;
	}

	public override void GenerateTerrain (int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
	{
		Mesh mesh = stairs [GetVariant (BlockTerrain.GetData (value))];
		DrawMeshBlock (x, y, z, value, mesh, GetColorC (value), g);
	}
}

public class SlabBlock : PaintableBlock
{
	Mesh[] slabs = new Mesh[2];

	public override void Initialize (GameObject game)
	{
		base.Initialize (game);
		BlockMeshes meshes = game.GetComponent<BlockMeshes> ();
		slabs [1] = meshes.slab;
		slabs [0] = BlockMeshes.UpsideDownMesh (meshes.slab);
	}

	public override int? GetColor (int data)
	{
		if ((data & 2) != 0) {
			return new int? (data >> 2 & 15);
		}
		return null;
	}

	public static bool GetIsTop (int data)
	{
		return (data & 1) != 0;
	}

	public override void GenerateTerrain (int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
	{
		int i = SlabBlock.GetIsTop (BlockTerrain.GetData (value)) ? 1 : 0;
		Mesh mesh = slabs [i];
		DrawMeshBlock (x, y, z, value, mesh, GetColorC (value), g);
	}
}