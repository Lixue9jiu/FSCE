using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BlocksData : MonoBehaviour
{
    struct BlockData
    {
        public int Index;
        public string Name;
        public int TextureSlot;

        public BlockData(string src)
        {
            string[] strs = src.Split(';');
            Index = int.Parse(strs[0]);
            Name = strs[1];
            TextureSlot = int.Parse(strs[2]);
        }
    }

    static NullBlock nullBlock = new NullBlock();
    static Block[] blocks;
    static Dictionary<System.Type, int[]> definedBlocks = new Dictionary<System.Type, int[]>() {
        { typeof(XBlock), new int[] { 20, 24, 25, 28, 174, 204 } },
        { typeof(IvyBlock), new int[] { 197 } },
        { typeof(AirBlock), new int[] { 0 } },
        { typeof(GrassBlock), new int[] { 8 } },
        { typeof(WaterBlock), new int[] { 18 } },
        { typeof(XGrassBlock), new int[] { 19 } },
        { typeof(TreeBlock), new int[] { 9, 10, 11 } },
        { typeof(AlaphaTestBlock), new int[] { 15, 17, 44, 60 } },
        { typeof(TreeLeaveBlock), new int[] { 12, 13, 14, 225 } },
        { typeof(PaintableCubeBlock), new int[] { 3, 4, 5, 21, 26, 67, 68, 72, 73 } },
        { typeof(StairBlock), new int[] { 48, 49, 50, 51, 69, 76, 96, 217 } },
        { typeof(SlabBlock), new int[] { 52, 53, 54, 55, 70, 75, 95, 136 } },
        { typeof(FurnitureBlock), new int[] { 227 } },
        { typeof(TorchBlock), new int[] { 31 } },
        { typeof(RotatableGateBlock), new int[] { 186 } }
    };

    public static Block GetBlock(int content)
    {
        if (content < blocks.Length)
        {
            return blocks[content];
        }
        if (content == 1023)
        {
            return nullBlock;
        }
        return blocks[0];
    }

    public static Color[] DEFAULT_COLORS = new Color[] {
        new Color32 (255, 255, 255, 255),
        new Color32 (181, 255, 255, 255),
        new Color32 (255, 181, 255, 255),
        new Color32 (160, 181, 255, 255),
        new Color32 (255, 240, 160, 255),
        new Color32 (181, 255, 181, 255),
        new Color32 (255, 181, 160, 255),
        new Color32 (181, 181, 181, 255),
        new Color32 (112, 112, 112, 255),
        new Color32 (32, 112, 112, 255),
        new Color32 (112, 32, 112, 255),
        new Color32 (26, 52, 128, 255),
        new Color32 (87, 54, 31, 255),
        new Color32 (24, 116, 24, 255),
        new Color32 (136, 32, 32, 255),
        new Color32 (24, 24, 24, 255)
    };

    public static Color GetBlockColor(Block b, int value)
    {
        if (b is PaintableBlock)
        {
            return ((PaintableBlock)b).GetColorC(value);
        }
        return Color.white;
    }

    public static int GetBlockColorInt(Block b, int value)
    {
        if (b is PaintableBlock)
        {
            int? i = ((PaintableBlock)b).GetColor(BlockTerrain.GetData(value));
            return i.HasValue ? i.Value : 0;
        }
        return 0;
    }

    public static Color ParseColor(string str)
    {
        string[] rgb = str.Split(',');
        return new Color((float)int.Parse(rgb[0]) / 256f, (float)int.Parse(rgb[1]) / 256f, (float)int.Parse(rgb[2]) / 256f);
    }

    void Awake()
    {
        string[] strs = WorldManager.Project.GetGameInfo().Colors;
        for (int i = 0; i < 16; i++)
        {
            if (!string.IsNullOrEmpty(strs[i]))
            {
                DEFAULT_COLORS[i] = ParseColor(strs[i]);
            }
        }
        ParseBlocksData(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "BlocksData.txt");
        GetComponent<FurnitureManager>().AlaphaTest4();
    }

    void ParseBlocksData(string path)
    {
        Dictionary<int, BlockData> blockData = new Dictionary<int, BlockData>();
		using (TextReader reader = AssetUtils.LoadText(path))
        {
            reader.ReadLine();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                BlockData data = new BlockData(line);
                blockData[data.Index] = data;
            }
        }

        Dictionary<int, Block> blockClass = new Dictionary<int, Block>();
        foreach (System.Type t in definedBlocks.Keys)
        {
            foreach (int index in definedBlocks[t])
            {
                blockClass[index] = (Block)System.Activator.CreateInstance(t);
            }
        }

        List<Block> b = new List<Block>();
        int i = 0;
        int count = blockData.Count;
        while (i < count)
        {
            if (blockData.ContainsKey(i))
            {
                Block block;
                if (blockClass.ContainsKey(i))
                {
                    block = blockClass[i];
                }
                else
                {
                    block = new CubeBlock();
                }
                LoadBlockClass(block, blockData[i]);
                b.Add(block);
                i++;
            }
            else
            {
                Block block = new CubeBlock();
                LoadBlockClass(block, blockData[0]);
                b.Add(block);
            }
        }

        blocks = b.ToArray();
    }

    void LoadBlockClass(Block block, BlockData data)
    {
        block.Index = data.Index;
        block.Name = data.Name;
        block.TextureSlot = data.TextureSlot;
        block.Initialize();
    }

    public static bool IsTransparent(BlockTerrain.Chunk chunk, int x, int y, int z)
    {
        return GetBlock(chunk.GetCellContent(x, y, z)).IsTransparent;
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

    public static Vector3[] offsets = new Vector3[] {
        new Vector3 (0.0f, 0.0f, 1.0f),
        new Vector3 (0.0f, 1.0f, 0.0f),
        new Vector3 (0.0f, 1.0f, 1.0f),
        new Vector3 (1.0f, 0.0f, 0.0f),
        new Vector3 (1.0f, 0.0f, 1.0f),
        new Vector3 (1.0f, 1.0f, 0.0f),
        new Vector3 (1.0f, 1.0f, 1.0f)
    };

    public int Index;
    public bool IsTransparent;
    //public bool IsCubic = true;
    public int TextureSlot;

    public string Name;

    public string ToString(int value)
    {
        return string.Format("{0}: {1}, {2}", Name, Index, BlockTerrain.GetData(value));
    }

    public virtual void Initialize()
    {
    }

    public virtual int GetTextureSlot(int value, int face)
    {
        return TextureSlot;
    }

    protected void DrawCubeBlock(int x, int y, int z, int value, int neighborData, Color color, TerrainGenerator g)
    {
        Vector3 v000 = new Vector3(x, y, z);
        Vector3 v001 = v000 + offsets[0];
        Vector3 v010 = v000 + offsets[1];
        Vector3 v011 = v000 + offsets[2];
        Vector3 v100 = v000 + offsets[3];
        Vector3 v101 = v000 + offsets[4];
        Vector3 v110 = v000 + offsets[5];
        Vector3 v111 = v000 + offsets[6];

        if ((neighborData & XminusOne) != 0)
        {
            g.MeshFromRect(v001, v011, v010, v000);
            g.GenerateBlockUVs(GetTextureSlot(value, CellFace.BACK));
            g.GenerateBlockColors(color);
        }
        if ((neighborData & YminusOne) != 0)
        {
            g.MeshFromRect(v000, v100, v101, v001);
            g.GenerateBlockUVs(GetTextureSlot(value, CellFace.BOTTOM));
            g.GenerateBlockColors(color);
        }
        if ((neighborData & ZminusOne) != 0)
        {
            g.MeshFromRect(v000, v010, v110, v100);
            g.GenerateBlockUVs(GetTextureSlot(value, CellFace.LEFT));
            g.GenerateBlockColors(color);
        }
        if ((neighborData & XplusOne) != 0)
        {
            g.MeshFromRect(v100, v110, v111, v101);
            g.GenerateBlockUVs(GetTextureSlot(value, CellFace.FRONT));
            g.GenerateBlockColors(color);
        }
        if ((neighborData & YplusOne) != 0)
        {
            g.MeshFromRect(v111, v110, v010, v011);
            g.GenerateBlockUVs(GetTextureSlot(value, CellFace.TOP));
            g.GenerateBlockColors(color);
        }
        if ((neighborData & ZplusOne) != 0)
        {
            g.MeshFromRect(v101, v111, v011, v001);
            g.GenerateBlockUVs(GetTextureSlot(value, CellFace.RIGHT));
            g.GenerateBlockColors(color);
        }
    }

    protected void DrawMeshBlock(int x, int y, int z, int value, MeshData mesh, Color color, TerrainGenerator g)
    {
        g.MeshFromMesh(x, y, z, mesh);
        g.GenerateTextureForMesh(mesh, GetTextureSlot(value, CellFace.FRONT), color);
    }

    public abstract void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g);
}

public abstract class PaintableBlock : Block
{
    int paintTextureSlot;

    Dictionary<int, int> paintedTextures = new Dictionary<int, int>()
    {
        {4, 23},
        {70, 39},
        {6, 40},
        {176, 64},
        {7, 51},
        {54, 50},
        {8, 147},
        {16, 69},
        {1, 24}
    };

    public override void Initialize()
    {
        paintTextureSlot = paintedTextures[TextureSlot];
    }

    public abstract int? GetColor(int data);

    public Color GetColorC(int value)
    {
        int? i = GetColor(BlockTerrain.GetData(value));
        if (i.HasValue)
        {
            return BlocksData.DEFAULT_COLORS[i.Value];
        }
        return Color.white;
    }

    public override int GetTextureSlot(int value, int face)
    {
        if (GetColor(BlockTerrain.GetData(value)).HasValue)
        {
            return paintTextureSlot;
        }
        return TextureSlot;
    }
}

public class AirBlock : Block
{
    public override void Initialize()
    {
        IsTransparent = true;
    }

    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
    {
    }
}

public class CubeBlock : Block
{
    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
    {
        int neighborData = 0;
        neighborData += (BlocksData.IsTransparent(chunk, x - 1, y, z)) ? XminusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y - 1, z)) ? YminusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y, z - 1)) ? ZminusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x + 1, y, z)) ? XplusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y + 1, z)) ? YplusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y, z + 1)) ? ZplusOne : 0;

        DrawCubeBlock(x, y, z, value, neighborData, Color.white, g);
    }
}

public class WaterBlock : Block
{
    private static ColorMap map = new ColorMap(new Color32(0, 0, 128, 255), new Color32(0, 80, 100, 255), new Color32(0, 45, 85, 255), new Color32(0, 113, 97, 255));

    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
    {
        int neighborData = 0;
        neighborData += (BlocksData.IsTransparent(chunk, x - 1, y, z)) ? XminusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y - 1, z)) ? YminusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y, z - 1)) ? ZminusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x + 1, y, z)) ? XplusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y + 1, z)) ? YplusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y, z + 1)) ? ZplusOne : 0;

        DrawCubeBlock(x, y, z, value, neighborData, map.Lookup(chunk.GetShiftValue(x, z)), g);
    }
}

public class AlaphaTestBlock : Block
{
    public override void Initialize()
    {
        IsTransparent = true;
    }

    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
    {
        int neighborData = 0;
        neighborData += GetNeightbor(chunk, x - 1, y, z, XminusOne);
        neighborData += GetNeightbor(chunk, x, y - 1, z, YminusOne);
        neighborData += GetNeightbor(chunk, x, y, z - 1, ZminusOne);
        neighborData += GetNeightbor(chunk, x + 1, y, z, XplusOne);
        neighborData += GetNeightbor(chunk, x, y + 1, z, YplusOne);
        neighborData += GetNeightbor(chunk, x, y, z + 1, ZplusOne);

        DrawCubeBlock(x, y, z, value, neighborData, Color.white, g);
    }

    int GetNeightbor(BlockTerrain.Chunk chunk, int x, int y, int z, int mask)
    {
        int content = chunk.GetCellContent(x, y, z);
        if (content != Index && BlocksData.GetBlock(content).IsTransparent)
        {
            return mask;
        }
        return 0;
    }
}

public class TreeLeaveBlock : Block
{
    private ColorMap map;

    public override void Initialize()
    {
        IsTransparent = true;
        switch (Index)
        {
            case 12:
                map = new ColorMap(new Color32(96, 161, 123, 255), new Color32(174, 164, 42, 255), new Color32(96, 161, 123, 255), new Color32(30, 191, 1, 255));
                break;
            case 13:
                map = new ColorMap(new Color32(96, 161, 96, 255), new Color32(174, 109, 42, 255), new Color32(96, 161, 96, 255), new Color32(107, 191, 1, 255));
                break;
            case 14:
                map = new ColorMap(new Color32(96, 161, 150, 255), new Color32(129, 174, 42, 255), new Color32(96, 161, 150, 255), new Color32(1, 191, 53, 255));
                break;
            case 225:
                map = new ColorMap(new Color32(90, 141, 160, 255), new Color32(119, 152, 51, 255), new Color32(86, 141, 162, 255), new Color32(1, 158, 65, 255));
                break;
        }
    }

    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
    {
        int neighborData = 0;
        neighborData += GetNeightbor(chunk, x - 1, y, z, XminusOne);
        neighborData += GetNeightbor(chunk, x, y - 1, z, YminusOne);
        neighborData += GetNeightbor(chunk, x, y, z - 1, ZminusOne);
        neighborData += GetNeightbor(chunk, x + 1, y, z, XplusOne);
        neighborData += GetNeightbor(chunk, x, y + 1, z, YplusOne);
        neighborData += GetNeightbor(chunk, x, y, z + 1, ZplusOne);

        DrawCubeBlock(x, y, z, value, neighborData, map.Lookup(chunk.GetShiftValue(x, z)), g);
    }

    int GetNeightbor(BlockTerrain.Chunk chunk, int x, int y, int z, int mask)
    {
        int content = chunk.GetCellContent(x, y, z);
        if (content != Index && BlocksData.GetBlock(content).IsTransparent)
        {
            return mask;
        }
        return 0;
    }
}

public class GrassBlock : Block
{
    public static ColorMap map = new ColorMap(new Color32(141, 198, 166, 255), new Color32(210, 201, 93, 255), new Color32(141, 198, 166, 255), new Color32(79, 225, 56, 255));

    public override int GetTextureSlot(int value, int face)
    {
        if (face == CellFace.TOP)
            return 0;
        return 3;
    }

    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
    {
        int neighborData = 0;
        neighborData += (BlocksData.IsTransparent(chunk, x - 1, y, z)) ? XminusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y - 1, z)) ? YminusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y, z - 1)) ? ZminusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x + 1, y, z)) ? XplusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y + 1, z)) ? YplusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y, z + 1)) ? ZplusOne : 0;

        Vector3 v000 = new Vector3(x, y, z);
        Vector3 v001 = v000 + offsets[0];
        Vector3 v010 = v000 + offsets[1];
        Vector3 v011 = v000 + offsets[2];
        Vector3 v100 = v000 + offsets[3];
        Vector3 v101 = v000 + offsets[4];
        Vector3 v110 = v000 + offsets[5];
        Vector3 v111 = v000 + offsets[6];

        Color white = Color.white;

        if ((neighborData & XminusOne) != 0)
        {
            g.MeshFromRect(v001, v011, v010, v000);
            g.GenerateBlockUVs(GetTextureSlot(value, CellFace.BACK));
            g.GenerateBlockColors(white);
        }
        if ((neighborData & YminusOne) != 0)
        {
            g.MeshFromRect(v000, v100, v101, v001);
            g.GenerateBlockUVs(GetTextureSlot(value, CellFace.BOTTOM));
            g.GenerateBlockColors(white);
        }
        if ((neighborData & ZminusOne) != 0)
        {
            g.MeshFromRect(v000, v010, v110, v100);
            g.GenerateBlockUVs(GetTextureSlot(value, CellFace.LEFT));
            g.GenerateBlockColors(white);
        }
        if ((neighborData & XplusOne) != 0)
        {
            g.MeshFromRect(v100, v110, v111, v101);
            g.GenerateBlockUVs(GetTextureSlot(value, CellFace.FRONT));
            g.GenerateBlockColors(white);
        }
        if ((neighborData & YplusOne) != 0)
        {
            g.MeshFromRect(v111, v110, v010, v011);
            g.GenerateBlockUVs(GetTextureSlot(value, CellFace.TOP));
            g.GenerateBlockColors(map.Lookup(chunk.GetShiftValue(x, z)));
        }
        if ((neighborData & ZplusOne) != 0)
        {
            g.MeshFromRect(v101, v111, v011, v001);
            g.GenerateBlockUVs(GetTextureSlot(value, CellFace.RIGHT));
            g.GenerateBlockColors(white);
        }
    }
}

public class TreeBlock : CubeBlock
{
    public override int GetTextureSlot(int value, int face)
    {
        if (face == CellFace.TOP || face == CellFace.BOTTOM)
            return 21;
        return TextureSlot;
    }
}

public class PaintableCubeBlock : PaintableBlock
{
    public override int? GetColor(int data)
    {
        if ((data & 1) != 0)
        {
            return new int?(data >> 1 & 15);
        }
        return null;
    }

    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
    {
        int neighborData = 0;
        neighborData += (BlocksData.IsTransparent(chunk, x - 1, y, z)) ? XminusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y - 1, z)) ? YminusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y, z - 1)) ? ZminusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x + 1, y, z)) ? XplusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y + 1, z)) ? YplusOne : 0;
        neighborData += (BlocksData.IsTransparent(chunk, x, y, z + 1)) ? ZplusOne : 0;

        DrawCubeBlock(x, y, z, value, neighborData, GetColorC(value), g);
    }
}

public class XBlock : Block
{
    public override void Initialize()
    {
        IsTransparent = true;
        //IsCubic = false;
    }

    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
    {
        Vector3 v000 = new Vector3(x, y, z);
        Vector3 v001 = new Vector3(x, y, z + 1.0f);
        Vector3 v010 = new Vector3(x, y + 1.0f, z);
        Vector3 v011 = new Vector3(x, y + 1.0f, z + 1.0f);
        Vector3 v100 = new Vector3(x + 1.0f, y, z);
        Vector3 v101 = new Vector3(x + 1.0f, y, z + 1.0f);
        Vector3 v110 = new Vector3(x + 1.0f, y + 1.0f, z);
        Vector3 v111 = new Vector3(x + 1.0f, y + 1.0f, z + 1.0f);

        int textureSlot = TextureSlot;
        Color color = Color.white;

        g.MeshFromRect(v101, v111, v010, v000);
        g.GenerateBlockUVs(textureSlot);
        g.GenerateBlockColors(color);
        g.MeshFromRect(v000, v010, v111, v101);
        g.GenerateBlockUVs(textureSlot);
        g.GenerateBlockColors(color);
        g.MeshFromRect(v100, v110, v011, v001);
        g.GenerateBlockUVs(textureSlot);
        g.GenerateBlockColors(color);
        g.MeshFromRect(v001, v011, v110, v100);
        g.GenerateBlockUVs(textureSlot);
        g.GenerateBlockColors(color);
    }
}

public class XGrassBlock : Block
{
    public static bool GetIsSmall(int data)
    {
        return (data & 8) != 0;
    }

    public override void Initialize()
    {
        IsTransparent = true;
        //IsCubic = false;
    }

    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
    {
        Vector3 v000 = new Vector3(x, y, z);
        Vector3 v001 = new Vector3(x, y, z + 1.0f);
        Vector3 v010 = new Vector3(x, y + 1.0f, z);
        Vector3 v011 = new Vector3(x, y + 1.0f, z + 1.0f);
        Vector3 v100 = new Vector3(x + 1.0f, y, z);
        Vector3 v101 = new Vector3(x + 1.0f, y, z + 1.0f);
        Vector3 v110 = new Vector3(x + 1.0f, y + 1.0f, z);
        Vector3 v111 = new Vector3(x + 1.0f, y + 1.0f, z + 1.0f);

        int textureSlot = GetIsSmall(BlockTerrain.GetData(value)) ? 84 : 85;
        Color color = GrassBlock.map.Lookup(chunk.GetShiftValue(x, z));

        g.MeshFromRect(v101, v111, v010, v000);
        g.GenerateBlockUVs(textureSlot);
        g.GenerateBlockColors(color);
        g.MeshFromRect(v000, v010, v111, v101);
        g.GenerateBlockUVs(textureSlot);
        g.GenerateBlockColors(color);
        g.MeshFromRect(v100, v110, v011, v001);
        g.GenerateBlockUVs(textureSlot);
        g.GenerateBlockColors(color);
        g.MeshFromRect(v001, v011, v110, v100);
        g.GenerateBlockUVs(textureSlot);
        g.GenerateBlockColors(color);
    }
}

public class IvyBlock : Block
{
    private static ColorMap map = new ColorMap(new Color32(96, 161, 123, 255), new Color32(174, 164, 42, 255), new Color32(96, 161, 123, 255), new Color32(30, 191, 1, 255));

    public override void Initialize()
    {
        IsTransparent = true;
        //IsCubic = false;
    }

    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
    {
        int face = GetFace(BlockTerrain.GetData(value));

        Vector3 v0;
        Vector3 v1;
        Vector3 v2;
        Vector3 v3;
        switch (face)
        {
            case 1:
                v0 = new Vector3(x, y, z);
                v1 = new Vector3(x, y + 1.0f, z);
                v2 = new Vector3(x, y + 1.0f, z + 1.0f);
                v3 = new Vector3(x, y, z + 1.0f);
                break;
            case 0:
                v0 = new Vector3(x, y, z + 1.0f);
                v1 = new Vector3(x, y + 1.0f, z + 1.0f);
                v2 = new Vector3(x + 1.0f, y + 1.0f, z + 1.0f);
                v3 = new Vector3(x + 1.0f, y, z + 1.0f);
                break;
            case 3:
                v0 = new Vector3(x + 1.0f, y, z + 1.0f);
                v1 = new Vector3(x + 1.0f, y + 1.0f, z + 1.0f);
                v2 = new Vector3(x + 1.0f, y + 1.0f, z);
                v3 = new Vector3(x + 1.0f, y, z);
                break;
            case 2:
                v0 = new Vector3(x + 1.0f, y, z);
                v1 = new Vector3(x + 1.0f, y + 1.0f, z);
                v2 = new Vector3(x, y + 1.0f, z);
                v3 = new Vector3(x, y, z);
                break;
            default:
                throw new UnityException("undefined face: " + face);
        }

        Color color = map.Lookup(chunk.GetShiftValue(x, z));
        //Color color = Color.white;

        int count = g.vertices.Count;
        g.vertices.Add(v0);
        g.vertices.Add(v1);
        g.vertices.Add(v2);
        g.vertices.Add(v3);

        g.triangles.Add(count);
        g.triangles.Add(count + 1);
        g.triangles.Add(count + 2);
        g.triangles.Add(count + 2);
        g.triangles.Add(count + 3);
        g.triangles.Add(count);

        g.triangles.Add(count + 2);
        g.triangles.Add(count + 3);
        g.triangles.Add(count + 1);
        g.triangles.Add(count + 3);
        g.triangles.Add(count);
        g.triangles.Add(count + 1);

        g.GenerateBlockUVs(TextureSlot);
        g.GenerateBlockColors(color);
    }

    public static int GetFace(int data)
    {
        return data & 3;
    }
}

public class StairBlock : PaintableBlock
{
    MeshData[] stairs = new MeshData[24];

    public override void Initialize()
    {
        base.Initialize();
        IsTransparent = true;
        //IsCubic = false;

        float y;
        Matrix4x4 m;
        Mesh mesh;
        for (int i = 0; i < 24; i++)
        {
            y = 0;

            y -= GetRotation(i) * 90;

            m = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, y, 0), Vector3.one);
            switch ((i >> 3) & 3)
            {
                case 1:
                    mesh = BlockMeshes.FindMesh("Stair0");
                    break;
                case 0:
                    mesh = BlockMeshes.FindMesh("Stair1");
                    break;
                case 2:
                    mesh = BlockMeshes.FindMesh("Stair2");
                    break;
                default:
                    throw new System.Exception("unknown stair module: " + ((i >> 3) & 3));
            }

            stairs[i] = new MeshData(BlockMeshes.TranslateMesh(mesh, m, (i & 4) != 0));
        }
    }

    public static int GetRotation(int data)
    {
        return data & 3;
    }

    public override int? GetColor(int data)
    {
        if ((data & 32) != 0)
        {
            return new int?(data >> 6 & 15);
        }
        return null;
    }

    public static int GetVariant(int data)
    {
        return data & 31;
    }

    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
    {
        DrawMeshBlock(x, y, z, value, stairs[GetVariant(BlockTerrain.GetData(value))], GetColorC(value), g);
    }
}

public class SlabBlock : PaintableBlock
{
    MeshData[] slabs = new MeshData[2];

    public override void Initialize()
    {
        base.Initialize();
        IsTransparent = true;
        //IsCubic = false;

        Mesh slab = BlockMeshes.FindMesh("Slab");

        slabs[0] = new MeshData(slab);
        slabs[1] = new MeshData(BlockMeshes.UpsideDownMesh(slab));
    }

    public override int? GetColor(int data)
    {
        if ((data & 2) != 0)
        {
            return new int?(data >> 2 & 15);
        }
        return null;
    }

    public static bool GetIsTop(int data)
    {
        return (data & 1) != 0;
    }

    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
    {
        DrawMeshBlock(x, y, z, value, slabs[GetIsTop(BlockTerrain.GetData(value)) ? 1 : 0], GetColorC(value), g);
    }
}

public class NullBlock : Block
{
    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
    {
    }
}

public class TorchBlock : Block
{
    MeshData[] meshes = new MeshData[5];

    public override void Initialize()
    {
        IsTransparent = true;
        MeshData mesh = new MeshData(BlockMeshes.FindMesh("Torch"));
        meshes[0] = mesh.Transform(Matrix4x4.Rotate(Quaternion.Euler(34, 0, 0)) * Matrix4x4.Translate(new Vector3(0.5f, 0.15f, -0.05f)));
        meshes[1] = mesh.Transform(Matrix4x4.Rotate(Quaternion.Euler(34, 90, 0)) * Matrix4x4.Translate(new Vector3(-0.05f, 0.15f, 0.5f)));
        meshes[2] = mesh.Transform(Matrix4x4.Rotate(Quaternion.Euler(34, 180, 0)) * Matrix4x4.Translate(new Vector3(0.5f, 0.15f, 1.05f)));
        meshes[3] = mesh.Transform(Matrix4x4.Rotate(Quaternion.Euler(34, 270, 0)) * Matrix4x4.Translate(new Vector3(1.05f, 0.15f, 0.5f)));
        meshes[4] = mesh.Transform(Matrix4x4.Translate(new Vector3(0.5f, 0f, 0.5f)));
    }

    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
    {
        g.MeshFromMesh(x, y, z, meshes[BlockTerrain.GetData(value)], true);
    }
}

public class RotatableGateBlock : Block
{
    MeshData[] meshes = new MeshData[24];

    public override void Initialize()
    {
        IsTransparent = true;

        Mesh blockMesh;
        switch (Index)
        {
            case 134:
                blockMesh = BlockMeshes.FindMesh("NandGate");
                break;
            case 135:
                blockMesh = BlockMeshes.FindMesh("NorGate");
                break;
            case 137:
                blockMesh = BlockMeshes.FindMesh("AndGate");
                break;
            case 140:
                blockMesh = BlockMeshes.FindMesh("NotGate");
                break;
            case 143:
                blockMesh = BlockMeshes.FindMesh("OrGate");
                break;
            case 145:
                blockMesh = BlockMeshes.FindMesh("DelayGate");
                break;
            case 146:
                blockMesh = BlockMeshes.FindMesh("SRLatch");
                break;
            case 156:
                blockMesh = BlockMeshes.FindMesh("XorGate");
                break;
            case 157:
                blockMesh = BlockMeshes.FindMesh("RandomGenerator");
                break;
            case 179:
                blockMesh = BlockMeshes.FindMesh("MotionDetector");
                break;
            case 180:
                blockMesh = BlockMeshes.FindMesh("DigitalToAnalogConverter");
                break;
            case 181:
                blockMesh = BlockMeshes.FindMesh("AnalogToDigitalConverter");
                break;
            case 183:
                blockMesh = BlockMeshes.FindMesh("SoundGenerator");
                break;
            case 184:
                blockMesh = BlockMeshes.FindMesh("Counter");
                break;
            case 186:
                blockMesh = BlockMeshes.FindMesh("MemoryBank");
                break;
            case 187:
                blockMesh = BlockMeshes.FindMesh("RealTimeClock");
                break;
            case 188:
                blockMesh = BlockMeshes.FindMesh("TruthTableCircuit");
                break;
            default:
                throw new System.Exception("unsupported electric gate: " + Index);
        }

        Matrix4x4 m;
        Matrix4x4 m2;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                switch (j)
                {
                    case 0:
                        m = Matrix4x4.Rotate(Quaternion.Euler(0, 0, i * 90));
                        m2 = Matrix4x4.TRS(new Vector3(0.5f, -0.5f, 0f), Quaternion.Euler(90, 0, 0), Vector3.one);
                        break;
                    case 1:
                        m = Matrix4x4.Rotate(Quaternion.Euler(i * -90, 0, 0));
                        m2 = Matrix4x4.TRS(new Vector3(0f, -0.5f, 0.5f), Quaternion.Euler(90, 0, -270), Vector3.one);
                        break;
                    case 2:
                        m = Matrix4x4.Rotate(Quaternion.Euler(0, 0, i * -90));
                        m2 = Matrix4x4.TRS(new Vector3(-0.5f, -0.5f, 0f), Quaternion.Euler(90, 0, -180), Vector3.one);
                        break;
                    case 3:
                        m = Matrix4x4.Rotate(Quaternion.Euler(i * 90, 0, 0));
                        m2 = Matrix4x4.TRS(new Vector3(0f, -0.5f, -0.5f), Quaternion.Euler(90, 0, -90), Vector3.one);
                        break;
                    case 4:
                        m = Matrix4x4.Rotate(Quaternion.Euler(0, i * 90, 0));
                        m2 = Matrix4x4.Translate(new Vector3(0.5f, 0f, 0.5f));
                        break;
                    case 5:
                        m = Matrix4x4.Rotate(Quaternion.Euler(0, i * -90, 0));
                        m2 = Matrix4x4.TRS(new Vector3(0.5f, 0f, -0.5f), Quaternion.Euler(180, 0, 0), Vector3.one);
                        break;
                    default:
                        throw new System.Exception();
                }

                meshes[(j << 2) + i] = new MeshData(BlockMeshes.TranslateMeshRaw(blockMesh, m * m2));
            }
        }
    }

    public static int GetFace(int value)
    {
        return BlockTerrain.GetData(value) >> 2 & 7;
    }

    public override void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, TerrainGenerator g)
    {
        g.MeshFromMesh(x, y, z, meshes[BlockTerrain.GetData(value) & 31], true);
    }
}
