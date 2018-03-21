using UnityEngine;
using System.Collections.Generic;

public class OperationManager : MonoBehaviour
{
    public GameObject operationMenu;

    static Dictionary<string, System.Type> operations = new Dictionary<string, System.Type>()
    {
        {"op_none", null},
        {"op_normal", typeof(NormalBlockOperation)},
        {"op_select", typeof(SelectOperation)}
    };

    public System.Type currentOperation;

    public void SwitchOperation(string str)
    {
        if (currentOperation != null)
        {
            GetOpBehavior(currentOperation).enabled = false;
        }
        if (operations.ContainsKey(str))
        {
            currentOperation = operations[str];
            GetOpBehavior(currentOperation).enabled = true;
        }
        else
        {
            currentOperation = null;
        }
    }

    public MonoBehaviour GetOpBehavior(System.Type type)
    {
        return GetComponent(type) as MonoBehaviour;
    }

	private void Update()
	{
        if (Input.GetKeyUp(KeyCode.E))
        {
            WindowManager.Get<OperationWindow>().TuggleE();
        }
	}
}
