using UnityEngine;

//using UnityEditor;

public class BlockMeshes : MonoBehaviour
{
    static AssetBundle meshes;

    static Vector3 half = new Vector3(0.5f, 0.5f, 0.5f);

    private void Awake()
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
            v = matrix.MultiplyPoint(v);
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
        return meshes.LoadAsset<Mesh>(str);
    }

    //   public GameObject gates;

    //private void Start()
    //{
    //       TransformAllMesh(gates);
    //}

    //public static void TransformAllMesh(GameObject obj)
    //{
    //    MeshFilter[] ms = obj.GetComponentsInChildren<MeshFilter>();
    //    foreach (MeshFilter m in ms)
    //    {
    //        Mesh meshToSave = TranslateMeshRaw(m.sharedMesh, m.gameObject.transform.localToWorldMatrix);

    //        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/Meshes/", meshToSave.name, "asset");
    //        if (string.IsNullOrEmpty(path)) continue;

    //        path = FileUtil.GetProjectRelativePath(path);

    //        MeshUtility.Optimize(meshToSave);

    //        AssetDatabase.CreateAsset(meshToSave, path);
    //        AssetDatabase.SaveAssets();
    //    }
    //}

    public static Mesh TranslateMeshRaw(Mesh mesh, Matrix4x4 matrix)
    {
        Mesh m = new Mesh();
        m.name = mesh.name;
        Vector3[] vec = mesh.vertices;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector3 v = vec[i];
            v -= half;
            v = matrix.MultiplyPoint(v);
            v += half;
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
