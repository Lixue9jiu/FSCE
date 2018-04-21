﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using System.Collections.Generic;

/*=============================================================================
 |	    Project:  Unity3D Scene OBJ Exporter
 |
 |		  Notes: Only works with meshes + meshRenderers. No terrain yet
 |
 |       Author:  aaro4130
 |
 |     DO NOT USE PARTS OF THIS CODE, OR THIS CODE AS A WHOLE AND CLAIM IT
 |     AS YOUR OWN WORK. USE OF CODE IS ALLOWED IF I (aaro4130) AM CREDITED
 |     FOR THE USED PARTS OF THE CODE.
 |
 *===========================================================================*/

public class OBJExporter : ScriptableWizard
{
    public bool onlySelectedObjects = false;
    public bool generateMaterials = true;
    public bool exportTextures = true;
    public bool splitObjects = true;
    public bool autoMarkTexReadable = false;
    public bool objNameAddIdNum = false;

    //public bool materialsUseTextureName = false;

    private string versionString = "v2.0";
    private string lastExportFolder;

    bool StaticBatchingEnabled()
    {
        PlayerSettings[] playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
        if (playerSettings == null)
        {
            return false;
        }
        SerializedObject playerSettingsSerializedObject = new SerializedObject(playerSettings);
        SerializedProperty batchingSettings = playerSettingsSerializedObject.FindProperty("m_BuildTargetBatching");
        for (int i = 0; i < batchingSettings.arraySize; i++)
        {
            SerializedProperty batchingArrayValue = batchingSettings.GetArrayElementAtIndex(i);
            if (batchingArrayValue == null)
            {
                continue;
            }
            IEnumerator batchingEnumerator = batchingArrayValue.GetEnumerator();
            if (batchingEnumerator == null)
            {
                continue;
            }
            while (batchingEnumerator.MoveNext())
            {
                SerializedProperty property = (SerializedProperty)batchingEnumerator.Current;
                if (property != null && property.name == "m_StaticBatching")
                {
                    return property.boolValue;
                }
            }
        }
        return false;
    }

    void OnWizardUpdate()
    {
        helpString = "Aaro4130's OBJ Exporter " + versionString;
    }

    Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
    {
        return angle * (point - pivot) + pivot;
    }
    Vector3 MultiplyVec3s(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

    void OnWizardCreate()
    {
        if(StaticBatchingEnabled() && Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Error", "Static batching is enabled. This will cause the export file to look like a mess, as well as be a large filesize. Disable this option, and restart the player, before continuing.", "OK");
            goto end;
        }
        if (autoMarkTexReadable)
        {
            int yes = EditorUtility.DisplayDialogComplex("Warning", "This will convert all textures to Advanced type with the read/write option set. This is not reversible and will permanently affect your project. Continue?", "Yes", "No", "Cancel");
            if(yes > 0)
            {
                goto end;
            }
        }
        string lastPath = EditorPrefs.GetString("a4_OBJExport_lastPath", "");
        string lastFileName = EditorPrefs.GetString("a4_OBJExport_lastFile", "unityexport.obj");
        string expFile = EditorUtility.SaveFilePanel("Export OBJ", lastPath, lastFileName, "obj");
        if (expFile.Length > 0)
        {
            var fi = new System.IO.FileInfo(expFile);
            EditorPrefs.SetString("a4_OBJExport_lastFile", fi.Name);
            EditorPrefs.SetString("a4_OBJExport_lastPath", fi.Directory.FullName);
            Export(expFile);
        }
        end:;
    }

    void Export(string exportPath)
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

        Export(exportPath, ms.ToArray(), ts.ToArray());
    }

    void Export(string exportPath, Mesh[] meshes, Matrix4x4[] transforms)
    {
        //init stuff
        //Dictionary<string, bool> materialCache = new Dictionary<string, bool>();
        var exportFileInfo = new System.IO.FileInfo(exportPath);
        lastExportFolder = exportFileInfo.Directory.FullName;
        //string baseFileName = System.IO.Path.GetFileNameWithoutExtension(exportPath);
        EditorUtility.DisplayProgressBar("Exporting OBJ", "Please wait.. Starting export.", 0);

        //get list of required export things
        //MeshFilter[] sceneMeshes;
        //if (onlySelectedObjects)
        //{
        //    List<MeshFilter> tempMFList = new List<MeshFilter>();
        //    foreach (GameObject g in Selection.gameObjects)
        //    {

        //        MeshFilter f = g.GetComponent<MeshFilter>();
        //        if (f != null)
        //        {
        //            tempMFList.Add(f);
        //        }

        //    }
        //    sceneMeshes = tempMFList.ToArray();
        //}
        //else
        //{
        //    sceneMeshes = FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];

        //}

        //if (Application.isPlaying)
        //{
        //    foreach (MeshFilter mf in sceneMeshes)
        //    {
        //        MeshRenderer mr = mf.gameObject.GetComponent<MeshRenderer>();
        //        if (mr != null)
        //        {
        //            if (mr.isPartOfStaticBatch)
        //            {
        //                EditorUtility.ClearProgressBar();
        //                EditorUtility.DisplayDialog("Error", "Static batched object detected. Static batching is not compatible with this exporter. Please disable it before starting the player.", "OK");
        //                return;
        //            }
        //        }
        //    }
        //}
        
        //work on export
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("# Export of " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        sb.AppendLine("# from Aaro4130 OBJ Exporter " + versionString);
        //if (generateMaterials)
        //{
        //    sb.AppendLine("mtllib " + baseFileName + ".mtl");
        //}
        float maxExportProgress = (float)(meshes.Length + 1);
        int lastIndex = 0;
        for(int i = 0; i < meshes.Length; i++)
        {
            string meshName = meshes[i].name;
            float progress = (float)(i + 1) / maxExportProgress;
            EditorUtility.DisplayProgressBar("Exporting objects... (" + Mathf.Round(progress * 100) + "%)", "Exporting object " + meshName, progress);

            if (splitObjects)
            {
                string exportName = meshName;
                if (objNameAddIdNum)
                {
                    exportName += "_" + i;
                }
                sb.AppendLine("g " + exportName);
            }
            //if(mr != null && generateMaterials)
            //{
            //    Material[] mats = mr.sharedMaterials;
            //    for(int j=0; j < mats.Length; j++)
            //    {
            //        Material m = mats[j];
            //        if (!materialCache.ContainsKey(m.name))
            //        {
            //            materialCache[m.name] = true;
            //            sbMaterials.Append(MaterialToString(m));
            //            sbMaterials.AppendLine();
            //        }
            //    }
            //}

            //export the meshhh :3
            Mesh msh = meshes[i];
            Matrix4x4 trans = transforms[i];
            int faceOrder = (int)Mathf.Clamp((trans.lossyScale.x * trans.lossyScale.z), -1, 1);
            
            //export vector data (FUN :D)!
            foreach (Vector3 vx in msh.vertices)
            {
                Vector3 v = vx;
                v = trans.MultiplyPoint3x4(v);
                v.x *= -1;
                sb.AppendLine("v " + v.x + " " + v.y + " " + v.z);
            }
            foreach (Vector3 vx in msh.normals)
            {
                Vector3 v = vx;

                v = MultiplyVec3s(v, trans.lossyScale.normalized);
                v = RotateAroundPoint(v, Vector3.zero, trans.rotation);

                v.x *= -1;
                sb.AppendLine("vn " + v.x + " " + v.y + " " + v.z);

            }
            foreach (Vector2 v in msh.uv)
            {
                sb.AppendLine("vt " + v.x + " " + v.y);
            }

            for (int j=0; j < msh.subMeshCount; j++)
            {
                //if(mr != null && j < mr.sharedMaterials.Length)
                //{
                //    string matName = mr.sharedMaterials[j].name;
                //    sb.AppendLine("usemtl " + matName);
                //}
                //else
                //{
                //    sb.AppendLine("usemtl " + meshName + "_sm" + j);
                //}

                int[] tris = msh.GetTriangles(j);
                for(int t = 0; t < tris.Length; t+= 3)
                {
                    int idx2 = tris[t] + 1 + lastIndex;
                    int idx1 = tris[t + 1] + 1 + lastIndex;
                    int idx0 = tris[t + 2] + 1 + lastIndex;
                    if(faceOrder < 0)
                    {
                        sb.AppendLine("f " + ConstructOBJString(idx2) + " " + ConstructOBJString(idx1) + " " + ConstructOBJString(idx0));
                    }
                    else
                    {
                        sb.AppendLine("f " + ConstructOBJString(idx0) + " " + ConstructOBJString(idx1) + " " + ConstructOBJString(idx2));
                    }
                    
                }
            }

            lastIndex += msh.vertices.Length;
        }

        //write to disk
        System.IO.File.WriteAllText(exportPath, sb.ToString());
        //if (generateMaterials)
        //{
        //    System.IO.File.WriteAllText(exportFileInfo.Directory.FullName + "\\" + baseFileName + ".mtl", sbMaterials.ToString());
        //}

        //export complete, close progress dialog
        EditorUtility.ClearProgressBar();
    }

    string TryExportTexture(string propertyName,Material m)
    {
        if (m.HasProperty(propertyName))
        {
            Texture t = m.GetTexture(propertyName);
            if(t != null)
            {
                return ExportTexture((Texture2D)t);
            }
        }
        return "false";
    }
    string ExportTexture(Texture2D t)
    {
        try
        {
            if (autoMarkTexReadable)
            {
                string assetPath = AssetDatabase.GetAssetPath(t);
                var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (tImporter != null)
                {
                    tImporter.textureType = TextureImporterType.Default;

                    if (!tImporter.isReadable)
                    {
                        tImporter.isReadable = true;

                        AssetDatabase.ImportAsset(assetPath);
                        AssetDatabase.Refresh();
                    }
                }
            }
            string exportName = lastExportFolder + "\\" + t.name + ".png";
            Texture2D exTexture = new Texture2D(t.width, t.height, TextureFormat.ARGB32, false);
            exTexture.SetPixels(t.GetPixels());
            System.IO.File.WriteAllBytes(exportName, exTexture.EncodeToPNG());
            return exportName;
        }
        catch (System.Exception)
        {
            Debug.Log("Could not export texture : " + t.name + ". is it readable?");
            return "null";
        }

    }

    private string ConstructOBJString(int index)
    {
        string idxString = index.ToString();
        return idxString + "/" + idxString + "/" + idxString;
    }
    string MaterialToString(Material m)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("newmtl " + m.name);


        //add properties
        if (m.HasProperty("_Color"))
        {
            sb.AppendLine("Kd " + m.color.r.ToString() + " " + m.color.g.ToString() + " " + m.color.b.ToString());
            if (m.color.a < 1.0f)
            {
                //use both implementations of OBJ transparency
                sb.AppendLine("Tr " + (1f - m.color.a).ToString());
                sb.AppendLine("d " + m.color.a.ToString());
            }
        }
        if (m.HasProperty("_SpecColor"))
        {
            Color sc = m.GetColor("_SpecColor");
            sb.AppendLine("Ks " + sc.r.ToString() + " " + sc.g.ToString() + " " + sc.b.ToString());
        }
        if (exportTextures) {
            //diffuse
            string exResult = TryExportTexture("_MainTex", m);
            if (exResult != "false")
            {
                sb.AppendLine("map_Kd " + exResult);
            }
            //spec map
            exResult = TryExportTexture("_SpecMap", m);
            if (exResult != "false")
            {
                sb.AppendLine("map_Ks " + exResult);
            }
            //bump map
            exResult = TryExportTexture("_BumpMap", m);
            if (exResult != "false")
            {
                sb.AppendLine("map_Bump " + exResult);
            }

    }
        sb.AppendLine("illum 2");
        return sb.ToString();
    }

    [MenuItem("File/Export/Wavefront OBJ")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Export OBJ", typeof(OBJExporter), "Export");
    }
}