using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class TouchControlSet : MonoBehaviour {

    public CameraController cameraController;

    private void Start()
    {
        GetComponentInChildren<TouchPad>().sensitivity = cameraController.RotationSensitivity * 0.03333333333f;
        GetComponentInChildren<Joystick>().sensitivity = cameraController.WalkSpeed * 0.4f;
    }

}
