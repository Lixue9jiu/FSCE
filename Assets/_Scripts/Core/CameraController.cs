using UnityEngine;
using UnityEngine.EventSystems;

using InputManager = UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager;

public class CameraController : MonoBehaviour
{

    public float verticalRotation = 0;

    public float WalkSpeed = 1;
    public float RotationSensitivity = 2;

    public static bool isCursorLocked;

    //    private void Awake()
    //    {
    //#if MOBILE_INPUT

    //#endif
    //}

    void Update()
    {
        float rotationH = InputManager.GetAxis("Mouse X");
        float rotationV = InputManager.GetAxis("Mouse Y");

        rotationV = Mathf.Clamp(-rotationV * RotationSensitivity, -80 - verticalRotation, 80 - verticalRotation);
        verticalRotation += rotationV;

        Camera.main.transform.Rotate(new Vector3(0, rotationH * RotationSensitivity, 0), Space.World);
        Camera.main.transform.Rotate(new Vector3(rotationV, 0, 0));

        float forwardSpeed = InputManager.GetAxis("Vertical");
        float sideWaySpeed = InputManager.GetAxis("Horizontal");

        Vector3 speed = new Vector3(sideWaySpeed, 0, forwardSpeed) * WalkSpeed;

        Camera.main.transform.Translate(speed);

        float elevation = InputManager.GetAxis("Elevation");

        Camera.main.transform.Translate(new Vector3(0, elevation, 0) * WalkSpeed, Space.World);
    }

    public static void SetCursorLocked(bool isLocked)
    {
        isCursorLocked = isLocked;

#if !MOBILE_INPUT
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        Cursor.visible = !isLocked;
#endif
    }


}

//public class MobileMouseInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
//{
//   const string horizontalInputName = "Mouse X";
//   const string virticalInputName = "Mouse Y";

//   bool isDragging;
//   int pointerIndex = -1;

//   InputManager.VirtualAxis horizontalInput;
//   InputManager.VirtualAxis virticalInput;

//private void OnEnable()
//{
//       horizontalInput = new InputManager.VirtualAxis(horizontalInputName);
//       virticalInput = new InputManager.VirtualAxis(virticalInputName);
//}

//public void OnPointerDown(PointerEventData eventData)
//   {
//       isDragging = true;
//       pointerIndex = eventData.pointerId;
//   }

//   public void OnPointerUp(PointerEventData eventData)
//   {
//       isDragging = false;
//       pointerIndex = -1;
//   }

//private void Update()
//{
//	if (isDragging) {

//	}
//}
//}