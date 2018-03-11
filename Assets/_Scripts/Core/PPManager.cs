using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PPManager : MonoBehaviour {

	public PostProcessingBehaviour ppBehaviour;

	void Awake()
	{
		int level = QualitySettings.GetQualityLevel ();
		if (level == 0) {
			ppBehaviour.enabled = false;
		} else {
			ppBehaviour.enabled = true;
		}
	}

}
