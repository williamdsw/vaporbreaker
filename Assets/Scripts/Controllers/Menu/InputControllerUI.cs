using Luminosity.IO;
using MVC.Enums;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Controllers.Menu
{
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

        // || Cached

        private Image acceptInputImage;
        private Image cancelInputImage;
        private Image leftInputImage;
        private Image rightInputImage;
        private Image upInputImage;
        private Image downInputImage;
        private Image changeSongInputImage;
        private TextMeshProUGUI acceptLabel;
        private TextMeshProUGUI cancelLabel;
        private TextMeshProUGUI horizontalLabel;
        private TextMeshProUGUI verticalLabel;
        private TextMeshProUGUI changeSongLabel;

        // Cached Others Objects

        public static InputControllerUI Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            GetRequiredComponents();
            Translate();
        }

        private void LateUpdate() => SetupGamepadScheme();

        /// <summary>
        /// Get required components
        /// </summary>
        private void GetRequiredComponents()
        {
            try
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

                acceptLabel = acceptInput.GetComponentInChildren<TextMeshProUGUI>();
                cancelLabel = cancelInput.GetComponentInChildren<TextMeshProUGUI>();
                horizontalLabel = horizontalInput.GetComponentInChildren<TextMeshProUGUI>();
                verticalLabel = verticalInput.GetComponentInChildren<TextMeshProUGUI>();
                changeSongLabel = changeSongInput.GetComponentInChildren<TextMeshProUGUI>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Translates the UI
        /// </summary>
        public void Translate()
        {
            try
            {
                /*acceptLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.icons_accept);
                cancelLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.icons_cancel);
                horizontalLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.icons_move);
                verticalLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.icons_move);
                changeSongLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.instructions_changesong);*/
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Setup gamepad scheme based on current connected gamepad
        /// </summary>
        private void SetupGamepadScheme()
        {
            UpdateUI(GamepadState.IsConnected(GamepadIndex.GamepadOne));
            ToggleInputImagesVisibility();
        }

        /// <summary>
        /// Update UI elements
        /// </summary>
        /// <param name="isGamepadConnected"> Is gamepad connected ? </param>
        private void UpdateUI(bool isGamepadConnected)
        {
            try
            {
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Show or hide elements
        /// </summary>
        private void ToggleInputImagesVisibility()
        {
            try
            {
                GameObject languagePanel = GameObject.Find(NamesTags.Panels.LanguagePanelName);
                GameObject mainPanel = GameObject.Find(NamesTags.Panels.MainPanelName);
                GameObject optionsPanel = GameObject.Find(NamesTags.Panels.OptionsPanelName);
                GameObject pausePanel = GameObject.Find(NamesTags.Panels.PausePanelName);
                GameObject progressPanel = GameObject.Find(NamesTags.Panels.ProgressPanelName);
                GameObject selectLevelPanel = GameObject.Find(NamesTags.Panels.SelectLevelPanelName);

                if (mainPanel && mainPanel.activeSelf)
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
                if (progressPanel && progressPanel.activeSelf)
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

                // OPTIONS MENU OR LANGUAGE PANEL
                if (optionsPanel && optionsPanel.activeSelf || languagePanel && languagePanel.activeSelf)
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}