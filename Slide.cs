using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Slide : MonoBehaviour
{
    bool touching;
    InputAction swipeAction = null;
    public float treshold;

    public void Touch(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            if(swipeAction == null)
            {
                swipeAction = context.action.actionMap.FindAction("Swipe");
            }
            touching = true;
            Debug.Log("Touch started");
        }
        else if(context.canceled)
        {
            touching = false;
            Debug.Log("Touch ended");
        }

    }



    private void Update()
    {
        if(touching)
        {
            if(swipeAction.inProgress)
            {
                float swipeValue = swipeAction.ReadValue<Vector2>().x;
                Debug.Log(swipeValue);

                if(swipeValue < -treshold)
                {
                    Ruudukko.Instance.Slide(false);
                    touching = false;
                }
                else if(swipeValue > treshold)
                {
                    Ruudukko.Instance.Slide(true);
                    touching = false;
                }
            }
        }
    }
}
