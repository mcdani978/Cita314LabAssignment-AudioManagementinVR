using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class XrAudioManager : MonoBehaviour
{
    [Header("Grab Interactables")]
    [SerializeField] XRGrabInteractable[] grabInteractables;
    [SerializeField] AudioSource grabSound;
    [SerializeField] AudioClip grabClip;
    [SerializeField] AudioClip keyClip;
    [SerializeField] AudioSource activatedSound;
    [SerializeField] AudioClip grabActivatedClip;
    [SerializeField] AudioClip wandActivatedClip;

    [Header("Drawer Interactables")]
    [SerializeField] DrawerInteractable drawer;
    [SerializeField] AudioSource drawerSound;
    [SerializeField] AudioClip drawerMoveClip;

    [Header("Hinge Interactables")]
    [SerializeField] SimpleHingeInteractable[] cabinetDoors = new SimpleHingeInteractable[2];
    [SerializeField] AudioSource[] cabinetDoorSound;
    [SerializeField] AudioClip cabinetDoorMoveClip;

    [SerializeField] AudioClip lockComboClip;

    [SerializeField] AudioClip unlockComboClip;

    [SerializeField] AudioClip comboButtonPressedClip;

    [Header("Combo Lock")]
    [SerializeField] combinationlock comboLock;

    [SerializeField] AudioSource combinationlockSound;


    [Header("The Wall")]
    [SerializeField] private TheWall wall;           // Reference to TheWall component
    [SerializeField] private AudioSource wallSource; // AudioSource to play sound
    [SerializeField] private AudioClip destroyWallClip; // Clip to play when wall is destroyed
    [SerializeField] private AudioClip fallBackClip; // Fallback clip if no destroyWallClip is found

    private const string FallBackClip_Name = "fallBackClip"; // Fallback clip name

    private void OnEnable()
    {
        SetGrabbables();
        grabInteractables = FindObjectsByType<XRGrabInteractable>(FindObjectsSortMode.None);

        // If the fallback clip is not set, create it with a simple silent clip (1 sample, 44100 Hz, 1 channel)
        if (fallBackClip == null)
        {
            fallBackClip = AudioClip.Create(FallBackClip_Name, 1, 1, 44100, false);
        }

        // Check if the wall reference is assigned
        if (wall == null)
        {
            Debug.LogError("Wall reference is not assigned!");
            return;
        }

        // Use fallback clip if no destroyWallClip is found
        if (destroyWallClip == null)
        {
            destroyWallClip = fallBackClip;
        }

        drawerSound.clip = drawerMoveClip;

        // Add listener to the OnDestroy event of the wall
        if (wall.OnDestroy != null)
        {
            wall.OnDestroy.AddListener(OnDestroyWall);
        }
        else
        {
            Debug.LogWarning("OnDestroy event in 'TheWall' is not set up!");
        }

        // Initialize cabinet doors and sounds
        cabinetDoorSound = new AudioSource[cabinetDoors.Length];
        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            SetCabinetDoors(i);
        }
    }

    private void SetGrabbables()
    {
        for (int i = 0; i < grabInteractables.Length; i++)
        {
            grabInteractables[i].selectEntered.AddListener(OnSelectEnterGrabbable);
            grabInteractables[i].selectExited.AddListener(OnSelectExitGrabbable);
        }
    }

    private void SetCabinetDoors(int index)
    {
        if (cabinetDoors[index] != null)
        {
            cabinetDoorSound[index] = cabinetDoors[index].gameObject.AddComponent<AudioSource>();
            cabinetDoorMoveClip = cabinetDoors[index].GetHingeMoveClip;  // Removed parentheses
            CheckClip(ref cabinetDoorMoveClip);
            cabinetDoorSound[index].clip = cabinetDoorMoveClip;
            cabinetDoors[index].OnHingeSelected.AddListener(OnDoorMove);
        }
    }

    private void OnDoorMove(SimpleHingeInteractable arg0)
    {
        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            if (arg0 == cabinetDoors[i])
            {
                cabinetDoorSound[i].Play();
            }
        }
    }

    private void OnDoorStop(SelectExitEventArgs arg0)
    {
        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            if (arg0.interactableObject == cabinetDoors[i])
            {
                cabinetDoorSound[i].Stop();
            }
        }
    }

    private void CheckClip(ref AudioClip clip)
    {
        if (clip == null)
        {
            clip = fallBackClip;
        }
    }

    private void OnDrawerMove(SelectEnterEventArgs arg0)
    {
        drawerSound.Play();
    }

    private void OnDrawerStop(SelectExitEventArgs arg0)
    {
        drawerSound.Stop();
    }

    private void OnSelectEnterGrabbable(SelectEnterEventArgs arg0)
    {
        if (arg0.interactableObject.transform.CompareTag("Key"))
        {
            grabSound.clip = keyClip;
        }
        else
        {
            grabSound.clip = grabClip;
        }
        grabSound.Play();
    }

    private void OnSelectExitGrabbable(SelectExitEventArgs arg0)
    {
        grabSound.clip = grabClip;
        grabSound.Play();
    }

    private void OnDestroyWall()
    {
        // Play the destroy clip only if the wallSource is assigned
        if (wallSource != null && destroyWallClip != null)
        {
            wallSource.clip = destroyWallClip;
            wallSource.Play();
        }
        else
        {
            Debug.LogError("AudioSource or destroyWallClip is not assigned!");
        }
    }

    private void OnDisable()
    {
        // Remove listener when the component is disabled
        if (wall != null && wall.OnDestroy != null)
        {
            wall.OnDestroy.RemoveListener(OnDestroyWall);
        }
    }
}

