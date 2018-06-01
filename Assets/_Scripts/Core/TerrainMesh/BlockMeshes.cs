using UnityEngine;

//using UnityEditor;

public class BlockMeshes : MonoBehaviour
{
    static AssetBundle meshes;

    static Vector3 half = new Vector3(0.5f, 0.5f, 0.5f);

	public void Initialize()
	{
		if (meshes == null)
            meshes = AssetBundle.LoadFromFile(System.IO.Path.Combine(Application.streamingAssetsPath, "meshes"));
	}

	private void OnDestroy()
	{
        meshes.Unload(true);
	}

	public static Mesh TranslateMesh(Mesh mesh, Matrix4x4 matrix, bool upsidedown)
    {
        Mesh m = new Mesh();
        Vector3[] vec = mesh.vertices;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector3 v = vec[i];
            v -= half;
            v = matrix.MultiplyPoint3x4(v);
            v += half;
            if (upsidedown)
            {
                v.y = 1 - v.y;
            }
            vec[i] = v;
        }
        int[] triangles = mesh.triangles;
        if (upsidedown)
        {
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int tmp = triangles[i];
                triangles[i] = triangles[i + 1];
                triangles[i + 1] = tmp;
            }
        }
        m.vertices = vec;
        m.triangles = triangles;
        m.uv = mesh.uv;
        return m;
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

    public static Mesh TranslateMeshRaw(Mesh mesh, Matrix4x4 matrix)
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

    public static Mesh UpsideDownMesh(Mesh mesh)
    {
        Mesh m = new Mesh();
        Vector3[] vec = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector3 v = vec[i];
            v.y = 1 - v.y;
            vec[i] = v;
        }
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int tmp = triangles[i];
            triangles[i] = triangles[i + 1];
            triangles[i + 1] = tmp;
        }

        m.vertices = vec;
        m.triangles = triangles;
        m.uv = mesh.uv;
        return m;
    }
}
