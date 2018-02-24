using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point2
{
	public int X;
	public int Y;

	public Point2(int x, int y)
	{
		X = x;
		Y = y;
	}

	public override int GetHashCode ()
	{
		return X + Y;
	}

	public bool Equals (Point2 p)
	{
		return p.X == X && p.Y == Y;
	}

	public override bool Equals (object obj)
	{
		return obj is Point2 && Equals((Point2)obj);
	}
}