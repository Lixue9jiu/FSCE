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
        GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
        float width = content.transform.childCount * grid.cellSize.x
                             + content.transform.childCount * grid.spacing.x
                             + grid.padding.left + grid.padding.right;
        mainPanel.sizeDelta = new Vector2(width, mainPanel.sizeDelta.y);
    }

    public void TuggleE()
    {
      if (isShowing)
      {
        Hide();
      }
      else {
        Show();
      }
    }
}
