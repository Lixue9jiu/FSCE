using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWindow : MonoBehaviour {

	public bool isShowing { get; protected set; }

    public virtual void Show()
    {
        if (!WindowManager.SetAsActiveWindow(this))
            return;
        
        gameObject.SetActive(true);
        PauseManager.SetAllActive(false);

		isShowing = true;
    }

    public virtual void Hide()
    {
		if (!isShowing)
			return;
        
        gameObject.SetActive(false);
        PauseManager.SetAllActive(true);

		isShowing = false;
        Destroy(gameObject);
    }
}
