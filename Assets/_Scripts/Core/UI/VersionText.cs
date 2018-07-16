using UnityEngine;
using UnityEngine.UI;

public class VersionText : MonoBehaviour
{
    void Start()
    {
        GetComponent<Text>().text = string.Format("Fancy SurvivclCraft Editor {0}", Application.version);
    }
}
