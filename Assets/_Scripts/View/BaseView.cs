using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSCE.View
{
    [RequireComponent(typeof(Canvas))]
    public class BaseView : MonoBehaviour
    {
        Canvas canvas;

		private void Start()
		{
            canvas = GetComponent<Canvas>();
		}

	}
}