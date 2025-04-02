using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DrawerInteractable : XRGrabInteractable
{
   
    [SerializeField] private Transform drawerTransform;
    [SerializeField] private XRSocketInteractor keySocket;
    [SerializeField] private GameObject keyIndicatorLight;
    [SerializeField] private bool isLocked;
    [SerializeField] private Vector3 limitDistances = new Vector3(.02f, .02f, 0);

  
    private Transform parentTransform;
    private Vector3 limitPositions;

  
    private const string Default_Layer = "Default";
    private const string Grab_Layer = "Grab";
    private bool isGrabbed;


    void Start()
    {
        if (keySocket != null)
        {
            keySocket.selectEntered.AddListener(OnDrawerUnlocked);
            keySocket.selectExited.AddListener(OnDrawerLocked);
        }

        parentTransform = transform.parent;
        limitPositions = drawerTransform.localPosition;
    }

    private void OnDrawerLocked(SelectExitEventArgs arg0)
    {
        isLocked = true;
        Debug.Log("DRAWER LOCKED");
    }

    private void OnDrawerUnlocked(SelectEnterEventArgs arg0)
    {
        isLocked = false;
        if (keyIndicatorLight != null)
        {
            keyIndicatorLight.SetActive(false);
        }
        Debug.Log("DRAWER UNLOCKED");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (!isLocked)
        {
            transform.SetParent(parentTransform);
            isGrabbed = true;
        }
        else
        {
            ChangeLayerMask(Default_Layer);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        ChangeLayerMask(Grab_Layer);
        isGrabbed = false;
    }

    void Update()
    {
        if (isGrabbed && drawerTransform != null)
        {
            drawerTransform.localPosition = new Vector3(
                drawerTransform.localPosition.x,
                drawerTransform.localPosition.y,
                transform.localPosition.z
            );

            CheckLimits();
        }
    }

 
    private void CheckLimits()
    {
        Vector3 localPos = transform.localPosition;

        if (localPos.x > limitPositions.x + limitDistances.x ||
            localPos.x < limitPositions.x - limitDistances.x ||
            localPos.y > limitPositions.y + limitDistances.y ||
            localPos.y < limitPositions.y - limitDistances.y)
        {
            ChangeLayerMask(Default_Layer);
        }
    }

    private void ChangeLayerMask(string mask)
    {
        gameObject.layer = LayerMask.NameToLayer(mask);
    }
}
