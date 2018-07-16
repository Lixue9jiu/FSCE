using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextBoxWindow : BaseWindow
{
    public InputField Input;
    public Text Text;

    public UnityAction<string> OnComfirmed;

    public static void ShowTextBox(string title, UnityAction<string> handler)
    {
        var window = WindowManager.Get<TextBoxWindow>();
        window.Text.text = title;
        window.OnComfirmed = handler;
        window.Show();
    }

    public void OnConfirmButtonClicked()
    {
        Hide();
        OnComfirmed.Invoke(Input.text);
    }

    public void OnCancelButtonClicked()
    {
        Hide();
    }
}
