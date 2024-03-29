﻿using Controllers.Core;
using MVC.BL;
using MVC.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Controllers.Panel
{
    /// <summary>
    /// Controller for Language Panel
    /// </summary>
    public class LanguagePanelController : MonoBehaviour
    {
        // || Inspector References

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

        // || Properties

        public static LanguagePanelController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            languages = new LocalizationBL().ListAll().Select(x => x.Language).ToList();

            currentButtonIndex = languages.IndexOf(PlayerPrefsController.Language);

            GetRequiredComponents();
            Translate();
            BindEventListeners();
        }

        private void Update() => CheckSelectedGameObject();

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
            MainMenuPanelController.Instance.TogglePanel(true);
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get back to previous screen
        /// </summary>
        /// <param name="callbackContext"> Context with parameters </param>
        public void OnCancel(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed && callbackContext.ReadValueAsButton())
            {
                if (panel.activeSelf)
                {
                    SaveAndBackToMainMenu();
                }
            }
        }
    }
}