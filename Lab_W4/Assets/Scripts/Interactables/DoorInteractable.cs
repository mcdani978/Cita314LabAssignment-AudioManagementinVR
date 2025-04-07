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

        // Initialize the door's starting rotation
        startRotation = transform.localEulerAngles;
        startAngleX = GetAngle(startRotation.x);

        // Lock or unlock the door based on the initial state
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
            // Keep doorObject's rotation synchronized with the base hinge rotation
            doorObject.localEulerAngles = new Vector3(
                doorObject.localEulerAngles.x,
                transform.localEulerAngles.y,  // Keeps y-axis rotation synchronized
                doorObject.localEulerAngles.z
            );
        }

        // If the door is selected, check the rotation limits
        if (isSelected)
        {
            CheckLimits();
        }
    }

    private void CheckLimits()
    {
        isClosed = false;
        float localAngleX = GetAngle(transform.localEulerAngles.x);

        // Ensure the door does not exceed the rotation limits
        if (localAngleX >= startAngleX + rotationLimits.x || localAngleX <= startAngleX - rotationLimits.x)
        {
            ReleaseHinge();
        }
    }

    private float GetAngle(float angle)
    {
        // Normalize the angle to the range -180 to 180
        if (angle >= 180)
        {
            angle -= 360;
        }
        return angle;
    }

    protected override void ResetHinge()
    {
        // Reset to the start rotation if the door is closed
        if (isClosed)
        {
            transform.localEulerAngles = startRotation;
        }
    }

    // Abstract methods for the base class that need to be implemented
    protected abstract void LockHinge();
    protected abstract void UnlockHinge();
    protected abstract void ReleaseHinge();
}
