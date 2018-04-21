using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class AutoDisappear : MonoBehaviour {

    public float alphaOffset = 0.5f;

    CanvasRenderer canvasRenderer;

    public CanvasRenderer childText;

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
        childText.SetAlpha(alpha);
        alpha -= alphaOffset;
	}
}
