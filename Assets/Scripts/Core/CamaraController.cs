using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraController : MonoBehaviour
{

	public float WalkSpeed = 1;
	public float RotationSensitivity = 2;

	bool isCursorLocked;

	void Start ()
	{
		SetCursorLocked (true);
	}

	void Update ()
	{
		if (isCursorLocked) {
			float rotationH = Input.GetAxis ("Mouse X");
			float rotationV = Input.GetAxis ("Mouse Y");

			transform.Rotate (new Vector3 (0, rotationH, 0) * RotationSensitivity, Space.World);
			transform.Rotate (new Vector3 (-rotationV, 0, 0) * RotationSensitivity);

			float forwardSpeed = Input.GetAxis ("Vertical");
			float sideWaySpeed = Input.GetAxis ("Horizontal");

			Vector3 speed = new Vector3 (sideWaySpeed, 0, forwardSpeed) * WalkSpeed;

			transform.Translate (speed);

			float elevation = Input.GetAxis ("Elevation");

			transform.Translate (new Vector3 (0, elevation, 0) * WalkSpeed, Space.World);
		}

		if (Input.GetKeyUp (KeyCode.Escape)) {
			SetCursorLocked (!isCursorLocked);
		}
	}

	public void SetCursorLocked(bool isLocked)
	{
		isCursorLocked = isLocked;
		if (isLocked) {
			Cursor.lockState = CursorLockMode.Locked;
		} else {
			Cursor.lockState = CursorLockMode.None;
		}
		Cursor.visible = !isLocked;
	}
}
