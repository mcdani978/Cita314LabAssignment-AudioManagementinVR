using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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

        if (drawer != null)
        {
            SetDrawerInteractables();

        }
        else
        {
            Debug.LogWarning("OnDestroy event in 'TheWall' is not set up!");
        }
    }

    private void SetGrabbables()
    {
        for (int i = 0; i < grabInteractables.Length; i++)
        {
            grabInteractables[i].selectEntered.AddListener(OnSelectEnterGrabbable);
            grabInteractables[i].selectExited.AddListener(OnSelectExitGrabbable);
            //grabInteractables[i].activated.AddListener(OnActivatedGrabbable);
        }
    }

    private void CheckClip(AudioClip clip)
    {
        clip = fallBackClip;
    }

    private void SetDrawerInteractables()
    {
        drawerSound = drawer.transform.AddComponent<AudioSource>();
        drawerMoveClip = drawer.GetDrawerMoveClip;
        CheckClip(drawerMoveClip);
        drawerSound.clip = drawerMoveClip;
        drawerSound.lopp = true;
        drawer.selectEntered.AddListener(OnDrawerMove);
        drawer.selectExited.AddListener(OnDrawerStop);
        drawerSocket = drawer.GetKeySocket;
        if(drawerSocket != null)
        {
            drawerSocketSound = drawerSocket.transform.AddComponent<AudioSource>();
            drawerSocket.SelectEntered.AddListner(OnDrawerSocketed);
            drawerSocketClip = drawer.GetSocketedClip;
            CheckClip(drawerSocketClip);
            drawerSocketSound.clip = drawerSocketClip;
        }
    }

    private void OnDrawerSocketed(SelectEnterEventArgs arg0)
    {

    }

    private void SetWall()
    {
        drawerSocketSound.Play();
    }

    private void OnDrawerMove(SelectEnterEventArgs arg0)
    {
        drawerSound.Play();
    }

    private void OnDrawerStop(SelectEnterEventArgs argo)
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

    //private void OnActivatedGrabbable(ActivateEventArgs arg0)
    //{
    //    GameObject tempGameObject = arg0.interactableObject.transform.gameObject;
    //    if(tempGameObject.GetComponent<WandControl>() != null)
    //    {
    //        activatedSound.clip = wandActivatedClip;
    //    }

    //    else
    //    {
    //        activatedSound.clip = grabActivatedClip;
    //    }
    //    activatedSound.Play();

    //}
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
