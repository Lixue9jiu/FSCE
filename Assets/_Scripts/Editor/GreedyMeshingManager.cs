using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GreedyMeshingManager
{
    const string greedyMeshing = "GREEDY_MESHING";

    [MenuItem("Greedy Meshing/Enable")]
    private static void Enable()
    {
        SetEnabled(greedyMeshing, true);
    }


    [MenuItem("Greedy Meshing/Enable", true)]
    private static bool EnableValidate()
    {
        var defines = GetDefinesList();
        return !defines.Contains(greedyMeshing);
    }


    [MenuItem("Greedy Meshing/Disable")]
    private static void Disable()
    {
        SetEnabled(greedyMeshing, false);
    }


    [MenuItem("Greedy Meshing/Disable", true)]
    private static bool DisableValidate()
    {
        var defines = GetDefinesList();
        return defines.Contains(greedyMeshing);
    }

    private static void SetEnabled(string defineName, bool enable)
    {
        var group = EditorUserBuildSettings.selectedBuildTargetGroup;
        var defines = GetDefinesList();
        if (enable)
        {
            if (defines.Contains(defineName))
            {
                return;
            }
            defines.Add(defineName);
        }
        else
        {
            if (!defines.Contains(defineName))
            {
                return;
            }
            while (defines.Contains(defineName))
            {
                defines.Remove(defineName);
            }
        }
        string definesString = string.Join(";", defines.ToArray());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, definesString);
    }


    private static List<string> GetDefinesList()
    {
        return new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';'));
    }
}
