using Controllers.Menu;
using Core;
using Effects;
using MVC.Enums;
using MVC.Global;
using MVC.Models;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utilities;

namespace Controllers.Core
{
    public class PauseController : MonoBehaviour
    {
        // || Inspector References

        [Header("Pause UI Objects")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button levelsButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Labels to Translate")]
        [SerializeField] private TextMeshProUGUI headerLabel;
        [SerializeField] private TextMeshProUGUI currentTrackNameLabel;

        // || State

        private bool pauseState = false;

        // || Cached

        private TextMeshProUGUI resumeButtonLabel;
        private TextMeshProUGUI restartButtonLabel;
        private TextMeshProUGUI levelsButtonLabel;
        private TextMeshProUGUI mainMenuButtonLabel;

        // || Properties

        public static PauseController Instance { get; private set; }
        public bool CanPause { private get; set; } = true;

        private void Awake()
        {
            Instance = this;
            currentTrackNameLabel.text = string.Empty;
            panel.SetActive(false);

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
                resumeButtonLabel = resumeButton.GetComponentInChildren<TextMeshProUGUI>();
                restartButtonLabel = restartButton.GetComponentInChildren<TextMeshProUGUI>();
                levelsButtonLabel = levelsButton.GetComponentInChildren<TextMeshProUGUI>();
                mainMenuButtonLabel = mainMenuButton.GetComponentInChildren<TextMeshProUGUI>();
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
                headerLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.pause_paused);
                resumeButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.pause_resume);
                restartButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.pause_restart);
                levelsButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.pause_levels);
                mainMenuButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.pause_mainmenu);
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
                resumeButton.onClick.AddListener(() => PauseOrResumeGame());
                restartButton.onClick.AddListener(() => StartCoroutine(ResetLevelCoroutine()));
                levelsButton.onClick.AddListener(() => StartCoroutine(GotoNextScene(SceneManagerController.SelectLevelsSceneName)));
                mainMenuButton.onClick.AddListener(() => StartCoroutine(GotoNextScene(SceneManagerController.MainMenuSceneName)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Pause or resume current game
        /// </summary>
        public void PauseOrResumeGame()
        {
            pauseState = !pauseState;
            panel.SetActive(pauseState);
            GameSessionController.Instance.ActualGameState = (pauseState ? Enumerators.GameStates.PAUSE : Enumerators.GameStates.GAMEPLAY);
            ConfigurationsController.ToggleCursor(pauseState);
        }

        /// <summary>
        /// Shows track info on pause
        /// </summary>
        /// <param name="track"> Instance of Track </param>
        public void SetTrackInfo(Track track) => currentTrackNameLabel.text = string.Format("{0} - {1}", track.Artist, track.Title);

        /// <summary>
        /// Reset current level
        /// </summary>
        private IEnumerator ResetLevelCoroutine()
        {
            GameSessionController.Instance.ActualGameState = Enumerators.GameStates.TRANSITION;
            FadeEffect.Instance.ResetAnimationFunctions();
            FadeEffect.Instance.FadeToLevel();
            yield return new WaitForSecondsRealtime(FadeEffect.Instance.GetFadeOutLength());
            GameStatusController.Instance.IsLevelCompleted = false;
            GameStatusController.Instance.CameFromLevel = false;
            PersistentData.Instance.DestroyInstance();
            GameSessionController.Instance.DestroyInstance();
            SceneManagerController.ReloadScene();
        }

        /// <summary>
        /// Goto to desired scene
        /// </summary>
        /// <param name="sceneName"> Next scene name </param>
        private IEnumerator GotoNextScene(string sceneName)
        {
            GameSessionController.Instance.ActualGameState = Enumerators.GameStates.TRANSITION;
            FadeEffect.Instance.ResetAnimationFunctions();
            FadeEffect.Instance.FadeToLevel();
            yield return new WaitForSecondsRealtime(FadeEffect.Instance.GetFadeOutLength());
            GameStatusController.Instance.IsLevelCompleted = false;
            GameSessionController.Instance.GotoScene(sceneName);
        }

        /// <summary>
        /// Pause or resume current game
        /// </summary>
        /// <param name="callbackContext"> Context with parameters </param>
        public void OnPause(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed && callbackContext.ReadValueAsButton())
            {
                if (CanPause)
                {
                    PauseOrResumeGame();
                }
            }
        }
    }
}