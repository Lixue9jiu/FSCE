using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using UnityEngine;

public static class XMLUtils
{

	public static XElement FindValuesByName (XElement elem, string name)
	{
		return elem.Elements ("Values").Single (e => e.Attribute ("Name").Value == name);
	}

	public static string FindValueByName (XElement elem, string name)
	{
		return elem.Elements ("Value").Single (e => e.Attribute ("Name").Value == name).Attribute ("Value").Value;
	}

	public static void GetValue<T> (this XElement elem, string name, out T value)
	{
		value = GetValue<T> (elem, name);
	}

	public static T GetValue<T> (this XElement elem, string name)
	{
		return (T)Convert (FindValueByName (elem, name), typeof(T));
	}

	public static XElement GetValues (this XElement elem, string name)
	{
		return FindValuesByName (elem, name);
	}

	static object Convert (string value, System.Type type)
	{
		if (type == typeof(Vector3)) {
			string[] strs = value.Split (',');
			return new Vector3 (float.Parse (strs [0]), float.Parse (strs [1]), float.Parse (strs [2]));
		}
		return System.Convert.ChangeType (value, type);
	}
}
