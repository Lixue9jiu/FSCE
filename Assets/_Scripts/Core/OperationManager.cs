using UnityEngine;
using System.Collections.Generic;

public class OperationManager : MonoBehaviour
{
    static OperationManager main;

    public static OperationManager instance
    {
        get
        {
            if (main == null)
                main = FindObjectOfType<OperationManager>();
            return main;
        }
    }

    public Operation currentOperation = null;

    private void Start()
    {
        //WindowManager.Get<ConsoleWindow>().AssignCommand("operation", delegate (string[] args) {
        //    WindowManager.Get<OperationWindow>().OnButtonClicked(args[0]);
        //});
    }

    public void SwitchOperation<T>()
    {
        SwitchOperation(typeof(T));
    }

    public void SwitchOperation(System.Type type)
    {
        if (type.IsInstanceOfType(currentOperation))
            return;

        if (currentOperation != null)
            currentOperation.OnRemoveFromCurrent();

        currentOperation = GetOpBehavior(type);
        currentOperation.OnSetToCurrent();
    }

    public Operation GetOpBehavior(System.Type type)
    {
        if (type == null)
            return null;
        Component component = GetComponent(type);
        if (component != null)
        {
            return component as Operation;
        }
        return gameObject.AddComponent(type) as Operation;
    }

    public void SetCurrentOpEnabled(bool active)
    {
        if (currentOperation != null)
            currentOperation.enabled = active;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            WindowManager.Get<OperationWindow>().Show();
        }
    }
}
