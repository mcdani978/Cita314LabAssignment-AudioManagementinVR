using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class ExplosiveDevice : XRGrabInteractable
{
    public UnityEvent OnDetonated; // Fixed position of UnityEvent declaration

    private bool isActivated;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        // Fixed incorrect GetComponent syntax
        if (args.interactableObject.transform.GetComponent<XRSocketInteractor>() != null)
        {
            isActivated = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isActivated)
        {
            OnDetonated?.Invoke();
        }
    }
}
