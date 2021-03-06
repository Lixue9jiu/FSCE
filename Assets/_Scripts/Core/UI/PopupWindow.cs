﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PopupWindow : BaseWindow
{

    public Text text;
    public Button yesButton;
    public Button noButton;
    public Button cancelButton;
    public GameObject panel;

    static PopupWindow main;

    public static PopupWindow Main
    {
        get
        {
            if (main == null)
            {
                main = FindObjectOfType<PopupWindow>();
                if (main == null)
                    Debug.LogError("no popup window found");
            }
            return main;
        }
    }

    public void ShowThreeChoices(string question, UnityAction yes, UnityAction no, UnityAction cancel)
    {
        panel.SetActive(true);

        text.text = question;

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(yes);
        yesButton.onClick.AddListener(HidePanel);

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(no);
        noButton.onClick.AddListener(HidePanel);

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(cancel);
        cancelButton.onClick.AddListener(HidePanel);
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }
}
