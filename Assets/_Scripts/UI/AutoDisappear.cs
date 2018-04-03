using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasRenderer))]
public class AutoDisappear : MonoBehaviour {

    public float alphaOffset = 0.5f;

    CanvasRenderer canvasRenderer;

    float alpha;

	private void Start()
	{
        canvasRenderer = GetComponent<CanvasRenderer>();
        alpha = 5f;
	}

	private void Update()
	{
        if (alpha <= 0)
        {
            Destroy(gameObject);
            return;
        }
        canvasRenderer.SetAlpha(alpha);
        GetComponentInChildren<CanvasRenderer>().SetAlpha(alpha);
        alpha -= alphaOffset;
	}
}
