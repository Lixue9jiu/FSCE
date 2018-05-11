using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DAEExporter : ScriptableWizard {

    //private string lastExportFolder;

	private void OnWizardCreate()
	{
        string lastPath = EditorPrefs.GetString("lx_OBJExport_lastPath", "");
        string lastFileName = EditorPrefs.GetString("lx_OBJExport_lastFile", "unityexport.dae");
        string expFile = EditorUtility.SaveFilePanel("Export DAE", lastPath, lastFileName, "dae");
        if (expFile.Length > 0)
        {
            var fi = new System.IO.FileInfo(expFile);
            EditorPrefs.SetString("lx_OBJExport_lastFile", fi.Name);
            EditorPrefs.SetString("lx_OBJExport_lastPath", fi.Directory.FullName);
            Export(expFile);
        }
	}

    void Export(string path)
    {
        BlockTerrain terrain = FindObjectOfType<BlockTerrain>();
        int[] chunks = FindObjectOfType<ChunkRenderer>().RenderingChunks;

        List<Mesh> ms = new List<Mesh>();
        List<Matrix4x4> ts = new List<Matrix4x4>();
        for (int i = 0; i < chunks.Length; i++)
        {
            ChunkInstance instance = terrain.chunkInstances[chunks[i]];

            ms.Add(instance.meshes[0]);
            ts.Add(instance.transform);
            ms.Add(instance.meshes[1]);
            ts.Add(instance.transform);
        }

        Export(path, ms.ToArray(), ts.ToArray());
    }

    void Export(string exportPath, Mesh[] meshes, Matrix4x4[] transforms)
    {
        //var exportFileInfo = new System.IO.FileInfo(exportPath);
        //lastExportFolder = exportFileInfo.Directory.FullName;
        EditorUtility.DisplayProgressBar("Exporting OBJ", "Please wait.. Starting export.", 0);

        ColladaExporter exporter = new ColladaExporter(exportPath);

        float maxExportProgress = (float)(meshes.Length + 1);
        for (int i = 0; i < meshes.Length; i++)
        {
            string meshName = meshes[i].name;
            float progress = (float)(i + 1) / maxExportProgress;
            EditorUtility.DisplayProgressBar("Exporting objects... (" + Mathf.Round(progress * 100) + "%)", "Exporting object " + meshName, progress);

            string id = string.Format("mesh_{0}", i);
            exporter.AddGeometry(id, meshes[i]);
            exporter.AddGeometryToScene(id, "Chunk", transforms[i]);
        }

        EditorUtility.ClearProgressBar();
    }

    [MenuItem("File/Export/Collada DAE")]
    static void ExportPly()
    {
        ScriptableWizard.DisplayWizard("Export DAE", typeof(DAEExporter), "Export");
    }
}
