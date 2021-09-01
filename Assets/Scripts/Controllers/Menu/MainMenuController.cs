using Luminosity.IO;
using MVC.Enums;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace Controllers.Menu
{
    public class MainMenuController : MonoBehaviour
    {
        // || Inspector References

        [Header("Required UI Elements")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject panel;

        [Header("Buttons")]
        [SerializeField] private Button levelsButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button languageButton;
        [SerializeField] private Button soundtrackButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;

        // || Cached

        private TextMeshProUGUI levelsButtonLabel;
        private TextMeshProUGUI optionsButtonLabel;
        private TextMeshProUGUI languageButtonLabel;
        private TextMeshProUGUI soundtrackButtonLabel;
        private TextMeshProUGUI creditsButtonLabel;
        private TextMeshProUGUI quitButtonLabel;

        // || Properties

        public static MainMenuController Instance { get; private set; }
        public bool HasSavedGame { get; set; } = false;

        private void Awake()
        {
            Instance = this;
            ConfigurationsController.ToggleCursor(false);
            GetRequiredComponents();
            Translate();

            AudioController.Instance.ChangeMusic(AudioController.Instance.AllLoopedSongs[0], false, "", true, false);

            // Resets for animation works
            Time.timeScale = 1f;
            HasSavedGame = ProgressManager.HasProgress();

            BindEventListeners();
            InputManager.Load();

            levelsButton.Select();
        }

        private void Update() => CheckSelectedGameObject();

        /// <summary>
        /// Get required components
        /// </summary>
        private void GetRequiredComponents()
        {
            try
            {
                levelsButtonLabel = levelsButton.GetComponentInChildren<TextMeshProUGUI>();
                optionsButtonLabel = optionsButton.GetComponentInChildren<TextMeshProUGUI>();
                languageButtonLabel = languageButton.GetComponentInChildren<TextMeshProUGUI>();
                soundtrackButtonLabel = soundtrackButton.GetComponentInChildren<TextMeshProUGUI>();
                creditsButtonLabel = creditsButton.GetComponentInChildren<TextMeshProUGUI>();
                quitButtonLabel = quitButton.GetComponentInChildren<TextMeshProUGUI>();
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
                levelsButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.pause_levels);
                optionsButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.mainmenu_options);
                languageButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.mainmenu_language);
                soundtrackButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.mainmenu_soundtrack);
                creditsButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.mainmenu_credits);
                quitButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.mainmenu_quit);
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
                levelsButton.onClick.AddListener(() => GotoLevelsOrOpenProgressMenu());
                optionsButton.onClick.AddListener(() => OpenOptionsMenu());
                languageButton.onClick.AddListener(() => OpenLanguageMenu());
                soundtrackButton.onClick.AddListener(() => GotoSoundtracks());
                creditsButton.onClick.AddListener(() => GotoCredits());
                quitButton.onClick.AddListener(() => QuitGame());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Redirects to Credits
        /// </summary>
        private void GotoCredits()
        {
            if (EventSystem.current.currentSelectedGameObject != creditsButton.gameObject) return;

            AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmit, AudioController.Instance.GetMaxSFXVolume());
            StartCoroutine(CallNextScene(SceneManagerController.CreditsSceneName));
        }

        /// <summary>
        /// Redirects to Sountracks
        /// </summary>
        private void GotoSoundtracks()
        {
            if (EventSystem.current.currentSelectedGameObject != soundtrackButton.gameObject) return;

            AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmit, AudioController.Instance.GetMaxSFXVolume());
            StartCoroutine(CallNextScene(SceneManagerController.SoundtracksSceneName));
        }

        /// <summary>
        /// Quits the game
        /// </summary>
        private void QuitGame()
        {
            if (EventSystem.current.currentSelectedGameObject != quitButton.gameObject) return;

            SceneManagerController.QuitGame();
        }

        /// <summary>
        /// Open language selection menu
        /// </summary>
        private void OpenLanguageMenu()
        {
            if (EventSystem.current.currentSelectedGameObject != languageButton.gameObject) return;

            AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmit, AudioController.Instance.GetMaxSFXVolume());
            TogglePanel(false);
            LanguageMenuController.Instance.TogglePanel(true);
        }

        /// <summary>
        /// Open options menu
        /// </summary>
        private void OpenOptionsMenu()
        {
            if (EventSystem.current.currentSelectedGameObject != optionsButton.gameObject) return;

            AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmit, AudioController.Instance.GetMaxSFXVolume());
            TogglePanel(false);
            OptionsMenuController.Instance.TogglePanel(true);
        }

        /// <summary>
        /// Check save progress to open menu or redirect to level selection
        /// </summary>
        private void GotoLevelsOrOpenProgressMenu()
        {
            if (EventSystem.current.currentSelectedGameObject != levelsButton.gameObject) return;

            AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmit, AudioController.Instance.GetMaxSFXVolume());
            if (HasSavedGame)
            {
                TogglePanel(false);
                ProgressMenuController.Instance.TogglePanel(true);
            }
            else
            {
                StartCoroutine(CallNextScene(SceneManagerController.SelectLevelsSceneName));
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
                levelsButton.Select();
                Translate();
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
                EventSystem.current.SetSelectedGameObject(levelsButton.gameObject);
            }
        }

        /// <summary>
        /// Call the next scene
        /// </summary>
        /// <param name="nextSceneName"> Next Scene </param>
        public IEnumerator CallNextScene(string nextSceneName)
        {
            AudioController.Instance.StopMusic();

            canvasGroup.interactable = false;

            FadeEffect.Instance.FadeToLevel();
            yield return new WaitForSecondsRealtime(FadeEffect.Instance.GetFadeOutLength());
            GameStatusController.Instance.NextSceneName = nextSceneName;
            GameStatusController.Instance.CameFromLevel = false;
            SceneManagerController.CallScene(SceneManagerController.LoadingSceneName);
        }
    }
}