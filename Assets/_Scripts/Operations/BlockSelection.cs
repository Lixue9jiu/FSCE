using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSelection
{
	List<Point3> blocks;

	Point3 center;

	public BlockSelection ()
	{
		blocks = new List<Point3> ();
	}

	private BlockSelection (Point3 center, Point3[] pos)
	{
		this.center = center;
		blocks = new List<Point3> (pos);
	}

	void SetCenterAddition (Point3 _center)
	{
		if (blocks.Count == 0) {
			center = _center;
		} else {
			center = (center + _center) / 2;
		}
	}

	void SetCenterDeletion (Point3 _center)
	{
		if (blocks.Count == 0) {
			center = _center;
		} else {
			center = center - ((_center - center) / 2);
		}
	}

	public void AddToSelection (Point3 start, Point3 end)
	{
		int sizex = start.X - end.X;
		int sizey = start.Y - end.Y;
		int sizez = start.Z - end.Z;

		for (int x = 0; x < sizex; x++) {
			for (int y = 0; y < sizey; y++) {
				for (int z = 0; z < sizez; z++) {
					blocks.Add (new Point3 (x + start.X, y + start.Y, z + start.Z));
				}
			}
		}

		SetCenterAddition ((start + end) / 2);
	}

	public void RemoveFromSelection (Point3 start, Point3 end)
	{
		int sizex = start.X - end.X;
		int sizey = start.Y - end.Y;
		int sizez = start.Z - end.Z;

		for (int x = 0; x < sizex; x++) {
			for (int y = 0; y < sizey; y++) {
				for (int z = 0; z < sizez; z++) {
					blocks.Remove (new Point3 (x + start.X, y + start.Y, z + start.Z));
				}
			}
		}

		SetCenterDeletion ((start + end) / 2);
	}

	public void Export (BlockSelectionPacker packer)
	{
		Point3 p;
		for (int i = 0; i < blocks.Count; i++) {
			p = blocks [i];
			packer.AddBlock (p.X, p.Y, p.Z);
		}
	}

	public static BlockSelection Load (BlockSelectionPacker packer)
	{
		Point3[] positions = BlockSelectionPacker.ItemBlockPositions.LoadData (packer);
		Point3 c = positions [0];
		for (int i = 1; i < positions.Length; i++) {
			c = (c + positions [i]) / 2;
		}
		return new BlockSelection (c, positions);
	}

	public void Clear ()
	{
		blocks.Clear ();
	}
}
