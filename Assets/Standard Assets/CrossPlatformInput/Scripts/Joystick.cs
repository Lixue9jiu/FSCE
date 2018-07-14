using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public int MovementRange = 100;
        public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
        public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input
        public float sensitivity = 1f;

        public RectTransform Stick;

        bool isHolding;

        int m_movementRange;
        Vector3 m_StartPos;
        CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
        CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input

        void OnEnable()
        {
            CreateVirtualAxes();
            m_StartPos = transform.position;
            m_movementRange = (int)(transform.parent.GetComponent<RectTransform>().localScale.x * MovementRange);
            Debug.Log(m_movementRange);
        }

        void UpdateVirtualAxes(Vector3 value)
        {
            var delta = m_StartPos - value;
            delta.y = -delta.y;
            delta /= m_movementRange;
            m_HorizontalVirtualAxis.Update(-delta.x * sensitivity);
            m_VerticalVirtualAxis.Update(delta.y * sensitivity);
        }

        void CreateVirtualAxes()
        {
            m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);

            m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
        }

        public void OnDrag(PointerEventData data)
        {
            Vector3 newPos = Vector3.zero;
            newPos.x = (data.position.x - m_StartPos.x);
            newPos.y = (data.position.y - m_StartPos.y);

            float m = newPos.magnitude;
            if (m > m_movementRange)
            {
                newPos *= m_movementRange / m;
            }

            Stick.position = new Vector3(m_StartPos.x + newPos.x, m_StartPos.y + newPos.y, m_StartPos.z + newPos.z);
            UpdateVirtualAxes(Stick.position);
        }

        public void OnPointerUp(PointerEventData data)
        {
            isHolding = false;
        }

        public void OnPointerDown(PointerEventData data)
        {
            isHolding = true;
            OnDrag(data);
        }

        void Update()
        {
            if (!isHolding && Stick.position != m_StartPos)
            {
                Stick.position = Vector3.LerpUnclamped(Stick.position, m_StartPos, 0.1f);
                UpdateVirtualAxes(Stick.position);
            }
        }

        void OnDisable()
        {
            // remove the joysticks from the cross platform input
            m_HorizontalVirtualAxis.Remove();
            m_VerticalVirtualAxis.Remove();
        }
    }
}