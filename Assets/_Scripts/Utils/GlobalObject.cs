using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalObject : MonoBehaviour {

	void Start () {
        DontDestroyOnLoad(gameObject);
		SceneManager.LoadScene ("MainMenu");
	}
}
