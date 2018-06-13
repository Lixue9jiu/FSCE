using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point3
{

	public int X;
	public int Y;
	public int Z;

	public static Point3 one = new Point3 (1, 1, 1);

	public Point3 (int x, int y, int z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public override int GetHashCode ()
	{
		return X + Y + Z;
	}

	public bool Equals (Point3 p)
	{
		return X == p.X && Y == p.Y && Z == p.Z;
	}

	public override bool Equals (object obj)
	{
		return obj is Point3 && Equals ((Point3)obj);
	}

	public override string ToString ()
	{
		return string.Format ("{0},{1},{2}", X, Y, Z);
	}

	public Vector3 ToVec3 ()
	{
		return new Vector3 (X, Y, Z);
	}

	public static Point3 Min (Point3 a, Point3 b)
	{
		return new Point3 (Mathf.Min (a.X, b.X), Mathf.Min (a.Y, b.Y), Mathf.Min (a.Z, b.Z));
	}

	public static Point3 Max (Point3 a, Point3 b)
	{
		return new Point3 (Mathf.Max (a.X, b.X), Mathf.Max (a.Y, b.Y), Mathf.Max (a.Z, b.Z));
	}

	public static Point3 operator+ (Point3 a, Point3 b)
	{
		return new Point3 (a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	}

	public static Point3 operator- (Point3 a, Point3 b)
	{
		return new Point3 (a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	}

	public static Point3 operator* (Point3 a, int b)
	{
		return new Point3 (a.X * b, a.Y * b, a.Z * b);
	}

	public static Point3 operator/ (Point3 a, int b)
	{
		return new Point3 (a.X / b, a.Y / b, a.Z / b);
	}
}
