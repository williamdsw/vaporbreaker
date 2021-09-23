using Controllers.Core;
using Luminosity.IO;
using MVC.Enums;
using MVC.Global;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Controllers.Menu
{
    public class OptionsMenuController : MonoBehaviour
    {
        // || Inspector References

        [Header("Required UI Elements")]
        [SerializeField] private GameObject panel;

        [Header("Label Buttons")]
        [SerializeField] private Button bgmVolumeButton;
        [SerializeField] private Button meVolumeButton;
        [SerializeField] private Button sfxVolumeButton;

        [Header("Value Buttons")]
        [SerializeField] private Button bgmVolumeValueButton;
        [SerializeField] private Button meVolumeValueButton;
        [SerializeField] private Button sfxVolumeValueButton;

        [Header("Other Buttons")]
        [SerializeField] private Button backButton;
        [SerializeField] private Button defaultButton;
        [SerializeField] private Button[] labelButtons;
        [SerializeField] private Button[] allOtherButtons;

        [Header("Other Texts to Translate")]
        [SerializeField] private TextMeshProUGUI header;

        // || Config

        private float timeToWaitUpdateVolume = 0.1f;
        private readonly float startTimeToWaitUpdateVolume = 0.1f;
        private float timeToPlaySound = 1f;
        private readonly float startTimeToPlaySound = 1f;

        // || State

        private float BGMVolume = 1f;
        private float MEVolume = 1f;
        private float SFXVolume = 1f;

        // || Cached

        private TextMeshProUGUI bgmVolumeButtonLabel;
        private TextMeshProUGUI meVolumeButtonLabel;
        private TextMeshProUGUI sfxVolumeButtonLabel;
        private TextMeshProUGUI bgmVolumeValueButtonLabel;
        private TextMeshProUGUI meVolumeValueButtonLabel;
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
            LoadSettings();
        }

        private void Update()
        {
            ChooseVolume(bgmVolumeButton, bgmVolumeValueButton, bgmVolumeValueButtonLabel, AudioController.Instance.AudioSourceBGM);
            ChooseVolume(meVolumeButton, meVolumeValueButton, meVolumeValueButtonLabel, AudioController.Instance.AudioSourceME);
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
                bgmVolumeButtonLabel = bgmVolumeButton.GetComponentInChildren<TextMeshProUGUI>();
                meVolumeButtonLabel = meVolumeButton.GetComponentInChildren<TextMeshProUGUI>();
                sfxVolumeButtonLabel = sfxVolumeButton.GetComponentInChildren<TextMeshProUGUI>();
                bgmVolumeValueButtonLabel = bgmVolumeValueButton.GetComponentInChildren<TextMeshProUGUI>();
                meVolumeValueButtonLabel = meVolumeValueButton.GetComponentInChildren<TextMeshProUGUI>();
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
                bgmVolumeButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.options_backgroundvolume);
                meVolumeButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.options_musiceffectsvolume);
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
                bgmVolumeButton.Select();
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
                bgmVolumeButton.onClick.AddListener(() => ToggleButton(bgmVolumeValueButton, true));
                meVolumeButton.onClick.AddListener(() => ToggleButton(meVolumeValueButton, true));
                sfxVolumeButton.onClick.AddListener(() => ToggleButton(sfxVolumeValueButton, true));

                bgmVolumeValueButton.onClick.AddListener(() =>
                {
                    ToggleButton(bgmVolumeValueButton, false);
                    bgmVolumeButton.Select();
                    AudioController.Instance.AudioSourceBGM.volume = BGMVolume;
                });

                meVolumeValueButton.onClick.AddListener(() =>
                {
                    ToggleButton(meVolumeValueButton, false);
                    meVolumeButton.Select();
                    ApplyValues();
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
                EventSystem.current.SetSelectedGameObject(bgmVolumeButton.gameObject);
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

        /// <summary>
        /// Toggle all other buttons
        /// </summary>
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
                else if (audioSource == AudioController.Instance.AudioSourceME)
                {
                    MEVolume = volume / 100f;
                    ConfigurationsController.SetAudioSourceVolume(AudioController.Instance.AudioSourceME, MEVolume);
                }

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
                    else if (audioSource == AudioController.Instance.AudioSourceME)
                    {
                        timeToPlaySound -= Time.fixedDeltaTime;

                        if (timeToPlaySound <= 0)
                        {
                            AudioController.Instance.PlaySFX(AudioController.Instance.NewScoreEffect, MEVolume);
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

                if (InputManager.GetMouseButtonDown(0) || InputManager.GetMouseButtonDown(1) || InputManager.GetMouseButtonDown(2))
                {
                    volumeButton.Select();
                    return;
                }
            }
        }

        /// <summary>
        /// Reset values to default
        /// </summary>
        private void SetDefaultValues()
        {
            BGMVolume = 0.6f;
            SFXVolume = 0.75f;
            MEVolume = 0.5f;
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

                AudioController.Instance.MaxBGMVolume = BGMVolume = PlayerPrefsController.BackgroundMusicVolume;
                AudioController.Instance.MaxMEVolume = MEVolume = PlayerPrefsController.MusicEffectsVolume;
                AudioController.Instance.MaxSFXVolume = SFXVolume = PlayerPrefsController.SoundEffectsVolume;

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
                PlayerPrefsController.BackgroundMusicVolume = BGMVolume;
                PlayerPrefsController.MusicEffectsVolume = MEVolume;
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
                ConfigurationsController.SetAudioSourceVolume(AudioController.Instance.AudioSourceBGM, (BGMVolume / (BGMVolume > 1f ? 100f : 1f)));
                ConfigurationsController.SetAudioSourceVolume(AudioController.Instance.AudioSourceME, (MEVolume / (MEVolume > 1f ? 100f : 1f)));
                ConfigurationsController.SetAudioSourceVolume(AudioController.Instance.AudioSourceSFX, (SFXVolume / (SFXVolume > 1f ? 100f : 1f)));

                AudioController.Instance.MaxBGMVolume = BGMVolume;
                AudioController.Instance.MaxMEVolume = MEVolume;
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
                bgmVolumeValueButtonLabel.text = string.Concat(BGMVolume * 100, "%");
                meVolumeValueButtonLabel.text = string.Concat(MEVolume * 100, "%");
                sfxVolumeValueButtonLabel.text = string.Concat(SFXVolume * 100, "%");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}