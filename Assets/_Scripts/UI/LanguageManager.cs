using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour
{

	static Dictionary<string, string> currentMap = new Dictionary<string, string> ();

	void Start ()
	{
		if (currentMap.Count == 0) {
			SwitchLanguage (Application.systemLanguage.ToString ());
		}

        ReplaceText();

		Debug.Log ("running in: " + Application.systemLanguage);
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

	public static string GetString (string id)
	{
        string value;
        if (currentMap.TryGetValue(id, out value))
        {
            return value;
        }
        return id;
	}

	public static void SwitchLanguage (string language)
	{
		using (Stream s = File.OpenRead (Path.Combine (Application.streamingAssetsPath, "strings.xml"))) {
			XElement root = XDocument.Load (s).Root;
			XElement all = root.Element (language);
			if (all == null) {
				all = root.Element ("English");
			}

			foreach (XElement elem in all.Elements ()) {
				currentMap [elem.Attribute ("id").Value] = elem.Value;
			}
		}
	}

	private void OnTransformChildrenChanged()
	{
        ReplaceText();
	}
}
