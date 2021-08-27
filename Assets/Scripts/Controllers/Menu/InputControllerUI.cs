using Luminosity.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class InputControllerUI : MonoBehaviour
{
    [Header("UI Indicator Sprites")]
    [SerializeField] private Sprite[] keyboardSprites;
    [SerializeField] private Sprite[] gamepadSprites;

    [Header("UI Indicator Game Objects")]
    [SerializeField] private GameObject acceptInput;
    [SerializeField] private GameObject cancelInput;
    [SerializeField] private GameObject horizontalInput;
    [SerializeField] private GameObject leftInput;
    [SerializeField] private GameObject rightInput;
    [SerializeField] private GameObject verticalInput;
    [SerializeField] private GameObject upInput;
    [SerializeField] private GameObject downInput;
    [SerializeField] private GameObject changeSongInput;

    [Header("Labels to Translate")]
    [SerializeField] private List<TextMeshProUGUI> uiLabels = new List<TextMeshProUGUI>();

    // Cached Images
    private Image acceptInputImage;
    private Image cancelInputImage;
    private Image leftInputImage;
    private Image rightInputImage;
    private Image upInputImage;
    private Image downInputImage;
    private Image changeSongInputImage;

    // Cached Others Objects
    private LocalizationController localizationController;

    private void Start()
    {
        localizationController = FindObjectOfType<LocalizationController>();
        TranslateLabels();
        DefineComponents();
    }

    private void LateUpdate()
    {
        SetupControlScheme();
    }

    private void DefineComponents()
    {
        // Submit / Cancel
        if (acceptInput && cancelInput)
        {
            acceptInputImage = acceptInput.GetComponentInChildren<Image>();
            cancelInputImage = cancelInput.GetComponentInChildren<Image>();
        }

        // Left / Right
        if (leftInput && rightInput)
        {
            leftInputImage = leftInput.GetComponentInChildren<Image>();
            rightInputImage = rightInput.GetComponentInChildren<Image>();

            if (!leftInputImage)
            {
                leftInput.GetComponent<Image>();
            }

            if (!rightInputImage)
            {
                rightInput.GetComponent<Image>();
            }
        }

        // Up / Down
        if (upInput && downInput)
        {
            upInputImage = upInput.GetComponentInChildren<Image>();
            downInputImage = downInput.GetComponentInChildren<Image>();

            if (!upInputImage)
            {
                upInput.GetComponent<Image>();
            }

            if (!downInputImage)
            {
                downInput.GetComponent<Image>();
            }
        }

        // Back
        if (changeSongInput)
        {
            changeSongInputImage = changeSongInput.GetComponentInChildren<Image>();
            if (!changeSongInputImage)
            {
                changeSongInput.GetComponent<Image>();
            }
        }
    }

    // Translate labels based on choosed language
    public void TranslateLabels()
    {
        if (!localizationController) return;

        List<string> labels = localizationController.GetUiIconsLabels();
        if (labels.Count == 0 || uiLabels.Count == 0 || labels.Count != uiLabels.Count) return;
        for (int index = 0; index < labels.Count; index++)
        {
            if (!uiLabels[index]) break;
            uiLabels[index].SetText(labels[index]);
        }
    }

    // Setups Control Scheme depending on Gamepad connected
    private void SetupControlScheme()
    {
        UpdateUI(GamepadState.IsConnected(GamepadIndex.GamepadOne));
        ToggleInputImagesVisibility();
    }

    private void UpdateUI(bool isGamepadConnected)
    {
        if (keyboardSprites.Length == 0 || gamepadSprites.Length == 0) return;

        // Submit / Cancel
        acceptInputImage.sprite = (isGamepadConnected ? gamepadSprites[0] : keyboardSprites[0]);
        cancelInputImage.sprite = (isGamepadConnected ? gamepadSprites[1] : keyboardSprites[1]);

        // Horizontal
        if (leftInputImage)
        {
            leftInputImage.sprite = (isGamepadConnected ? gamepadSprites[2] : keyboardSprites[2]);
        }

        if (rightInputImage)
        {
            rightInputImage.sprite = (isGamepadConnected ? gamepadSprites[3] : keyboardSprites[3]);
        }

        // Vertical
        if (upInputImage)
        {
            upInputImage.sprite = (isGamepadConnected ? gamepadSprites[4] : keyboardSprites[4]);
        }

        if (downInputImage)
        {
            downInputImage.sprite = (isGamepadConnected ? gamepadSprites[5] : keyboardSprites[5]);
        }

        // Change Song 
        if (changeSongInput)
        {
            changeSongInputImage.sprite = (isGamepadConnected ? gamepadSprites[6] : keyboardSprites[6]);
        }
    }

    // Toggles input images visibiliy depending on which panel is active
    private void ToggleInputImagesVisibility()
    {
        // Panels
        GameObject mainMenu = GameObject.Find(NamesTags.MainPanelName);
        GameObject progressMenu = GameObject.Find(NamesTags.ProgressPanelName);
        GameObject optionsMenu = GameObject.Find(NamesTags.OptionsPanelName);
        GameObject defaultKeyboardMenu = GameObject.Find(NamesTags.DefaultKeyboardLayoutPanelName);
        GameObject defaultGamepadMenu = GameObject.Find(NamesTags.DefaultGamepadLayoutPanelName);
        GameObject bindingMenu = GameObject.Find(NamesTags.BindingsPanelName);
        GameObject selectLevelPanel = GameObject.Find(NamesTags.SelectLevelPanelName);
        GameObject pausePanel = GameObject.Find(NamesTags.PausePanelName);

        // MAIN MENU
        if (mainMenu && mainMenu.activeSelf)
        {
            if (horizontalInput && horizontalInput.activeSelf)
            {
                horizontalInput.SetActive(false);
            }

            if (changeSongInput && changeSongInput.activeSelf)
            {
                changeSongInput.SetActive(false);
            }

            return;
        }

        // PROGRESS MENU
        if (progressMenu && progressMenu.activeSelf)
        {
            if (horizontalInput && !horizontalInput.activeSelf)
            {
                horizontalInput.SetActive(true);
            }

            if (changeSongInput && changeSongInput.activeSelf)
            {
                changeSongInput.SetActive(false);
            }

            return;
        }

        // OPTIONS MENU OR BINDING MENU
        if ((optionsMenu && optionsMenu.activeSelf) || (bindingMenu && bindingMenu.activeSelf))
        {
            if (horizontalInput && !horizontalInput.activeSelf)
            {
                horizontalInput.SetActive(true);
            }

            if (verticalInput && !verticalInput.activeSelf)
            {
                verticalInput.SetActive(true);
            }

            if (acceptInput && !acceptInput.activeSelf)
            {
                acceptInput.SetActive(true);
            }

            if (changeSongInput && changeSongInput.activeSelf)
            {
                changeSongInput.SetActive(false);
            }

            return;
        }

        // DEFAULT KEYBOARD MENU OR DEFAULT GAMEPAD MENU
        if ((defaultKeyboardMenu && defaultKeyboardMenu.activeSelf) || (defaultGamepadMenu && defaultGamepadMenu.activeSelf))
        {
            if (horizontalInput && horizontalInput.activeSelf)
            {
                horizontalInput.SetActive(false);
            }

            if (verticalInput && verticalInput.activeSelf)
            {
                verticalInput.SetActive(false);
            }

            if (acceptInput && acceptInput.activeSelf)
            {
                acceptInput.SetActive(false);
            }

            if (changeSongInput && changeSongInput.activeSelf)
            {
                changeSongInput.SetActive(false);
            }

            return;
        }

        // SELECT LEVEL PANEL
        if (selectLevelPanel && selectLevelPanel.activeSelf)
        {
            if (verticalInput && verticalInput.activeSelf)
            {
                verticalInput.SetActive(false);
            }

            if (changeSongInput && changeSongInput.activeSelf)
            {
                changeSongInput.SetActive(false);
            }

            return;
        }

        // PAUSE PANEL
        if (pausePanel && pausePanel.activeSelf)
        {
            if (horizontalInput && horizontalInput.activeSelf)
            {
                horizontalInput.SetActive(false);
            }

            if (changeSongInput && !changeSongInput.activeSelf)
            {
                changeSongInput.SetActive(true);
            }

            return;
        }
    }
}
