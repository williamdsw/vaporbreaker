using Controllers.Core;
using Luminosity.IO;
using MVC.Global;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class SelectLevelMenu : MonoBehaviour
{
    // Config Parameters
    [SerializeField] private Button levelScreenButton;
    [SerializeField] private Sprite[] levelsSprites;

    [Header("Level's Information Texts")]
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private GameObject savingText;

    [Header("TV Information Texts")]
    [SerializeField] private TextMeshProUGUI recText;
    [SerializeField] private TextMeshProUGUI channelText;
    [SerializeField] private TextMeshProUGUI tvTimeText;
    [SerializeField] private TextMeshProUGUI tvBestTimeText;

    [Header("Confirmation Elements")]
    [SerializeField] private GameObject questionText;
    [SerializeField] private Button noButton;
    [SerializeField] private Button yesButton;

    [Header("Labels to Translate")]
    [SerializeField] private List<TextMeshProUGUI> uiLabels = new List<TextMeshProUGUI>();
    private List<string> levelInformationLabels = new List<string>();
    private List<string> levelNames = new List<string>();

    // Config
    [SerializeField] private int currentLevelIndex = 0;
    [SerializeField] private List<string> levelsNamesList;
    [SerializeField] private List<bool> isLevelUnlockedList;
    [SerializeField] private List<bool> isLevelCompletedList;
    [SerializeField] private List<int> highScoresList;
    [SerializeField] private List<int> highTimeScoresList;
    [SerializeField] private List<DateTime?> lastTimePlayedList;
    private int defaultTime = 0;
    private int defaultScore = 0;
    private float timeToWaitAfterSave = 2f;
    private GameState actualGameState = GameState.GAMEPLAY;

    // Cached Components
    private Image levelScreenImage;
    private AnimationEffect animationEffect;

    // Cached Others
    private AudioController audioController;
    private FadeEffect fadeEffect;
    private GameStatusController gameStatusController;
    private KnobEffect knobEffect;
    private LocalizationController localizationController;
    private PlayerProgress progress = new PlayerProgress();

    private void Start()
    {
        // Components
        levelScreenImage = levelScreenButton.GetComponent<Image>();
        animationEffect = levelScreenButton.GetComponent<AnimationEffect>();

        // Other
        audioController = FindObjectOfType<AudioController>();
        fadeEffect = FindObjectOfType<FadeEffect>();
        gameStatusController = FindObjectOfType<GameStatusController>();
        knobEffect = FindObjectOfType<KnobEffect>();
        localizationController = FindObjectOfType<LocalizationController>();

        // Play music
        audioController.ChangeMusic(audioController.AllLoopedSongs[1], false, "", true, false);
        gameStatusController.SetHasStartedSong(true);

        LoadProgress();
        VerifyIfCameFromLevel();

        // Default
        levelScreenButton.Select();

        // Resets for animation works
        Time.timeScale = 1f;
        Cursor.visible = true;

        TranslateLabels();
        BindingEvents();
        UpdateUI();
    }

    private void Update()
    {
        // Cancels
        if (actualGameState != GameState.GAMEPLAY) return;

        ChangeLevel();
        CaptureCancelButton();
    }

    private void LoadProgress()
    {
        progress = ProgressManager.LoadProgress();

        // Getting values
        currentLevelIndex = progress.GetCurrentLevelIndex();
        levelsNamesList = progress.GetLevelNamesList();
        isLevelUnlockedList = progress.GetIsLevelUnlockedList();
        isLevelCompletedList = progress.GetIsLevelCompletedList();
        highScoresList = progress.GetHighScoresList();
        highTimeScoresList = progress.GetHighTimeScoresList();
        lastTimePlayedList = progress.GetLastTimePlayedList();
    }

    // Binds button's click event
    private void BindingEvents()
    {
        // Cancel
        if (!levelScreenButton || !questionText || !noButton || !yesButton) return;

        // Click on image
        levelScreenButton.onClick.AddListener(() =>
        {
            if (actualGameState != GameState.GAMEPLAY) return;

            if (isLevelUnlockedList[currentLevelIndex])
            {
                audioController.PlaySFX(audioController.UiSubmit, audioController.MaxSFXVolume);
                levelScreenButton.interactable = false;
                questionText.SetActive(true);
                noButton.Select();
            }
        });

        // Question Buttons
        noButton.onClick.AddListener(() =>
        {
            if (actualGameState != GameState.GAMEPLAY) return;

            audioController.PlaySFX(audioController.UiCancel, audioController.MaxSFXVolume);
            questionText.SetActive(false);
            levelScreenButton.interactable = true;
            levelScreenButton.Select();
        });

        // Question Buttons
        yesButton.onClick.AddListener(() =>
        {
            if (actualGameState != GameState.GAMEPLAY) return;

            // Hides
            questionText.SetActive(false);

            // Params
            audioController.PlaySFX(audioController.UiSubmit, audioController.MaxSFXVolume);
            audioController.StopMusic();

            // Game status params
            gameStatusController.SetHasStartedSong(false);
            gameStatusController.SetCameFromLevel(true);
            gameStatusController.SetLevelIndex(currentLevelIndex);
            gameStatusController.SetIsLevelCompleted(false);
            gameStatusController.SetOldScore(highScoresList[currentLevelIndex]);
            gameStatusController.SetOldTimeScore(highTimeScoresList[currentLevelIndex]);

            string sceneName = levelsNamesList[currentLevelIndex];
            sceneName = sceneName.Replace(" ", "_");
            StartCoroutine(CallNextScene(sceneName));
        });
    }

    // Translate labels based on choosed language
    private void TranslateLabels()
    {
        if (!localizationController) return;

        List<string> labels = localizationController.GetSelectLevelsLabels();
        levelInformationLabels = localizationController.GetLevelInformationLabels();
        levelNames = localizationController.GetLevelsNames();
        if (labels.Count == 0 || uiLabels.Count == 0) return;
        for (int index = 0; index < labels.Count; index++)
        {
            uiLabels[index].SetText(labels[index]);
        }
    }

    // Change level on left / right
    private void ChangeLevel()
    {
        if (!levelScreenButton.interactable) return;

        // Cancels
        if (levelsNamesList.Count == 0 || isLevelUnlockedList.Count == 0 ||
            highScoresList.Count == 0 || highTimeScoresList.Count == 0 ||
            levelsSprites.Length == 0) return;

        if (InputManager.GetButtonDown(Configuration.InputsNames.UiRight))
        {
            knobEffect.TurnDirection("Turn_Right");
            audioController.PlaySFX(audioController.TvSwitch, audioController.MaxSFXVolume);
            currentLevelIndex++;
            currentLevelIndex = (currentLevelIndex >= levelsNamesList.Count ? 0 : currentLevelIndex);
            UpdateUI();
        }
        else if (InputManager.GetButtonDown(Configuration.InputsNames.UiLeft))
        {
            knobEffect.TurnDirection("Turn_Left");
            audioController.PlaySFX(audioController.TvSwitch, audioController.MaxSFXVolume);
            currentLevelIndex--;
            currentLevelIndex = (currentLevelIndex < 0 ? levelsNamesList.Count - 1 : currentLevelIndex);
            UpdateUI();
        }
    }

    // Updates the UI values
    private void UpdateUI()
    {
        // Cancels
        if (!levelNameText || !bestScoreText || !bestTimeText || !tvTimeText || !tvBestTimeText) return;

        // Set texts
        string levelName = (isLevelUnlockedList[currentLevelIndex] ? levelNames[currentLevelIndex] : "??????????");
        levelNameText.text = levelName;
        channelText.text = (currentLevelIndex + 1).ToString("00");

        // Values and effects
        if (isLevelUnlockedList[currentLevelIndex])
        {
            if (isLevelCompletedList[currentLevelIndex])
            {
                bestScoreText.text = string.Concat(levelInformationLabels[0], " ", highScoresList[currentLevelIndex]);
                bestTimeText.text = string.Concat(levelInformationLabels[1], " ", Formatter.FormatEllapsedTime(highTimeScoresList[currentLevelIndex]));
                string hour = lastTimePlayedList[currentLevelIndex].Value.ToString("hh:mm tt");
                string monthDayYear = lastTimePlayedList[currentLevelIndex].Value.ToString("MMM dd yyyy");
                tvTimeText.text = string.Concat(hour, " ", monthDayYear);
                tvBestTimeText.text = Formatter.FormatEllapsedTime(highTimeScoresList[currentLevelIndex]);
            }
            else
            {
                bestScoreText.text = bestTimeText.text = tvBestTimeText.text = tvTimeText.text = string.Empty;
            }

            levelScreenImage.sprite = levelsSprites[currentLevelIndex];
            animationEffect.enabled = false;
            recText.text = "REC";
            audioController.StopME();
        }
        else
        {
            bestScoreText.text = bestTimeText.text = recText.text = tvBestTimeText.text = 
            tvTimeText.text = string.Empty;
            levelScreenImage.sprite = null;
            animationEffect.enabled = true;
            audioController.PlayME(audioController.TvStatic, audioController.MaxSFXVolume / 2, true);
        }
    }

    // Capture Cancel Button on situations
    private void CaptureCancelButton()
    {
        // Leave to Main Menu
        if (levelScreenButton.interactable)
        {
            if (InputManager.GetButtonDown(Configuration.InputsNames.UiCancel))
            {
                audioController.StopMusic();
                audioController.StopME();
                audioController.PlaySFX(audioController.UiCancel, audioController.MaxSFXVolume);
                gameStatusController.SetIsLevelCompleted(false);
                StartCoroutine(CallNextScene(SceneManagerController.MainMenuSceneName));
            }
        }

        // Leave question (Equals to "No Button")
        if (questionText.activeSelf)
        {
            if (InputManager.GetButtonDown(Configuration.InputsNames.UiCancel))
            {
                audioController.PlaySFX(audioController.UiCancel, audioController.MaxSFXVolume);
                questionText.SetActive(false);
                levelScreenButton.interactable = true;
                levelScreenButton.Select();
            }
        }
    }

    private void VerifyIfCameFromLevel()
    {
        // Cancels
        if (!gameStatusController.GetCameFromLevel()) return;

        currentLevelIndex = gameStatusController.GetLevelIndex();

        // Status
        if (gameStatusController.GetIsLevelCompleted())
        {
            // Score
            if (gameStatusController.GetNewScore() > gameStatusController.GetOldScore())
            {
                highScoresList[currentLevelIndex] = (int) gameStatusController.GetNewScore();
            }
            else
            {
                highScoresList[currentLevelIndex] = (int) gameStatusController.GetOldScore();
            }

            // Old Score
            if (gameStatusController.GetOldTimeScore() == defaultTime)
            {
                highTimeScoresList[currentLevelIndex] = (int) gameStatusController.GetNewTimeScore();
            }
            else
            {
                if (gameStatusController.GetNewTimeScore() < gameStatusController.GetOldTimeScore())
                {
                    highTimeScoresList[currentLevelIndex] = (int) gameStatusController.GetNewTimeScore();
                }
                else
                {
                    highTimeScoresList[currentLevelIndex] = (int) gameStatusController.GetOldTimeScore();
                }
            }

            // Level Completed
            if (!isLevelCompletedList[currentLevelIndex])
            {
                isLevelCompletedList[currentLevelIndex] = true;
            }

            // Last Time Played
            lastTimePlayedList[currentLevelIndex] = DateTime.Now;

            // Enable next stage
            if ((currentLevelIndex + 1) < levelsNamesList.Count)
            {
                if (!isLevelCompletedList[currentLevelIndex + 1])
                {
                    isLevelUnlockedList[currentLevelIndex + 1] = true;
                }
            }

            StartCoroutine(SaveProgress());
        }
    }

    public void MakeSelectOnPointerEnter(Button button)
    {
        if (!button || !button.interactable) return;
        button.Select();
    }

    // Wait fade out length to fade out to next scene
    private IEnumerator CallNextScene(string nextSceneName)
    {
        actualGameState = GameState.TRANSITION;

        // Fade Out effect
        float fadeOutLength = fadeEffect.GetFadeOutLength();
        fadeEffect.FadeToLevel();
        yield return new WaitForSecondsRealtime(fadeOutLength);
        gameStatusController.SetNextSceneName(nextSceneName);
        SceneManagerController.CallScene(SceneManagerController.GetLoadingSceneName());
    }

    // Save progress
    private IEnumerator SaveProgress()
    {
        actualGameState = GameState.SAVE_LOAD;
        savingText.SetActive(true);

        // Passing values
        progress.SetCurrentLevelIndex(currentLevelIndex);
        progress.SetIsLevelUnlockedList(isLevelUnlockedList);
        progress.SetIsLevelCompletedList(isLevelCompletedList);
        progress.SetHighScoresList(highScoresList);
        progress.SetHighTimeScoresList(highTimeScoresList);
        progress.SetLastTimePlayedList(lastTimePlayedList);

        // Saves
        ProgressManager.SaveProgress(progress);

        // Waits and return
        yield return new WaitForSecondsRealtime(timeToWaitAfterSave);
        savingText.SetActive(false);
        levelScreenButton.Select();
        actualGameState = GameState.GAMEPLAY;
    }
}