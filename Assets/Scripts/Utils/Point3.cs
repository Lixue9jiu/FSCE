using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point3 {

	public int X;
	public int Y;
	public int Z;

	public Point3(int x, int y, int z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public override int GetHashCode ()
	{
		return X + Y + Z;
	}

	public bool Equals(Point3 p)
	{
		return X == p.X && Y == p.Y && Z == p.Z;
	}

	public override bool Equals (object obj)
	{
		return obj is Point3 && Equals ((Point3)obj);
	}

	public override string ToString ()
	{
		return string.Format ("{0}, {1}, {2}", X, Y, Z);
	}

	public Vector3 ToVec3()
	{
		return new Vector3 (X, Y, Z);
	}
}
