using Controllers.Core;
using Luminosity.IO;
using MVC.Enums;
using MVC.Global;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Controllers.Menu
{
    public class OptionsMenuController : MonoBehaviour
    {
        [Header("Required UI Elements")]
        [SerializeField] private GameObject panel;

        [Header("Label Buttons")]
        [SerializeField] private Button resolutionButton;
        [SerializeField] private Button fullScreenButton;
        [SerializeField] private Button bgmVolumeButton;
        [SerializeField] private Button sfxVolumeButton;

        [Header("Value Buttons")]
        [SerializeField] private Button resolutionValueButton;
        [SerializeField] private Button fullScreenValueButton;
        [SerializeField] private Button bgmVolumeValueButton;
        [SerializeField] private Button sfxVolumeValueButton;

        [Header("Other Buttons")]
        [SerializeField] private Button backButton;
        [SerializeField] private Button defaultButton;
        [SerializeField] private Button[] labelButtons;
        [SerializeField] private Button[] allOtherButtons;

        [Header("Other Texts to Translate")]
        [SerializeField] private TextMeshProUGUI header;

        // Params config
        private List<string> resolutions = new List<string>();
        private string[] fullScreenOptions = { "No", "Yes" };
        private int resolutionIndex = 0;
        private int fullScreenIndex = 0;
        private string actualResolution = string.Empty;
        private bool isFullscreen = true;
        private float BGMVolume = 1f;
        private float SFXVolume = 1f;
        private int width;
        private int height;

        // Delay times
        private float timeToWaitUpdateVolume = 0.1f;
        private float startTimeToWaitUpdateVolume = 0.1f;
        private float timeToPlaySound = 1f;
        private float startTimeToPlaySound = 1f;

        // || Cached

        private TextMeshProUGUI resolutionButtonLabel;
        private TextMeshProUGUI fullScreenButtonLabel;
        private TextMeshProUGUI bgmVolumeButtonLabel;
        private TextMeshProUGUI sfxVolumeButtonLabel;
        private TextMeshProUGUI resolutionValueButtonLabel;
        private TextMeshProUGUI fullScreenValueButtonLabel;
        private TextMeshProUGUI bgmVolumeValueButtonLabel;
        private TextMeshProUGUI sfxVolumeValueButtonLabel;
        private TextMeshProUGUI backButtonLabel;
        private TextMeshProUGUI defaultButtonLabel;

        // || Properties

        public static OptionsMenuController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            GetRequiredComponents();
            Translate();
            BindEventListeners();
            GetResolutions();
            LoadSettings();
        }

        private void Update()
        {
            ChooseResolution();
            ChooseFullScreen();
            ChooseVolume(bgmVolumeButton, bgmVolumeValueButton, bgmVolumeValueButtonLabel, AudioController.Instance.AudioSourceBGM);
            ChooseVolume(sfxVolumeButton, sfxVolumeValueButton, sfxVolumeValueButtonLabel, AudioController.Instance.AudioSourceSFX);
            CheckSelectedGameObject();
            CaptureCancelButton();
        }

        /// <summary>
        /// Get required components
        /// </summary>
        private void GetRequiredComponents()
        {
            try
            {
                resolutionButtonLabel = resolutionButton.GetComponentInChildren<TextMeshProUGUI>();
                fullScreenButtonLabel = fullScreenButton.GetComponentInChildren<TextMeshProUGUI>();
                bgmVolumeButtonLabel = bgmVolumeButton.GetComponentInChildren<TextMeshProUGUI>();
                sfxVolumeButtonLabel = sfxVolumeButton.GetComponentInChildren<TextMeshProUGUI>();
                resolutionValueButtonLabel = resolutionValueButton.GetComponentInChildren<TextMeshProUGUI>();
                fullScreenValueButtonLabel = fullScreenValueButton.GetComponentInChildren<TextMeshProUGUI>();
                bgmVolumeValueButtonLabel = bgmVolumeValueButton.GetComponentInChildren<TextMeshProUGUI>();
                sfxVolumeValueButtonLabel = sfxVolumeValueButton.GetComponentInChildren<TextMeshProUGUI>();
                backButtonLabel = backButton.GetComponentInChildren<TextMeshProUGUI>();
                defaultButtonLabel = defaultButton.GetComponentInChildren<TextMeshProUGUI>();
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
                header.text = LocalizationController.Instance.GetWord(LocalizationFields.mainmenu_options);
                fullScreenButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.options_fullscreen);
                resolutionButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.options_resolution);
                bgmVolumeButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.options_backgroundvolume);
                sfxVolumeButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.options_effectsvolume);
                backButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.general_back);
                defaultButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.general_default);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Show or hide the panel
        /// </summary>
        /// <param name="toShow"> Is to show the panel ? </param>
        public void TogglePanel(bool toShow)
        {
            panel.SetActive(toShow);

            if (toShow)
            {
                resolutionButton.Select();
                Translate();
            }
        }

        /// <summary>
        /// Bind event listeners to elements
        /// </summary>
        private void BindEventListeners()
        {
            try
            {
                // Label Buttons
                resolutionButton.onClick.AddListener(() => ToggleButton(resolutionValueButton, true));
                fullScreenButton.onClick.AddListener(() => ToggleButton(fullScreenValueButton, true));
                bgmVolumeButton.onClick.AddListener(() => ToggleButton(bgmVolumeValueButton, true));
                sfxVolumeButton.onClick.AddListener(() =>
                {
                    ToggleButton(sfxVolumeValueButton, true);
                    ApplyValues();
                });

                // Value Buttons
                resolutionValueButton.onClick.AddListener(() =>
                {
                    ToggleButton(resolutionValueButton, false);
                    resolutionButton.Select();
                });

                fullScreenValueButton.onClick.AddListener(() =>
                {
                    ToggleButton(fullScreenValueButton, false);
                    fullScreenButton.Select();
                });

                bgmVolumeValueButton.onClick.AddListener(() =>
                {
                    ToggleButton(bgmVolumeValueButton, false);
                    bgmVolumeButton.Select();
                    AudioController.Instance.AudioSourceBGM.volume = BGMVolume;
                });

                sfxVolumeValueButton.onClick.AddListener(() =>
                {
                    ToggleButton(sfxVolumeValueButton, false);
                    sfxVolumeButton.Select();
                    ApplyValues();
                });

                backButton.onClick.AddListener(() => GetBackToMainMenu());
                defaultButton.onClick.AddListener(() =>
                {
                    AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmitSound, SFXVolume);
                    SetDefaultValues();
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check last selected object case mouse clicks outside
        /// </summary>
        private void CheckSelectedGameObject()
        {
            if (!panel.activeSelf) return;

            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(resolutionButton.gameObject);
            }
        }

        /// <summary>
        /// Capture B or Esc to close panel
        /// </summary>
        private void CaptureCancelButton()
        {
            if (panel.activeSelf)
            {
                if (InputManager.GetButtonDown(Configuration.InputsNames.UiCancel, PlayerID.One))
                {
                    GetBackToMainMenu();
                }
            }
        }

        /// <summary>
        /// Apply values and save settings, and get back to the main menu
        /// </summary>
        private void GetBackToMainMenu()
        {
            AudioController.Instance.PlaySFX(AudioController.Instance.UiCancelSound, SFXVolume);
            ApplyValues();
            SaveSettings();
            TogglePanel(false);
            MainMenuController.Instance.TogglePanel(true);
        }

        /// <summary>
        /// Enable informed button
        /// </summary>
        /// <param name="button"> Instance of button </param>
        private void ToggleButton(Button button, bool enable)
        {
            AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmitSound, SFXVolume);
            button.interactable = enable;

            if (enable)
            {
                button.Select();
            }

            ToggleButtonColor(button);
            ToggleAllLabelButtonsInteractable();
        }

        private void ToggleAllLabelButtonsInteractable()
        {
            foreach (Button button in labelButtons)
            {
                button.interactable = !button.interactable;
            }

            foreach (Button button in allOtherButtons)
            {
                button.interactable = !button.interactable;
            }
        }

        /// <summary>
        /// Change the button color
        /// </summary>
        /// <param name="button"> Instance of button </param>
        private void ToggleButtonColor(Button button)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = (button.interactable ? Color.yellow : Color.white);
            button.colors = colors;
        }

        /// <summary>
        /// Choose resolution
        /// </summary>
        private void ChooseResolution()
        {
            if (resolutionValueButton.interactable)
            {
                // Horizontal inputs
                if (InputManager.GetButtonDown(Configuration.InputsNames.UiRight, PlayerID.One))
                {
                    resolutionIndex++;
                    resolutionIndex = (resolutionIndex >= resolutions.Count ? 0 : resolutionIndex);
                    resolutionValueButtonLabel.text = resolutions[resolutionIndex];
                    actualResolution = resolutions[resolutionIndex];
                }
                else if (InputManager.GetButtonDown(Configuration.InputsNames.UiLeft, PlayerID.One))
                {
                    resolutionIndex--;
                    resolutionIndex = (resolutionIndex < 0 ? resolutions.Count - 1 : resolutionIndex);
                    resolutionValueButtonLabel.text = resolutions[resolutionIndex];
                    actualResolution = resolutions[resolutionIndex];
                }

                // Apply effect
                if (InputManager.GetButtonDown(Configuration.InputsNames.UiRight, PlayerID.One) || InputManager.GetButtonDown(Configuration.InputsNames.UiLeft, PlayerID.One))
                {
                    string[] values = actualResolution.Split('x');
                    width = int.Parse(values[0]);
                    height = int.Parse(values[1]);
                    ConfigurationsController.SetResolution(width, height, isFullscreen);
                }

                // Case Input is cancel
                if (InputManager.GetButtonDown(Configuration.InputsNames.UiCancel, PlayerID.One))
                {
                    AudioController.Instance.PlaySFX(AudioController.Instance.UiCancelSound, AudioController.Instance.MaxSFXVolume);
                    resolutionValueButton.interactable = false;
                    ToggleButtonColor(resolutionValueButton);
                    ToggleAllLabelButtonsInteractable();
                    resolutionButton.Select();
                }

                // Case click outside
                if (InputManager.GetMouseButtonDown(0) || InputManager.GetMouseButtonDown(1) || InputManager.GetMouseButtonDown(2))
                {
                    resolutionValueButton.Select();
                }
            }
        }

        /// <summary>
        /// Choose full screen
        /// </summary>
        private void ChooseFullScreen()
        {
            if (fullScreenValueButton.interactable)
            {
                // Horizontal inputs
                if (InputManager.GetButtonDown(Configuration.InputsNames.UiRight, PlayerID.One))
                {
                    fullScreenIndex++;
                    fullScreenIndex = (fullScreenIndex >= fullScreenOptions.Length ? 0 : fullScreenIndex);
                    fullScreenValueButtonLabel.text = fullScreenOptions[fullScreenIndex];
                    isFullscreen = (fullScreenOptions[fullScreenIndex] == "Yes");
                }
                else if (InputManager.GetButtonDown(Configuration.InputsNames.UiLeft, PlayerID.One))
                {
                    fullScreenIndex--;
                    fullScreenIndex = (fullScreenIndex < 0 ? fullScreenOptions.Length - 1 : fullScreenIndex);
                    fullScreenValueButtonLabel.text = fullScreenOptions[fullScreenIndex];
                    isFullscreen = (fullScreenOptions[fullScreenIndex] == "Yes");
                }

                // Apply effect
                if (InputManager.GetButtonDown(Configuration.InputsNames.UiRight, PlayerID.One) || InputManager.GetButtonDown(Configuration.InputsNames.UiLeft, PlayerID.One))
                {
                    ConfigurationsController.SetResolution(width, height, isFullscreen);
                }

                // Case Input is cancel
                if (InputManager.GetButtonDown(Configuration.InputsNames.UiCancel, PlayerID.One))
                {
                    AudioController.Instance.PlaySFX(AudioController.Instance.UiCancelSound, AudioController.Instance.MaxSFXVolume);
                    fullScreenValueButton.interactable = false;
                    ToggleButtonColor(fullScreenValueButton);
                    ToggleAllLabelButtonsInteractable();
                    fullScreenButton.Select();
                }

                // Case click outside
                if (InputManager.GetMouseButtonDown(0) || InputManager.GetMouseButtonDown(1) || InputManager.GetMouseButtonDown(2))
                {
                    fullScreenValueButton.Select();
                    return;
                }
            }
        }

        /// <summary>
        /// Choose volume value
        /// </summary>
        /// <param name="labelButton"> Instance of Label Button </param>
        /// <param name="volumeButton"> Instance of Volume Button </param>
        /// <param name="volumeButtonLabel"> Instance of Volume Button Label </param>
        /// <param name="audioSource"> Instance of Audio Source </param>
        private void ChooseVolume(Button labelButton, Button volumeButton, TextMeshProUGUI volumeButtonLabel, AudioSource audioSource)
        {
            if (volumeButton.interactable)
            {
                // Parses
                string volumeText = volumeButtonLabel.text;
                volumeText = volumeText.Replace("%", "");
                float volume = float.Parse(volumeText);

                // Horizontal inputs
                if (InputManager.GetButton(Configuration.InputsNames.UiRight, PlayerID.One))
                {
                    if (volume >= 100) return;

                    timeToWaitUpdateVolume -= Time.fixedDeltaTime;

                    if (timeToWaitUpdateVolume <= 0)
                    {
                        volume++;
                        volume = (volume >= 100 ? 100 : volume);
                        volumeButtonLabel.text = string.Concat(volume, "%");
                        timeToWaitUpdateVolume = startTimeToWaitUpdateVolume;
                    }
                }
                else if (InputManager.GetButton(Configuration.InputsNames.UiLeft, PlayerID.One))
                {
                    if (volume <= 0) return;

                    timeToWaitUpdateVolume -= Time.fixedDeltaTime;

                    if (timeToWaitUpdateVolume <= 0)
                    {
                        volume--;
                        volume = (volume <= 0 ? 0 : volume);
                        volumeButtonLabel.text = string.Concat(volume, "%");
                        timeToWaitUpdateVolume = startTimeToWaitUpdateVolume;
                    }
                }

                // Config
                if (audioSource == AudioController.Instance.AudioSourceBGM)
                {
                    BGMVolume = volume / 100f;
                    ConfigurationsController.SetAudioSourceVolume(AudioController.Instance.AudioSourceBGM, BGMVolume);
                }
                else if (audioSource == AudioController.Instance.AudioSourceSFX)
                {
                    SFXVolume = volume / 100f;
                    ConfigurationsController.SetAudioSourceVolume(AudioController.Instance.AudioSourceSFX, SFXVolume);
                }

                // Plays sound only if it's SFX active
                if (InputManager.GetButton(Configuration.InputsNames.UiLeft, PlayerID.One) ||
                    InputManager.GetButton(Configuration.InputsNames.UiRight, PlayerID.One))
                {
                    if (audioSource == AudioController.Instance.AudioSourceSFX)
                    {
                        timeToPlaySound -= Time.fixedDeltaTime;

                        if (timeToPlaySound <= 0)
                        {
                            AudioController.Instance.PlaySFX(AudioController.Instance.PowerUpSound, SFXVolume);
                            timeToPlaySound = startTimeToPlaySound;
                        }
                    }
                }

                if (InputManager.GetButtonDown(Configuration.InputsNames.UiCancel, PlayerID.One))
                {
                    AudioController.Instance.PlaySFX(AudioController.Instance.UiCancelSound, SFXVolume);
                    volumeButton.interactable = false;
                    ToggleButtonColor(volumeButton);
                    ToggleAllLabelButtonsInteractable();
                    labelButton.Select();
                }

                // Case click outside
                if (InputManager.GetMouseButtonDown(0) || InputManager.GetMouseButtonDown(1) || InputManager.GetMouseButtonDown(2))
                {
                    volumeButton.Select();
                    return;
                }
            }
        }

        /// <summary>
        /// Get list of resolutions
        /// </summary>
        private void GetResolutions()
        {
            resolutions.Clear();
            foreach (Resolution resolution in Screen.resolutions)
            {
                resolutions.Add(string.Concat(resolution.width, "x", resolution.height));
            }
        }

        /// <summary>
        /// Get screen default resolution
        /// </summary>
        /// <returns> Screen default resolution </returns>
        private string GetDefaultResolution() => string.Concat(Screen.currentResolution.width, "x", Screen.currentResolution.height);

        /// <summary>
        /// Reset values to default
        /// </summary>
        private void SetDefaultValues()
        {
            actualResolution = GetDefaultResolution();
            resolutionIndex = resolutions.IndexOf(actualResolution);
            isFullscreen = true;
            BGMVolume = 0.6f;
            SFXVolume = 0.75f;
            UpdateUI();
            ApplyValues();
        }

        /// <summary>
        /// Load settings from PlayerPrefs
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                if (!PlayerPrefsController.HasPlayerPrefs)
                {
                    SetDefaultValues();
                    return;
                }

                actualResolution = PlayerPrefsController.Resolution;
                resolutionIndex = resolutions.IndexOf(actualResolution);
                isFullscreen = (PlayerPrefsController.IsFullScreen == 1);
                BGMVolume = PlayerPrefsController.BackgroundMusicVolume;
                SFXVolume = PlayerPrefsController.SoundEffectsVolume;

                AudioController.Instance.MaxBGMVolume = BGMVolume;
                AudioController.Instance.MaxSFXVolume = SFXVolume;

                UpdateUI();
                ApplyValues();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Save settings to PlayerPrefs
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                PlayerPrefsController.Resolution = actualResolution;
                PlayerPrefsController.IsFullScreen = (isFullscreen ? 1 : 0);
                PlayerPrefsController.BackgroundMusicVolume = BGMVolume;
                PlayerPrefsController.SoundEffectsVolume = SFXVolume;
                PlayerPrefsController.HasPlayerPrefs = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Apply selected values
        /// </summary>
        private void ApplyValues()
        {
            try
            {
                string[] values = actualResolution.Split('x');
                ConfigurationsController.SetResolution(int.Parse(values[0]), int.Parse(values[1]), isFullscreen);

                float divider = (BGMVolume > 1f ? 100f : 1f);
                ConfigurationsController.SetAudioSourceVolume(AudioController.Instance.AudioSourceBGM, (BGMVolume / divider));
                divider = (SFXVolume > 1f ? 100f : 1f);
                ConfigurationsController.SetAudioSourceVolume(AudioController.Instance.AudioSourceSFX, (SFXVolume / divider));

                AudioController.Instance.MaxBGMVolume = BGMVolume;
                AudioController.Instance.MaxSFXVolume = SFXVolume;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Update UI elements
        /// </summary>
        private void UpdateUI()
        {
            try
            {
                resolutionValueButtonLabel.text = actualResolution;
                fullScreenValueButtonLabel.text = (isFullscreen ? LocalizationController.Instance.GetWord(LocalizationFields.general_yes) : LocalizationController.Instance.GetWord(LocalizationFields.general_no));
                bgmVolumeValueButtonLabel.text = string.Concat(BGMVolume * 100, "%");
                sfxVolumeValueButtonLabel.text = string.Concat(SFXVolume * 100, "%");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}