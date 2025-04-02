using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class XrButtoninteractable : XRSimpleInteractable
{
    [SerializeField] Image buttonImage;

    [SerializeField] Color[] buttonColors = new Color[4];

    [SerializeField] private Color normalColor;

    [SerializeField] private Color highlightedColor;

    [SerializeField] private Color pressedColor;

    [SerializeField] private Color selectedColor;

    [SerializeField]private bool isPressed;
    // Start is called before the first frame update
    void Start()
    {
        normalColor = buttonColors[0];
        highlightedColor = buttonColors[1];
        pressedColor = buttonColors[2];
        selectedColor = buttonColors[3];
        ResetColor();
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        isPressed = false;
        buttonImage.color = highlightedColor;
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        if (!isPressed)
        {
            buttonImage.color = normalColor;
        }    
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        isPressed = true;
        buttonImage.color = pressedColor;

    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        buttonImage.color = selectedColor;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetColor()
    {
        buttonImage.color = normalColor;
    }
}
