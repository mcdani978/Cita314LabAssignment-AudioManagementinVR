using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class DoorInteractable : SimpleHingeInteractable
{
    [SerializeField] private bool isLocked; // Simulated lock state, replacing CombinationLock
    [SerializeField] private Transform doorObject;
    [SerializeField] private Vector3 rotationLimits;
    [SerializeField] private Collider closedCollider;

    private bool isClosed;
    private Vector3 startRotation;
    private float startAngleX;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        startRotation = transform.localEulerAngles;  // Store the initial rotation
        startAngleX = startRotation.x;

        if (startAngleX >= 180)
        {
            startAngleX -= 360;
        }

        // Replacing the missing CombinationLock logic with basic lock/unlock functionality
        if (isLocked)
        {
            OnLocked();
        }
        else
        {
            OnUnlocked();
        }
    }

    private void OnLocked()
    {
        LockHinge();
    }

    private void OnUnlocked()
    {
        UnlockHinge();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (doorObject != null)
        {
            doorObject.localEulerAngles = new Vector3(
                doorObject.localEulerAngles.x,
                transform.localEulerAngles.y,  // Ensures rotation follows the base hinge
                doorObject.localEulerAngles.z
            );
        }

        if (isSelected)
        {
            CheckLimits();
        }
    }

    private void CheckLimits()
    {
        isClosed = false;
        float localAngleX = transform.localEulerAngles.x;
        if (localAngleX >= startAngleX + rotationLimits.x ||
            localAngleX <= startAngleX - rotationLimits.x)
        {
            ReleaseHinge();
        }
    }

    private float GetAngle(float angle)
    {
        if (angle >= 180)
        {
            angle -= 360;
        }
        return angle;
    }

    protected override void ResetHinge()
    {
        if (isClosed)
        {
            transform.localEulerAngles = startRotation;
        }
    }

    // Abstract methods for the base class
    protected abstract void LockHinge();
    protected abstract void UnlockHinge();
    protected abstract void ReleaseHinge();
}
