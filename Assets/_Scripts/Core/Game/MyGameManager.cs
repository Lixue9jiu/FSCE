using UnityEngine;
using UnityEngine.SceneManagement;

public class MyGameManager : MonoBehaviour
{
	public bool loadingSuccessful;
	
	private void Awake()
	{
		try
		{
			GetComponent<BlockMeshes>().Initialize();
            GetComponent<BlocksData>().Initialize();
            GetComponent<FurnitureManager>().Initialize();
			loadingSuccessful = true;
		}
		catch(System.Exception e)
		{
			loadingSuccessful = false;
			Debug.LogError("error loading game");
			Debug.LogException(e);
			SceneManager.LoadScene("MainMenu");
		}
	}
}
