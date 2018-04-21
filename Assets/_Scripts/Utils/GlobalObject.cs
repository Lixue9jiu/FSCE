using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalObject : MonoBehaviour {

	static bool isInitialized;

	void Awake ()
	{
		if (isInitialized)
			Destroy (gameObject);

		isInitialized = true;
	}

	void Start ()
	{
		DontDestroyOnLoad(gameObject);
	}
}
