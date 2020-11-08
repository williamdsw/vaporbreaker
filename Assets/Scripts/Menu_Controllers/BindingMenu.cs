using Luminosity.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BindingMenu : MonoBehaviour
{
    [Header ("UI Elements")]
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject bindingMenu;
    [SerializeField] private Button[] labelButtons;
    [SerializeField] private Button[] valueButtons;
    [SerializeField] private Button backButton;
    [SerializeField] private Button defaultButton;
    [SerializeField] private Button defaultOptionsMenuButton;
    [SerializeField] private Button defaultBindingMenuButton;

    [Header ("Labels to Translate")]
    [SerializeField] private List<TextMeshProUGUI> uiLabels = new List<TextMeshProUGUI> ();

    // Cached
    private List<TextMeshProUGUI> valueButtonsTexts = new List<TextMeshProUGUI> ();
    private AudioController audioController;
    private LocalizationController localizationController;

    //--------------------------------------------------------------------------------//

    private void Start () 
    {
        // Components
        foreach (Button button in valueButtons)
        {
            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
            valueButtonsTexts.Add (text);
        }

        // Other Objects
        audioController = FindObjectOfType<AudioController>();
        localizationController = FindObjectOfType<LocalizationController>();

        BindingClickEvents ();
        TranslateLabels ();
        UpdateUI ();
    }

    private void Update() 
    {
        // Cancels
        if (optionsMenu.activeSelf) { return; }
        if (labelButtons.Length == 0 || valueButtons.Length == 0) { return; }

        ClickOrCancelButton (valueButtons[0], labelButtons[0]);
        ClickOrCancelButton (valueButtons[1], labelButtons[1]);
        ClickOrCancelButton (valueButtons[2], labelButtons[2]);
        ClickOrCancelButton (valueButtons[3], labelButtons[3]);
        ClickOrCancelButton (valueButtons[4], labelButtons[4]);
        ClickOrCancelButton (valueButtons[5], labelButtons[5]);
    }

    //--------------------------------------------------------------------------------//

    private void BindingClickEvents ()
    {
        if (labelButtons.Length == 0 || valueButtons.Length == 0) { return; }

        // KEYBOARD MOVE LEFT
        labelButtons[0].onClick.AddListener (delegate
        {
            // Cancels
            if (GamepadState.IsConnected (GamepadIndex.GamepadOne)) { return; }

            valueButtons[0].interactable = true;
            ToggleValueButtonColor (valueButtons[0]);
            valueButtons[0].Select ();
            ToggleAllLabelButtonsInteractable ();
        });

        // KEYBOARD MOVE RIGHT
        labelButtons[1].onClick.AddListener (delegate
        {
            // Cancels
            if (GamepadState.IsConnected (GamepadIndex.GamepadOne)) { return; }

            valueButtons[1].interactable = true;
            ToggleValueButtonColor (valueButtons[1]);
            valueButtons[1].Select ();
            ToggleAllLabelButtonsInteractable ();
        });

        // KEYBOARD IMPULSE
        labelButtons[2].onClick.AddListener (delegate
        {
            // Cancels
            if (GamepadState.IsConnected (GamepadIndex.GamepadOne)) { return; }

            valueButtons[2].interactable = true;
            ToggleValueButtonColor (valueButtons[2]);
            valueButtons[2].Select ();
            ToggleAllLabelButtonsInteractable ();
        });

        // KEYBOARD SHOOT
        labelButtons[3].onClick.AddListener (delegate
        {
            // Cancels
            if (GamepadState.IsConnected (GamepadIndex.GamepadOne)) { return; }

            valueButtons[3].interactable = true;
            ToggleValueButtonColor (valueButtons[3]);
            valueButtons[3].Select ();
            ToggleAllLabelButtonsInteractable ();
        });

        // GAMEPAD IMPULSE
        labelButtons[4].onClick.AddListener (delegate
        {
            // Cancels
            if (!GamepadState.IsConnected (GamepadIndex.GamepadOne)) { return; }

            valueButtons[4].interactable = true;
            ToggleValueButtonColor (valueButtons[4]);
            valueButtons[4].Select ();
            ToggleAllLabelButtonsInteractable ();
        });

        // GAMEPAD SHOOT
        labelButtons[5].onClick.AddListener (delegate
        {
            // Cancels
            if (!GamepadState.IsConnected (GamepadIndex.GamepadOne)) { return; }

            valueButtons[5].interactable = true;
            ToggleValueButtonColor (valueButtons[5]);
            valueButtons[5].Select ();
            ToggleAllLabelButtonsInteractable ();
        });

        // BACK BUTTON
        backButton.onClick.AddListener (delegate
        {
            audioController.PlaySFX (audioController.UiCancel, audioController.GetMaxSFXVolume ());
            bindingMenu.SetActive (false);
            optionsMenu.SetActive (true);
            defaultOptionsMenuButton.Select ();
            InputManager.Save ();
        });

        // DEFAULT BUTTON
        defaultButton.onClick.AddListener (delegate 
        { 
            audioController.PlaySFX (audioController.UiSubmit, audioController.GetMaxSFXVolume ());
            DefaultBindingValues ();
            UpdateUI ();
        });

        // UI Sounds
        foreach (Button button in labelButtons)
        {
            button.onClick.AddListener (delegate 
            {
                audioController.PlaySFX (audioController.UiSubmit, audioController.GetMaxSFXVolume ());
            });
        }

        // Rebind coroutines
        foreach (Button button in valueButtons)
        {
            button.onClick.AddListener (delegate 
            {
                RebindInput rebindInput = button.GetComponent<RebindInput>();
                rebindInput.StopAllCoroutines ();
                StartCoroutine (rebindInput.StartInputScanDelayed ());
                audioController.PlaySFX (audioController.UiSubmit, audioController.GetMaxSFXVolume ());
            });
        }
    }

    // Translate labels based on choosed language
    public void TranslateLabels ()
    {
        if (!localizationController) { return; }
        List<string> labels = localizationController.GetBindingsLabels ();
        if (labels.Count == 0 || uiLabels.Count == 0 || labels.Count != uiLabels.Count ) { return; }
        for (int index = 0; index < labels.Count; index++) { uiLabels[index].SetText (labels[index]); }
    }

    //--------------------------------------------------------------------------------//

    private void ToggleAllLabelButtonsInteractable ()
    {
        // Cancels
        if (labelButtons.Length == 0) { return; }

        foreach (Button button in labelButtons) { button.interactable = !button.interactable; }
        backButton.interactable = !backButton.interactable;
        defaultButton.interactable = !defaultButton.interactable;
    }

    private void ToggleValueButtonColor (Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = (button.interactable ? Color.yellow : Color.white);
        button.colors = colors;
    }

    //--------------------------------------------------------------------------------//

    // Controls input resolution
    private void ClickOrCancelButton (Button valueButton, Button labelButton)
    {
        if (valueButton.interactable)
        {
            if (InputManager.GetButtonDown ("UI_Submit"))
            {
                valueButton.onClick.Invoke ();
            }
            else if (InputManager.GetButtonDown ("UI_Cancel"))
            {
                valueButton.interactable = false;
                ToggleValueButtonColor (valueButton);
                labelButton.Select ();
                ToggleAllLabelButtonsInteractable ();
            }
        }
    }

    //--------------------------------------------------------------------------------//
    // MANAGEMENT

    // Define default values
    private void DefaultBindingValues ()
    {
        ControlScheme controlScheme = InputManager.GetControlScheme ("Keyboard_Mouse_Gamepad");

        // ----- Horizontal -----
        InputAction horizontalAction = controlScheme.GetAction (0);
        InputBinding horizontalBinding = horizontalAction.GetBinding (0);
        horizontalBinding.Positive = KeyCode.D;
        horizontalBinding.Negative = KeyCode.A;

        // ----- Impulse -----
        InputAction impulseAction = controlScheme.GetAction (1);

        // Keyboard
        InputBinding keyboardImpulseBinding = impulseAction.GetBinding (0);
        keyboardImpulseBinding.Positive = KeyCode.LeftShift;

        // Gamepad
        InputBinding gamepadImpulseBinding = impulseAction.GetBinding (2);
        gamepadImpulseBinding.GamepadAxis = GamepadAxis.LeftTrigger;

        // ----- Shoot -----
        InputAction shootAction = controlScheme.GetAction (2);

        // Keyboard
        InputBinding keyboardShootBinding = shootAction.GetBinding (0);
        keyboardShootBinding.Positive = KeyCode.RightControl;

        // Keyboard
        InputBinding gamepadShootBinding = shootAction.GetBinding (2);
        gamepadShootBinding.GamepadButton = GamepadButton.ActionLeft;
    }

    public void UpdateUI ()
    {
        ControlScheme controlScheme = InputManager.GetControlScheme ("Keyboard_Mouse_Gamepad");

        // ----- Horizontal -----
        InputAction horizontalAction = controlScheme.GetAction (0);
        InputBinding horizontalBinding = horizontalAction.GetBinding (0);

        // ----- Impulse -----
        InputAction impulseAction = controlScheme.GetAction (1);
        InputBinding keyboardImpulseBinding = impulseAction.GetBinding (0);
        InputBinding gamepadImpulseBinding = impulseAction.GetBinding (2);

        // ----- Shoot -----
        InputAction shootAction = controlScheme.GetAction (2);
        InputBinding keyboardShootBinding = shootAction.GetBinding (0);
        InputBinding gamepadShootBinding = shootAction.GetBinding (2);

        if (valueButtonsTexts.Count == 0) { return; }
        valueButtonsTexts[0].text = horizontalBinding.Negative.ToString ();
        valueButtonsTexts[1].text = horizontalBinding.Positive.ToString ();
        valueButtonsTexts[2].text = keyboardImpulseBinding.Positive.ToString ();
        valueButtonsTexts[3].text = keyboardShootBinding.Positive.ToString ();
        valueButtonsTexts[4].text = gamepadImpulseBinding.GamepadAxis.ToString ();
        valueButtonsTexts[5].text = gamepadShootBinding.GamepadButton.ToString ();
    }
}