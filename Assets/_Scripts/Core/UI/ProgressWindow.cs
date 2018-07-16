using UnityEngine;
using UnityEngine.UI;

public class ProgressWindow : BaseWindow
{
    [SerializeField]
    Slider slider;

    AsyncOperation operation;

    public override void Show()
    {
        if (operation == null)
            return;
        base.Show();
    }

    public void SetOperation(AsyncOperation operation)
    {
        this.operation = operation;
    }

    void Update()
    {
        slider.value = operation.progress;
    }
}
