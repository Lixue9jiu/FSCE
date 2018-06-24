using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class TouchPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        const float MIN_TOUCH_RANGE_SQR = 4f;
        const float LONG_CLICK_WAIT_TIME = 0.5f;
        const float SHORT_CLICK_WAIT_TIME = 0.2f;

        public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
        public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input
        public float sensitivity = 1f;

        Vector3 m_StartPos;
        Vector2 m_PreviousDelta;
        Vector3 m_JoytickOutput;
        CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
        CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input
        bool m_Dragging;
        int m_Id = -1;
        Vector2 m_PreviousTouchPos; // swipe style control touch

        public string shortClickAxisName = "Destroy";
        public string longClickAxisName = "Place";

        bool m_CheckForTouch;
        bool isShortClick;
        float m_DragTimeWaited;
        Vector2 m_AccumulatedPointerDelta;
        CrossPlatformInputManager.VirtualAxis m_ShortClickAxis;
        CrossPlatformInputManager.VirtualAxis m_LongClickAxis;

        void OnEnable()
        {
            CreateVirtualAxes();
        }

        void CreateVirtualAxes()
        {
            m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);

            m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);

            m_ShortClickAxis = new CrossPlatformInputManager.VirtualAxis(shortClickAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(m_ShortClickAxis);

            m_LongClickAxis = new CrossPlatformInputManager.VirtualAxis(longClickAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(m_LongClickAxis);
        }

        void UpdateVirtualAxes(Vector3 value)
        {
            m_HorizontalVirtualAxis.Update(value.x);
            m_VerticalVirtualAxis.Update(value.y);
        }

        public void OnPointerDown(PointerEventData data)
        {
            m_Dragging = true;
            m_Id = data.pointerId;
#if !UNITY_EDITOR
            m_PreviousTouchPos = Input.touches[m_Id].position;
#else
            m_PreviousTouchPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#endif
            m_CheckForTouch = true;
            m_DragTimeWaited = 0;
            m_AccumulatedPointerDelta = Vector2.zero;
        }

        void Update()
        {
            if (!m_Dragging)
            {
                m_PreviousTouchPos = Vector2.zero;
                return;
            }
            if (Input.touchCount >= m_Id + 1 && m_Id != -1)
            {
#if !UNITY_EDITOR
                Vector2 pointerDelta = new Vector2(Input.touches[m_Id].position.x - m_PreviousTouchPos.x , Input.touches[m_Id].position.y - m_PreviousTouchPos.y);
                pointerDelta.x *= sensitivity;
                pointerDelta.y *= sensitivity;
                m_PreviousTouchPos = Input.touches[m_Id].position;
#else
                Vector2 pointerDelta;
                pointerDelta.x = Input.mousePosition.x - m_PreviousTouchPos.x;
                pointerDelta.y = Input.mousePosition.y - m_PreviousTouchPos.y;

                pointerDelta.x *= sensitivity;
                pointerDelta.y *= sensitivity;
                m_PreviousTouchPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#endif
                if (m_CheckForTouch)
                {
                    m_DragTimeWaited += Time.deltaTime;
                    m_AccumulatedPointerDelta += pointerDelta;
                    if (m_DragTimeWaited > LONG_CLICK_WAIT_TIME)
                    {
                        m_CheckForTouch = false;
                        if (m_AccumulatedPointerDelta.sqrMagnitude < MIN_TOUCH_RANGE_SQR)
                        {
                            m_LongClickAxis.Update(1);
                        }
                    }
                }

                UpdateVirtualAxes(new Vector3(pointerDelta.x, pointerDelta.y, 0));
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            m_Dragging = false;
            m_Id = -1;
            UpdateVirtualAxes(Vector3.zero);
            m_LongClickAxis.Update(0);
            if (m_DragTimeWaited < SHORT_CLICK_WAIT_TIME && m_AccumulatedPointerDelta.sqrMagnitude < MIN_TOUCH_RANGE_SQR)
            {
                StartCoroutine(TuggleShortClick());
            }
        }

        IEnumerator TuggleShortClick()
        {
            m_ShortClickAxis.Update(1);
            yield return new WaitForFixedUpdate();
            m_ShortClickAxis.Update(0);
        }

        void OnDisable()
        {
            if (CrossPlatformInputManager.AxisExists(horizontalAxisName))
                CrossPlatformInputManager.UnRegisterVirtualAxis(horizontalAxisName);

            if (CrossPlatformInputManager.AxisExists(verticalAxisName))
                CrossPlatformInputManager.UnRegisterVirtualAxis(verticalAxisName);

            if (CrossPlatformInputManager.AxisExists(shortClickAxisName))
                CrossPlatformInputManager.UnRegisterVirtualAxis(shortClickAxisName);

            if (CrossPlatformInputManager.AxisExists(longClickAxisName))
                CrossPlatformInputManager.UnRegisterVirtualAxis(longClickAxisName);
        }
    }
}