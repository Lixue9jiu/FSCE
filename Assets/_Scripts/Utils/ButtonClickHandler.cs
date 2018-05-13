using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonClickHandler : MonoBehaviour, IPointerUpHandler
{
	public UnityEngine.Events.UnityEvent PointerUpHandler;

	public void OnPointerUp(PointerEventData eventData)
	{
		PointerUpHandler.Invoke();
	}
}
