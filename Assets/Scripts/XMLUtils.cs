using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using UnityEngine;

public static class XMLUtils {

	public static XElement FindElementByName(XElement elem, string name)
	{
		return elem.Elements ("Values").Single (e => e.Attribute ("Name").Value == name);
	}

	public static string FindValueByName(XElement elem, string name)
	{
		return elem.Elements ("Value").Single (e => e.Attribute ("Name").Value == name).Attribute ("Value").Value;
	}
}
