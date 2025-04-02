using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.UI;  // Make sure we are using the correct UI Image
using UnityEngine.Events;
// Remove using UnityEngine.UIElements;

public class combinationlock : MonoBehaviour
{
    public UnityAction UnlockAction;

    private void OnUnlocked() => UnlockAction?.Invoke();

    public UnityAction LockAction;

    private void OnLocked() => LockAction?.Invoke();

    [SerializeField] TMP_Text userInputText;

    [SerializeField] XrButtoninteractable[] comboButtons;

    [SerializeField] TMP_Text infoText;

    private const string startString = "Enter 3 Digit Combo";

    private const string resetString = "Enter 3 Digits To Reset Combo";

    [SerializeField] Image lockedPanel;  // This now specifically refers to UnityEngine.UI.Image

    [SerializeField] Color unlockedColor;

    [SerializeField] Color lockedColor;

    [SerializeField] TMP_Text lockedText;

    private const string unlockedString = "unlocked";

    private const string lockedString = "locked";

    [SerializeField] bool isLocked;

    [SerializeField] bool isResettable;

    [SerializeField] int[] comboValues = new int[3];

    private int[] inputValues;

    private int maxButtonPresses;

    private int buttonPresses;

    private bool resetCombo;

    void Start()
    {
        maxButtonPresses = comboValues.Length;
        inputValues = new int[comboValues.Length]; // Initialize inputValues first

        ResetUserValues();

        userInputText.text = "";

        for (int i = 0; i < comboButtons.Length; i++)
        {
            comboButtons[i].selectEntered.AddListener(OnComboButtonPressed);
        }
    }

    private void OnComboButtonPressed(SelectEnterEventArgs arg0)
    {
        if (buttonPresses >= maxButtonPresses)
        {
            // Too many button presses
            return;
        }

        for (int i = 0; i < comboButtons.Length; i++)
        {
            if (arg0.interactableObject.transform.name == comboButtons[i].transform.name)
            {
                userInputText.text = i.ToString(); // Fixed missing semicolon
                inputValues[buttonPresses] = i;
            }

            if (comboButtons[i] != null) // Ensure it exists before calling
            {
                comboButtons[i].ResetColor();
            }
        }

        buttonPresses++;
        if (buttonPresses == maxButtonPresses)
        {
            CheckCombo();
        }
    }

    private void CheckCombo()
    {
        if (resetCombo)
        {
            resetCombo = false;
            LockCombo();
            return;
        }

        int matches = 0;

        for (int i = 0; i < maxButtonPresses; i++)
        {
            if (inputValues[i] == comboValues[i])
            {
                matches++;
            }
        }

        if (matches == maxButtonPresses)
        {
            UnlockCombo();
        }
        else
        {
            ResetUserValues();
        }
    }

    private void UnlockCombo()
    {
        isLocked = false;
        OnUnlocked();
        lockedPanel.color = unlockedColor;
        lockedText.text = unlockedString;

        if (isResettable)
        {
            ResetCombo();
        }
    }

    private void LockCombo()
    {
        isLocked = true;
        OnLocked();
        lockedPanel.color = lockedColor;
        lockedText.text = lockedString;
        infoText.text = startString;

        // Save the current combo to comboValues when locking
        for (int i = 0; i < maxButtonPresses; i++)
        {
            comboValues[i] = inputValues[i];
        }

        ResetUserValues();
    }

    private void ResetCombo()
    {
        infoText.text = resetString;
        ResetUserValues();
        resetCombo = true;
    }

    private void ResetUserValues()
    {
        inputValues = new int[maxButtonPresses];
        userInputText.text = "";
        buttonPresses = 0;
    }
}
