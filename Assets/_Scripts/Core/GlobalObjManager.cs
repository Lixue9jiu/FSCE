using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalObjManager : MonoBehaviour {

	public static GameObject globalObj;

	static GlobalObjManager()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		GameObject obj = new GameObject ();
		obj.name = "test";
		Instantiate (obj);
	}

	static void OnSceneLoaded (Scene arg0, LoadSceneMode arg1)
	{
		Debug.Log (arg0.name);
		GameObject obj = new GameObject ();
		obj.name = "test";
		Instantiate (obj);
	}

}
