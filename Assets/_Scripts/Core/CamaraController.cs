using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CamaraController : MonoBehaviour
{

	public PauseMenu pauseMenu;

	public float WalkSpeed = 1;
	public float RotationSensitivity = 2;

	public static bool isCursorLocked;

	void Start ()
	{
//		SetCursorLocked (true);
	}

	void Update ()
	{
		if (isCursorLocked) {
			float rotationH = CrossPlatformInputManager.GetAxis ("Mouse X");
			float rotationV = CrossPlatformInputManager.GetAxis ("Mouse Y");

			transform.Rotate (new Vector3 (0, rotationH, 0) * RotationSensitivity, Space.World);
			transform.Rotate (new Vector3 (-rotationV, 0, 0) * RotationSensitivity);

			float forwardSpeed = CrossPlatformInputManager.GetAxis ("Vertical");
			float sideWaySpeed = CrossPlatformInputManager.GetAxis ("Horizontal");

			Vector3 speed = new Vector3 (sideWaySpeed, 0, forwardSpeed) * WalkSpeed;

			transform.Translate (speed);

			float elevation = CrossPlatformInputManager.GetAxis ("Elevation");

			transform.Translate (new Vector3 (0, elevation, 0) * WalkSpeed, Space.World);
		}

		if (Input.GetKeyUp (KeyCode.Escape)) {
			SetCursorLocked (false);
			pauseMenu.Pause ();
		} else if (Input.GetKeyUp (KeyCode.P)) {
			ScreenshotManager.Screenshot ();
		}
	}

	public static void SetCursorLocked (bool isLocked)
	{
		isCursorLocked = isLocked;

		#if !MOBILE_INPUT
		if (isLocked) {
			Cursor.lockState = CursorLockMode.Locked;
		} else {
			Cursor.lockState = CursorLockMode.None;
		}
		Cursor.visible = !isLocked;
		#endif
	}
}
