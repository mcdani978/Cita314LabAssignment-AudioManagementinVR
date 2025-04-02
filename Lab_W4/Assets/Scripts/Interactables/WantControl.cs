using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WantControl : XRGrabInteractable
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawnPoint; // Fixed "Tranform" typo

    private bool isFiring;

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        FireProjectile(); // Optional: Trigger projectile firing
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);
    }

    private void FireProjectile()
    {
        if (projectilePrefab != null && projectileSpawnPoint != null)
        {
            Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        }
    }
}
