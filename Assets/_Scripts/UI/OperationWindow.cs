using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperationWindow : BaseWindow
{

    public GameObject content;
    public RectTransform mainPanel;

	private void Start()
    {
        foreach (Button b in this.GetComponentsInChildren<Button>())
        {
            b.onClick.AddListener(() => { OnButtonClicked(b.name); });
        }

        GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
        float width = content.transform.childCount * grid.cellSize.x
                             + content.transform.childCount * grid.spacing.x
                             + grid.padding.left + grid.padding.right;
        mainPanel.sizeDelta = new Vector2(width, mainPanel.sizeDelta.y);
    }

	private void Update()
	{
        if (Input.GetKeyDown(KeyCode.E))
        {
            Hide();
        }
	}

	public void OnButtonClicked(string name)
    {
        switch (name)
        {
            case "op_none":
                OperationManager.instance.SwitchOperation(null);
                break;
            case "op_normal":
                OperationManager.instance.SwitchOperation<NormalBlockOperation>();
                break;
            case "op_select":
                OperationManager.instance.SwitchOperation<SelectOperation>();
                break;
        }

        Hide();
    }
}
