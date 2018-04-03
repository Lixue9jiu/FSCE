using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PPManager : MonoBehaviour
{
    public PostProcessingBehaviour ppBehaviour;

    void Start()
    {
        UpdatePPBehavior();
    }

    public void SetQuality(int level)
    {
        QualitySettings.SetQualityLevel(level);
        UpdatePPBehavior();
    }

    void UpdatePPBehavior()
    {
        if (QualitySettings.GetQualityLevel() == 0)
        {
            ppBehaviour.enabled = false;
        }
        else
        {
            ppBehaviour.enabled = true;
        }
    }
}
