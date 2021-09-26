using Controllers.Core;
using Effects;
using MVC.BL;
using MVC.Enums;
using MVC.Global;
using MVC.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utilities;

namespace Controllers.Menu
{
    public class SelectLevelsController : MonoBehaviour
    {
        // || Inspector References

        [Header("Required TV Elements")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject panel;
        [SerializeField] private Image levelImage;
        [SerializeField] private Sprite noSignalSprite;
        [SerializeField] private Sprite[] levelsSprites;
        [SerializeField] private TextMeshProUGUI recLabel;
        [SerializeField] private TextMeshProUGUI levelOrderLabel;
        [SerializeField] private TextMeshProUGUI levelNameLabel;
        [SerializeField] private TextMeshProUGUI playedLastTimeLabel;

        [Header("Required Side Panel Elements")]
        [SerializeField] private TextMeshProUGUI chooseAnLevelLabel;
        [SerializeField] private TextMeshProUGUI bestScoreLabel;
        [SerializeField] private TextMeshProUGUI bestScoreValueLabel;
        [SerializeField] private TextMeshProUGUI bestTimeLabel;
        [SerializeField] private TextMeshProUGUI bestTimeValueLabel;
        [SerializeField] private Button scoreboardButton;
        [SerializeField] private Button playButton;
        [SerializeField] private TextMeshProUGUI savingLabel;

        // || Config

        private const float TIME_TO_WAIT_AFTER_SAVE = 2f;

        // || State

        private int currentLevelIndex = 0;
        private bool hasPlayerFinishedGame = false;
        private bool backToPreviousScene = false;
        private Enumerators.GameStates actualGameState = Enumerators.GameStates.GAMEPLAY;

        // || Cached

        private PlayerProgress progress;
        private LevelBL levelBL;
        private ScoreboardBL scoreboardBL;
        private List<Level> levels;
        private TextMeshProUGUI scoreboardLabel;
        private TextMeshProUGUI playLabel;

        // || Properties

        public static SelectLevelsController Instance { get; private set; }
        public Enumerators.GameStates ActualGameState { get => actualGameState; set => actualGameState = value; }

        private void Awake()
        {
            Instance = this;
            progress = new PlayerProgress();
            levelBL = new LevelBL();
            scoreboardBL = new ScoreboardBL();
            levels = new List<Level>();

            ConfigurationsController.ToggleCursor(true);

            GetRequiredComponents();
            Translate();
        }

        private void Start()
        {
            AudioController.Instance.ChangeMusic(AudioController.Instance.AllLoopedSongs[1], false, string.Empty, true, false);
            GameStatusController.Instance.HasStartedSong = true;

            Time.timeScale = 1f;

            savingLabel.text = string.Empty;

            LoadProgress();
            CheckIfCameFromLevel();
            LoadLevels();
            UpdateLevelInfo();
        }

        /// <summary>
        /// Get required components
        /// </summary>
        private void GetRequiredComponents()
        {
            try
            {
                scoreboardLabel = scoreboardButton.GetComponentInChildren<TextMeshProUGUI>();
                playLabel = playButton.GetComponentInChildren<TextMeshProUGUI>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Translates the UI
        /// </summary>
        private void Translate()
        {
            try
            {
                chooseAnLevelLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.selectlevels_chooseanlevel);
                bestScoreLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.leveldetails_bestscore);
                bestTimeLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.leveldetails_besttime);
                scoreboardLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.general_scoreboard); // TODO
                playLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.leveldetails_play);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Load current or new progress
        /// </summary>
        private void LoadProgress()
        {
            try
            {
                progress = ProgressManager.LoadProgress();
                currentLevelIndex = progress.CurrentLevelIndex;
                hasPlayerFinishedGame = progress.HasPlayerFinishedGame;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check if previous scene is a level
        /// </summary>
        private void CheckIfCameFromLevel()
        {
            try
            {
                if (!GameStatusController.Instance.CameFromLevel) return;

                currentLevelIndex = GameStatusController.Instance.LevelIndex;

                if (GameStatusController.Instance.IsLevelCompleted)
                {
                    Level current = levelBL.GetById(GameStatusController.Instance.LevelId);
                    Level next = levelBL.GetById(GameStatusController.Instance.LevelId + 1);
                    Level last = levelBL.GetLastLevel();

                    if (current != null && !current.IsCompleted)
                    {
                        levelBL.UpdateIsCompletedById(GameStatusController.Instance.LevelId, true);
                    }

                    Scoreboard scoreboard = new Scoreboard()
                    {
                        LevelId = GameStatusController.Instance.LevelId,
                        Score = GameStatusController.Instance.NewScore,
                        TimeScore = GameStatusController.Instance.NewTimeScore,
                        BestCombo = GameStatusController.Instance.NewCombo,
                        Moment = DateTimeOffset.Now.ToUnixTimeSeconds()
                    };

                    scoreboardBL.Insert(scoreboard);

                    if (next != null && !next.IsUnlocked && !next.IsCompleted)
                    {
                        levelBL.UpdateIsUnlockedById(next.Id, true);
                    }

                    if (!hasPlayerFinishedGame)
                    {
                        hasPlayerFinishedGame = last.IsCompleted;
                    }

                    StartCoroutine(SaveProgress());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Load list of levels
        /// </summary>
        private void LoadLevels() => levels = levelBL.ListAll();

        /// <summary>
        /// Updates level information
        /// </summary>
        private void UpdateLevelInfo()
        {
            try
            {
                Level current = levels[currentLevelIndex];
                List<Scoreboard> scoreboards = scoreboardBL.ListByLevel(current.Id);
                Scoreboard bestScoreboard = scoreboardBL.GetByMaxScoreByLevel(current.Id);
                levelNameLabel.text = (current.IsUnlocked ? current.Name : LocalizationController.Instance.GetWord(LocalizationFields.selectlevels_nosignal));
                levelOrderLabel.text = (currentLevelIndex + 1).ToString("000");
                playedLastTimeLabel.text = (current.IsUnlocked && current.IsCompleted ? Formatter.FormatDateTimer(bestScoreboard.Moment, "hh:mm tt MMM dd yyyy") : string.Empty);
                bestScoreValueLabel.text = (current.IsUnlocked && current.IsCompleted ? Formatter.FormatToCurrency(bestScoreboard.Score) : string.Empty);
                bestTimeValueLabel.text = (current.IsUnlocked && current.IsCompleted ? Formatter.GetEllapsedTimeInHours((int)bestScoreboard.TimeScore) : string.Empty);
                scoreboardButton.gameObject.SetActive(current.IsUnlocked && current.IsCompleted);
                playButton.gameObject.SetActive(current.IsUnlocked);
                recLabel.text = (current.IsUnlocked ? "REC" : string.Empty);
                levelImage.sprite = (current.IsUnlocked ? levelsSprites[currentLevelIndex] : noSignalSprite);

                if (scoreboardButton.isActiveAndEnabled)
                {
                    scoreboardButton.Select();
                    scoreboardButton.onClick.RemoveAllListeners();
                    scoreboardButton.onClick.AddListener(() =>
                    {
                        panel.SetActive(false);
                        AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmitSound, AudioController.Instance.MaxSFXVolume);
                        ScoreboardPanelController.Instance.Show(scoreboards);
                    });
                }
                else if (playButton.interactable)
                {
                    playButton.Select();
                }

                if (playButton.interactable)
                {
                    playButton.onClick.RemoveAllListeners();
                    playButton.onClick.AddListener(() =>
                    {
                        if (ActualGameState != Enumerators.GameStates.GAMEPLAY || !panel.activeSelf) return;

                        canvasGroup.interactable = false;

                        AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmitSound, AudioController.Instance.MaxSFXVolume);
                        AudioController.Instance.StopMusic();

                        // Game status params
                        GameStatusController.Instance.LevelId = current.Id;
                        GameStatusController.Instance.LevelIndex = currentLevelIndex;
                        GameStatusController.Instance.NewScore = 0;
                        GameStatusController.Instance.NewTimeScore = 0;
                        GameStatusController.Instance.NewCombo = 0;
                        GameStatusController.Instance.OldScore = bestScoreboard.Score;
                        GameStatusController.Instance.OldTimeScore = bestScoreboard.TimeScore;
                        GameStatusController.Instance.OldCombo = bestScoreboard.BestCombo;

                        StartCoroutine(CallNextScene(SceneManagerController.LevelSceneName));
                    });
                }

                if (current.IsUnlocked)
                {
                    AudioController.Instance.StopME();
                }
                else
                {
                    AudioController.Instance.PlayME(AudioController.Instance.TvStaticEffect, AudioController.Instance.MaxSFXVolume / 2, true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Call next scene
        /// </summary>
        /// <param name="nextSceneName"> Name of the next scene </param>
        private IEnumerator CallNextScene(string nextSceneName)
        {
            ActualGameState = Enumerators.GameStates.TRANSITION;

            FadeEffect.Instance.FadeToLevel();
            yield return new WaitForSecondsRealtime(FadeEffect.Instance.GetFadeOutLength());

            GameStatusController.Instance.NextSceneName = nextSceneName;
            GameStatusController.Instance.CameFromLevel = false;
            GameStatusController.Instance.HasStartedSong = false;
            SceneManagerController.CallScene(SceneManagerController.LoadingSceneName);
        }

        /// <summary>
        /// Save current progress
        /// </summary>
        private IEnumerator SaveProgress()
        {
            ActualGameState = Enumerators.GameStates.SAVE_LOAD;
            savingLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.selectlevels_saving);
            canvasGroup.interactable = false;

            progress.CurrentLevelIndex = currentLevelIndex;
            progress.HasPlayerFinishedGame = hasPlayerFinishedGame;

            ProgressManager.SaveProgress(progress);

            yield return new WaitForSecondsRealtime(TIME_TO_WAIT_AFTER_SAVE);
            savingLabel.text = string.Empty;
            canvasGroup.interactable = true;
            ActualGameState = Enumerators.GameStates.GAMEPLAY;
        }

        /// <summary>
        /// Show this panel
        /// </summary>
        public void Show()
        {
            panel.SetActive(true);
            scoreboardButton.Select();
        }

        /// <summary>
        /// Change level based on direction
        /// </summary>
        /// <param name="callbackContext"> Context with parameters </param>
        public void ChangeLevel(InputAction.CallbackContext callbackContext)
        {
            try
            {
                if (ActualGameState != Enumerators.GameStates.GAMEPLAY || !panel.activeSelf) return;

                if (callbackContext.performed)
                {
                    Vector2 direction = callbackContext.ReadValue<Vector2>();
                    if (direction == Vector2.right)
                    {
                        KnobEffect.Instance.TurnDirection(NamesTags.AnimatorTriggers.TurnRight);
                        AudioController.Instance.PlaySFX(AudioController.Instance.TvSwitchSound, AudioController.Instance.MaxSFXVolume);
                        currentLevelIndex++;
                        currentLevelIndex = (currentLevelIndex >= levels.Count ? 0 : currentLevelIndex);
                        UpdateLevelInfo();
                    }
                    else if (direction == Vector2.left)
                    {
                        KnobEffect.Instance.TurnDirection(NamesTags.AnimatorTriggers.TurnLeft);
                        AudioController.Instance.PlaySFX(AudioController.Instance.TvSwitchSound, AudioController.Instance.MaxSFXVolume);
                        currentLevelIndex--;
                        currentLevelIndex = (currentLevelIndex < 0 ? levels.Count - 1 : currentLevelIndex);
                        UpdateLevelInfo();
                    }
                }
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
            if (ActualGameState != Enumerators.GameStates.GAMEPLAY || !panel.activeSelf) return;
            if (backToPreviousScene) return;

            if (callbackContext.performed && callbackContext.ReadValueAsButton())
            {
                backToPreviousScene = true;
                AudioController.Instance.StopMusic();
                AudioController.Instance.StopME();
                AudioController.Instance.PlaySFX(AudioController.Instance.UiCancelSound, AudioController.Instance.MaxSFXVolume);
                StartCoroutine(CallNextScene(SceneManagerController.MainMenuSceneName));
            }
        }
    }
}