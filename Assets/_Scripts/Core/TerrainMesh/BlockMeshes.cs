using UnityEngine;

//using UnityEditor;

public class BlockMeshes : MonoBehaviour
{
    static AssetBundle meshes;

	public void Initialize()
	{
		if (meshes == null)
            meshes = AssetBundle.LoadFromFile(System.IO.Path.Combine(Application.streamingAssetsPath, "meshes"));
	}

	private void OnDestroy()
	{
        meshes.Unload(true);
	}

    public static Mesh FindMesh(string str)
    {
        //Debug.Log("start finding mesh");
        //Mesh[] ms = Resources.FindObjectsOfTypeAll<Mesh>();
        //foreach (Mesh m in ms)
        //{
        //    Debug.Log(m.name);
        //    if (m.name == str)
        //    {
        //        return m;
        //    }
        //}
        //throw new System.Exception("mesh not found: " + str);
        Mesh mesh = meshes.LoadAsset<Mesh>(str);
        if (mesh == null)
            throw new System.Exception("cannot find mesh " + str);
        return mesh;
    }

    public static Mesh TranslateMesh(Mesh mesh, Matrix4x4 matrix)
    {
        Mesh m = new Mesh();
        m.name = mesh.name;
        Vector3[] vec = mesh.vertices;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector3 v = vec[i];
            v = matrix.MultiplyPoint3x4(v);
            vec[i] = v;
        }
        m.vertices = vec;
        m.triangles = mesh.triangles;
        m.uv = mesh.uv;
        m.colors = mesh.colors;
        return m;
    }
}
