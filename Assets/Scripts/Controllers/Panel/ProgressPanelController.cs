using Controllers.Core;
using MVC.BL;
using MVC.Enums;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utilities;

namespace Controllers.Panel
{
    /// <summary>
    /// Controller for Progress Panel
    /// </summary>
    public class ProgressPanelController : MonoBehaviour
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
        private LevelBL levelBL;
        private ScoreboardBL scoreboardBL;

        // || Properties

        public static ProgressPanelController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            levelBL = new LevelBL();
            scoreboardBL = new ScoreboardBL();

            GetRequiredComponents();
            Translate();
            BindEventListeners();
        }

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
                resetProgressButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.options_resetprogress);
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

            if (MainMenuPanelController.Instance.HasSavedGame && panel.activeSelf)
            {
                AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmitSound, AudioController.Instance.MaxSFXVolume);
                StartCoroutine(MainMenuPanelController.Instance.CallNextScene(SceneManagerController.SceneNames.SelectLevels));
            }
        }

        /// <summary>
        /// Ask to reset progress
        /// </summary>
        private void AskResetProgress()
        {
            if (EventSystem.current.currentSelectedGameObject != resetProgressButton.gameObject) return;

            if (MainMenuPanelController.Instance.HasSavedGame && panel.activeSelf)
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
                AudioController.Instance.PlaySFX(AudioController.Instance.UiCancelSound, AudioController.Instance.MaxSFXVolume);
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
                AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmitSound, AudioController.Instance.MaxSFXVolume);
                continueButton.interactable = resetProgressButton.interactable = true;
                scoreboardBL.DeleteAll();
                levelBL.ResetLevels();
                ProgressManager.DeleteProgress();
                questionObject.SetActive(false);
                TogglePanel(false);
                MainMenuPanelController.Instance.HasSavedGame = false;
                MainMenuPanelController.Instance.TogglePanel(true);
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
        /// Get back to previous screen
        /// </summary>
        /// <param name="callbackContext"> Context with parameters </param>
        public void OnCancel(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed && callbackContext.ReadValueAsButton())
            {
                if (questionObject.activeSelf)
                {
                    DontResetProgress(true);
                    return;
                }

                if (panel.activeSelf)
                {
                    AudioController.Instance.PlaySFX(AudioController.Instance.UiCancelSound, AudioController.Instance.MaxSFXVolume);
                    TogglePanel(false);
                    MainMenuPanelController.Instance.TogglePanel(true);
                    return;
                }
            }
        }
    }
}