using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoadingPanel : MonoBehaviour {

    public void OnLoadingFinished()
    {
        Destroy(gameObject);
    }
}
