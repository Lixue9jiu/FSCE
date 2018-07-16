using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ListPanel : MonoBehaviour
{
    [SerializeField]
    GameObject content;

    [SerializeField]
    ListPanelItem template;

    [SerializeField]
    public ListPanelEvent onItemSelected;

    int selectedItem = -1;

    List<ListPanelItem> items = new List<ListPanelItem>();

    public int SelectedItem
    {
        get
        {
            return selectedItem;
        }
    }

    public GameObject AddItem()
    {
        var item = Instantiate(template, content.transform);
        item.gameObject.SetActive(true);
        items.Add(item);
        return item.gameObject;
    }

    public void ClearItems()
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
        content.transform.DetachChildren();
        items.Clear();
        selectedItem = -1;
    }

    public void OnItemSelected(ListPanelItem item)
    {
        var newSelection = items.IndexOf(item);
        if (newSelection == selectedItem)
            return;
        if (selectedItem != -1)
            items[selectedItem].Deselect();
        selectedItem = newSelection;
        onItemSelected.Invoke(selectedItem);
    }

    [Serializable]
    public class ListPanelEvent : UnityEvent<int>
    {
    }
}
