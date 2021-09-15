using Controllers.Core;
using Luminosity.IO;
using MVC.BL;
using MVC.Enums;
using MVC.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Controllers.Menu
{
    public class LanguageMenuController : MonoBehaviour
    {
        [Header("Required UI Elements")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Button[] allButtons;
        [SerializeField] private Button backButton;

        [Header("Other Texts to Translate")]
        [SerializeField] private TextMeshProUGUI header;

        // || State

        private List<string> languages;
        private int currentButtonIndex;
        private string currentLanguage;

        // || Cached

        private TextMeshProUGUI backButtonLabel;
        private LocalizationBL localizationBL;

        // || Properties

        public static LanguageMenuController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            localizationBL = new LocalizationBL();
            languages = localizationBL.GetLanguages().Select(x => x.Language).ToList();

            currentButtonIndex = languages.IndexOf(PlayerPrefsController.Language);

            GetRequiredComponents();
            Translate();
            BindEventListeners();
        }

        private void Update()
        {
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
                backButtonLabel = backButton.GetComponentInChildren<TextMeshProUGUI>();
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
                header.text = LocalizationController.Instance.GetWord(LocalizationFields.mainmenu_language);
                backButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.general_back);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Bind event listeners to elements
        /// </summary>
        private void BindEventListeners()
        {
            try
            {
                for (int index = 0; index < allButtons.Length; index++)
                {
                    int currentIndex = index;
                    allButtons[index].onClick.AddListener(() => ActionButton(currentIndex));
                }

                backButton.onClick.AddListener(() => SaveAndBackToMainMenu());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Save current select language and get back to main menu
        /// </summary>
        private void SaveAndBackToMainMenu()
        {
            PlayerPrefsController.Language = currentLanguage;
            AudioController.Instance.PlaySFX(AudioController.Instance.UiCancelSound, AudioController.Instance.MaxSFXVolume);
            TogglePanel(false);
            MainMenuController.Instance.TogglePanel(true);
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
                allButtons[0].Select();
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
                EventSystem.current.SetSelectedGameObject(allButtons[0].gameObject);
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
                    SaveAndBackToMainMenu();
                }
            }
        }

        /// <summary>
        /// Applies the selected flag button
        /// </summary>
        /// <param name="index"> Button index </param>
        private void ActionButton(int index)
        {
            try
            {
                currentLanguage = languages[index];
                AudioController.Instance.PlaySFX(AudioController.Instance.ClickSound, AudioController.Instance.MaxSFXVolume);
                LocalizationController.Instance.LoadLocalization(currentLanguage);
                Translate();
                InputControllerUI.Instance.Translate();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}