using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkInstanceManager : MonoBehaviour
{
    //public GameObject chunksObj;
    //public GameObject chunkPrefab;

    //public Material[] terrain;
    //public Material[] all;

    //public void LoadChunkInstance(ChunkInstance instance)
    //{
    //    if (instance.chunkObj != null)
    //    {
    //        GameObject obj = instance.chunkObj;
    //        obj.GetComponent<MeshFilter>().mesh = instance.Meshes;
    //        obj.GetComponent<MeshRenderer>().sharedMaterials = instance.Meshes.subMeshCount == 1 ? terrain : all;
    //        obj.transform.position = instance.transform.MultiplyPoint3x4(Vector3.zero);
    //        obj.SetActive(true);
    //    }
    //    else
    //    {
    //        GameObject obj = Instantiate(chunkPrefab, instance.transform.MultiplyPoint3x4(Vector3.zero), Quaternion.identity, chunksObj.transform);
    //        obj.GetComponent<MeshFilter>().mesh = instance.Meshes;
    //        obj.GetComponent<MeshRenderer>().sharedMaterials = instance.Meshes.subMeshCount == 1 ? terrain : all;
    //        instance.chunkObj = obj;
    //    }
    //}

    //public void UpdateChunkInstance(ChunkInstance instance)
    //{
    //    GameObject obj = instance.chunkObj;
    //    obj.GetComponent<MeshFilter>().mesh = instance.Meshes;
    //    obj.GetComponent<MeshRenderer>().sharedMaterials = instance.Meshes.subMeshCount == 1 ? terrain : all;
    //}

    //public void UnloadChunkInstance(ChunkInstance instance)
    //{
    //    instance.chunkObj.SetActive(false);
    //}
}
