using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XrAudioManager : MonoBehaviour
{
    [SerializeField] XRGrabInteractable[] grabInteractables;

    [SerializeField] AudioSource grabSound;

    [SerializeField] AudioClip grabClip;

    [SerializeField] AudioClip keyClip;

    [SerializeField] AudioSource activatedSound;

    [SerializeField] AudioClip grabActivatedClip;

    [SerializeField] AudioClip wandActivatedClip;



    [SerializeField] private TheWall wall;           // Reference to TheWall component
    [SerializeField] private AudioSource wallSource; // AudioSource to play sound
    [SerializeField] private AudioClip destroyWallClip; // Clip to play when wall is destroyed
    [SerializeField] private AudioClip fallBackClip; // Fallback clip if no destroyWallClip is found

    private const string FallBackClip_Name = "fallBackClip"; // Fallback clip name

    private void OnEnable()
    {
        grabInteractables = FindObjectsByType<XRGrabInteractable>(FindObjectsSortMode.None);

        for (int i = 0; i < grabInteractables.Length; i++)
        {
            grabInteractables[i].selectEntered.AddListener(OnSelectEnterGrabbable);
            grabInteractables[i].selectExited.AddListener(OnSelectExitGrabbable);
            //grabInteractables[i].activated.AddListener(OnActivatedGrabbable);
        }

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

        // Add listener to the OnDestroy event of the wall
        if (wall.OnDestroy != null)
        {
            wall.OnDestroy.AddListener(OnDestroyWall);
        }
        else
        {
            Debug.LogWarning("OnDestroy event in 'TheWall' is not set up!");
        }
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
