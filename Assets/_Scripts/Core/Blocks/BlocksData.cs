using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class BlocksData : MonoBehaviour
{
	const string blocksDataFile = "BlocksData.csv";

	struct BlockData
	{
		public int Index;
		public string Name;
		public int TextureSlot;
		public string BlockType;

		public BlockData(string src)
		{
			string[] strs = src.Split(',');
			Index = int.Parse(strs[0]);
			Name = strs[1];
			TextureSlot = int.Parse(strs[2]);
			BlockType = strs[3];
		}
	}

	public static readonly Color[] DEFAULT_COLORS =
	{
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

	public static readonly Dictionary<int, int> paintedTextures = new Dictionary<int, int>
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

    public static Block[] Blocks { get; private set; }
	public static IStandardCubeBlock[] StandardCubeBlocks { get; private set; }
	public static INormalBlock[] NormalBlocks { get; private set; }

    public static bool[] IsTransparent { get; private set; }

	public static Color ColorFromInt(int? i)
	{
		return i.HasValue ? DEFAULT_COLORS[i.Value] : Color.white;
	}

	public static int GetBlockColorInt(Block block, int value)
	{
		IPaintableBlock paintable = block as IPaintableBlock;
		if (paintable == null)
			return 0;
		return paintable.GetColor(BlockTerrain.GetData(value)) ?? 0;
	}

	public void Initialize()
	{
		string[] strs = WorldManager.Project.GetGameInfo().Colors;
        for (int i = 0; i < 16; i++)
        {
            if (!string.IsNullOrEmpty(strs[i]))
            {
                string[] rgb = strs[i].Split(',');
                DEFAULT_COLORS[i] = new Color((float)int.Parse(rgb[0]) / 256f, (float)int.Parse(rgb[1]) / 256f, (float)int.Parse(rgb[2]) / 256f);
            }
        }
        Load();
	}

	void Load()
	{
		Dictionary<int, BlockData> blockData = new Dictionary<int, BlockData>();
		using (TextReader reader = AssetUtils.LoadText(Application.streamingAssetsPath + Path.DirectorySeparatorChar + blocksDataFile))
		{
			reader.ReadLine();
			string line;
			while ((line = reader.ReadLine()) != null)
			{
				BlockData data = new BlockData(line);
				blockData[data.Index] = data;
			}
		}

		Dictionary<string, System.Type> definedBlocks = new Dictionary<string, System.Type>();
		foreach (System.Type t in System.Reflection.Assembly.GetAssembly(typeof(Block)).GetTypes())
		{
			if (t.IsSubclassOf(typeof(Block)))
			{
				definedBlocks.Add(t.Name, t);
			}
		}

		List<Block> b = new List<Block>();
		int i = 0;
		int count = blockData.Count;
		while (i < count)
		{
			if (blockData.ContainsKey(i))
			{
				Block block = (Block)System.Activator.CreateInstance(definedBlocks[blockData[i].BlockType]);
				InitializeBlock(block, blockData[i]);
				b.Add(block);
				i++;
			}
			else
			{
				Block block = new CubeBlock();
				InitializeBlock(block, blockData[0]);
				b.Add(block);
			}
		}

		Blocks = b.ToArray();

		StandardCubeBlocks = new IStandardCubeBlock[Blocks.Length];
		NormalBlocks = new INormalBlock[Blocks.Length];
		IsTransparent = new bool[Blocks.Length];
		for (int k = 0; k < Blocks.Length; k++)
		{
			var standard = Blocks[k] as IStandardCubeBlock;
			var normal = Blocks[k] as INormalBlock;

			if (standard == null && normal == null)
				Debug.LogErrorFormat("block {0} does not defind GenerateTerrain", Blocks[k].GetType().Name);

			StandardCubeBlocks[k] = standard;
			NormalBlocks[k] = normal;
			IsTransparent[k] = standard == null;
		}
	}

	void InitializeBlock(Block block, BlockData data)
	{
		block.Index = data.Index;
		block.Name = data.Name;
		block.TextureSlot = data.TextureSlot;
		block.Initialize();
	}
}
