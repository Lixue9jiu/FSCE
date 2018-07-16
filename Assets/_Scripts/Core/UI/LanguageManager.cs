using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour
{

	static Dictionary<string, string> currentMap = new Dictionary<string, string>();

	void Start()
	{
		if (currentMap.Count == 0)
		{
			SwitchLanguage(Application.systemLanguage.ToString());
		}

		ReplaceText();
	}

	public void ReplaceText()
	{
		Text[] all = GetComponentsInChildren<Text>(true);
		for (int i = 0; i < all.Length; i++)
		{
			Text t = all[i];
			t.text = GetString(t.text);
		}
	}

	public static string GetString(string id)
	{
		string value;
		if (currentMap.TryGetValue(id, out value))
		{
			return value;
		}
		return id;
	}

	public static void SwitchLanguage(string language)
	{
		currentMap.Clear();

		using (TextReader reader = AssetUtils.LoadText(Path.Combine(Application.streamingAssetsPath, "strings.xml")))
		{
			XElement root = XDocument.Load(reader).Root;
			XElement all = root.Element(language);

			if (all == null)
			{
				all = root.Element("English");
			}
			else
			{
				XAttribute aka = all.Attribute("aka");
				if (aka != null)
					all = root.Element(aka.Value);
			}

			foreach (XElement elem in all.Elements())
			{
				currentMap[elem.Attribute("id").Value] = elem.Value;
			}
		}
		Debug.Log("running in: " + Application.systemLanguage);
	}

	private void OnTransformChildrenChanged()
	{
		ReplaceText();
	}
}
