using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkInstanceManager : MonoBehaviour
{
    public GameObject ChunksObj;
    public GameObject terrainPrefab;
    public GameObject alphaTestPrefab;

    public void LoadChunkInstance(ChunkInstance instance)
    {
        if (instance.chunkObj != null)
        {
            GameObject obj = instance.chunkObj;
            obj.SetActive(true);
            obj.transform.position = instance.transform.MultiplyPoint3x4(Vector3.zero);
            obj.GetComponent<MeshFilter>().mesh = instance.meshes[0];

            //obj.GetComponentInChildren<MeshFilter>().mesh = instance.meshes[1];
        }
        else
        {
            GameObject obj = Instantiate(terrainPrefab, instance.transform.MultiplyPoint3x4(Vector3.zero), Quaternion.identity, ChunksObj.transform);
            obj.GetComponent<MeshFilter>().mesh = instance.meshes[0];
            instance.chunkObj = obj;

            GameObject obj2 = Instantiate(alphaTestPrefab, instance.transform.MultiplyPoint3x4(Vector3.zero), Quaternion.identity, obj.transform);
            obj2.GetComponent<MeshFilter>().mesh = instance.meshes[1];
        }
    }

    public void UpdateChunkInstance(ChunkInstance instance)
    {
        instance.chunkObj.GetComponent<MeshFilter>().mesh = instance.meshes[0];
    }

    public void UnloadChunkInstance(ChunkInstance instance)
    {
        instance.chunkObj.SetActive(false);
    }
}
