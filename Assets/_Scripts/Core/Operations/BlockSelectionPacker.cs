using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BlockSelectionPacker
{
	BlockTerrain terrain;
	XElement root;

	ItemDefination[] itemDefinations = new ItemDefination[]
	{
		new ItemBlockPositions(),
		new ItemBlocks()
	};

	public BlockSelectionPacker (BlockTerrain terrain)
	{
		this.terrain = terrain;
		root = new XElement ("Items");
	}

	public void AddBlock (int x, int y, int z)
	{
		int value = terrain.GetCellValue (x, y, z);
		foreach (ItemDefination i in itemDefinations) {
			i.AddBlock (x, y, z, value);
		}
	}

	public void AddData (string name, byte[] data)
	{
		AddData (name, Base64Encode (data));
	}

	public void AddData (string name, string data)
	{
		root.Add (new XElement ("Item", new XAttribute ("Name", name), data));
	}

	public byte[] GetDataBytes (string name)
	{
		return Base64Decode (GetData (name));
	}

	public string GetData (string name)
	{
		return GetItem (name).Value;
	}

	public XElement GetItem (string name)
	{
		return root.Elements ("Item").SingleOrDefault (e => e.Attribute ("Name").Value == name);
	}

	public void Save (string fileName)
	{
		for (int i = 0; i < itemDefinations.Length; i++) {
			PackItem (itemDefinations[i]);
		}
		root.Save (fileName);
	}

	void PackItem (ItemDefination defination)
	{
		XElement elem = new XElement ("Item", new XAttribute ("Name", defination.GetName ()));
		defination.SaveData (elem);
		root.Add (elem);
	}

	public static string Base64Encode (byte[] data)
	{
		return System.Convert.ToBase64String (data, 0, data.Length);
	}

	public static byte[] Base64Decode (string str)
	{
		return System.Convert.FromBase64String (str);
	}

	public static void ConvertArray (System.Array src, System.Array dst, int byteLength)
	{
		System.Buffer.BlockCopy (src, 0, dst, 0, byteLength);
	}

	interface ItemDefination
	{
		void AddBlock (int x, int y, int z, int value);

		string GetName ();

		void SaveData (XElement elem);

		void Clear ();
	}

	public class ItemBlockPositions : ItemDefination
	{
		public const string name = "BlockPositions";

		List<int> data = new List<int> ();

		public void AddBlock (int x, int y, int z, int value)
		{
			data.Add (x);
			data.Add (y);
			data.Add (z);
		}

		public string GetName ()
		{
			return name;
		}

		public void SaveData (XElement elem)
		{
			byte[] dataArray = new byte[data.Count * sizeof(int)];
			ConvertArray (data.ToArray (), dataArray, dataArray.Length);
			elem.SetValue (Base64Encode (dataArray));
		}

		public static Point3[] LoadData (BlockSelectionPacker packer)
		{
			byte[] dataArray = Base64Decode (packer.GetItem(name).Value);
			System.IO.BinaryReader reader = new System.IO.BinaryReader (new System.IO.MemoryStream (dataArray));
			Point3[] result = new Point3[dataArray.Length * sizeof(int) / 3];
			for (int i = 0; i < result.Length; i++) {
				result [i].X = reader.ReadInt32 ();
				result [i].Y = reader.ReadInt32 ();
				result [i].Z = reader.ReadInt32 ();
			}
			return result;
		}

		public void Clear ()
		{
			data.Clear ();
		}
	}

	public class ItemBlocks : ItemDefination
	{
		public const string name = "Blocks";

		List<int> data = new List<int> ();

		public void AddBlock (int x, int y, int z, int value)
		{
			data.Add (value);
		}

		public string GetName ()
		{
			return name;
		}

		public void SaveData (XElement elem)
		{
			byte[] dataArray = new byte[data.Count * sizeof(int)];
			ConvertArray (data.ToArray (), dataArray, dataArray.Length);
			elem.SetValue (Base64Encode (dataArray));
		}

		public static int[] LoadData (XElement elem)
		{
			byte[] dataArray = Base64Decode (elem.Value);
			int[] array = new int[dataArray.Length / sizeof(int)];
			ConvertArray (dataArray, array, dataArray.Length);
			return array;
		}

		public void Clear ()
		{
			data.Clear ();
		}
	}

	//class ItemMemoryBank : ItemDefination
	//{
	//    public void AddBlock(int x, int y, int z, int value)
	//    {
	//        throw new System.NotImplementedException();
	//    }

	//    public string GetName()
	//    {
	//        throw new System.NotImplementedException();
	//    }

	//    public void SaveData(XElement elem)
	//    {
	//        throw new System.NotImplementedException();
	//    }
	//}

	//class ItemFurniture : ItemDefination
	//{
	//    public void AddBlock(int x, int y, int z, int value)
	//    {
	//        throw new System.NotImplementedException();
	//    }

	//    public string GetName()
	//    {
	//        throw new System.NotImplementedException();
	//    }

	//    public void SaveData(XElement elem)
	//    {
	//        throw new System.NotImplementedException();
	//    }
	//}

}
