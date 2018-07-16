using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class ListPanelItem : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    Color unselectedColorTint = new Color(1f, 1f, 1f, 0.5f);

    [SerializeField]
    Color selectedColorTint = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    [SerializeField]
    Image image;

    ListPanel listPanel;

    void Awake()
    {
        listPanel = GetComponentInParent<ListPanel>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.color = selectedColorTint;
        listPanel.OnItemSelected(this);
    }

    public void Deselect()
    {
        image.color = unselectedColorTint;
    }
}
