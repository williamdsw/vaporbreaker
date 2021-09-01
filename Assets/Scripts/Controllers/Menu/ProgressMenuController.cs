using System;
using System.Collections;
using System.Collections.Generic;
using Luminosity.IO;
using MVC.Enums;
using MVC.Global;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace Controllers.Menu
{
    public class ProgressMenuController : MonoBehaviour
    {
        // || Inspector References

        [Header("Required UI Elements")]
        [SerializeField] private GameObject panel;
        [SerializeField] private GameObject questionObject;

        [Header("Buttons")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button resetProgressButton;
        [SerializeField] private Button noButton;
        [SerializeField] private Button yesButton;

        [Header("Other Labels to Translate")]
        [SerializeField] private TextMeshProUGUI areYouSureLabel;


        // || Cached

        private TextMeshProUGUI continueButtonLabel;
        private TextMeshProUGUI resetProgressButtonLabel;
        private TextMeshProUGUI noButtonLabel;
        private TextMeshProUGUI yesButtonLabel;

        // || Properties

        public static ProgressMenuController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            GetRequiredComponents();
            Translate();
            BindEventListeners();
        }

        private void Update() => CaptureCancelButton();

        /// <summary>
        /// Get required components
        /// </summary>
        private void GetRequiredComponents()
        {
            try
            {
                continueButtonLabel = continueButton.GetComponentInChildren<TextMeshProUGUI>();
                resetProgressButtonLabel = resetProgressButton.GetComponentInChildren<TextMeshProUGUI>();
                noButtonLabel = noButton.GetComponentInChildren<TextMeshProUGUI>();
                yesButtonLabel = yesButton.GetComponentInChildren<TextMeshProUGUI>();
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
                continueButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.general_continue);
                resetProgressButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.configurations_resetprogress);
                noButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.general_no);
                yesButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.general_yes);
                areYouSureLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.resetprogress_areyousure);
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
                continueButton.onClick.AddListener(() => Continue());
                resetProgressButton.onClick.AddListener(() => AskResetProgress());
                noButton.onClick.AddListener(() => DontResetProgress(false));
                yesButton.onClick.AddListener(() => ResetProgress());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Continue to select levels
        /// </summary>
        private void Continue()
        {
            if (EventSystem.current.currentSelectedGameObject != continueButton.gameObject) return;

            if (MainMenuController.Instance.HasSavedGame && panel.activeSelf)
            {
                AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmit, AudioController.Instance.GetMaxSFXVolume());
                StartCoroutine(MainMenuController.Instance.CallNextScene(SceneManagerController.SelectLevelsSceneName));
            }
        }

        /// <summary>
        /// Ask to reset progress
        /// </summary>
        private void AskResetProgress()
        {
            if (EventSystem.current.currentSelectedGameObject != resetProgressButton.gameObject) return;

            if (MainMenuController.Instance.HasSavedGame && panel.activeSelf)
            {
                continueButton.interactable = resetProgressButton.interactable = false;
                questionObject.SetActive(true);
                noButton.Select();
            }
        }

        /// <summary>
        /// If the user refuses to reset progress
        /// </summary>
        private void DontResetProgress(bool ignoreChecking)
        {
            if (!ignoreChecking)
            {
                if (EventSystem.current.currentSelectedGameObject != noButton.gameObject) return;
            }

            if (questionObject.activeSelf)
            {
                AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmit, AudioController.Instance.GetMaxSFXVolume());
                continueButton.interactable = resetProgressButton.interactable = true;
                questionObject.SetActive(false);
                continueButton.Select();
            }
        }

        /// <summary>
        /// If the user wants to reset progress
        /// </summary>
        private void ResetProgress()
        {
            if (EventSystem.current.currentSelectedGameObject != yesButton.gameObject) return;

            if (questionObject.activeSelf)
            {
                AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmit, AudioController.Instance.GetMaxSFXVolume());
                continueButton.interactable = resetProgressButton.interactable = true;
                ProgressManager.DeleteProgress();
                MainMenuController.Instance.HasSavedGame = false;
                questionObject.SetActive(false);
                MainMenuController.Instance.TogglePanel(true);
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
                continueButton.Select();
                Translate();
            }
        }

        /// <summary>
        /// Capture ESC key or B button
        /// </summary>
        private void CaptureCancelButton()
        {
            try
            {
                if (questionObject.activeSelf)
                {
                    if (InputManager.GetButtonDown(Configuration.InputsNames.UiCancel))
                    {
                        DontResetProgress(true);
                    }

                    return;
                }

                if (panel.activeSelf)
                {
                    if (InputManager.GetButtonDown(Configuration.InputsNames.UiCancel))
                    {
                        AudioController.Instance.PlaySFX(AudioController.Instance.UiCancel, AudioController.Instance.GetMaxSFXVolume());
                        TogglePanel(false);
                        MainMenuController.Instance.TogglePanel(true);
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