using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject[] allMenus;

    [Header("All Menu Buttons")]
    [SerializeField] private Button[] allMenuButtons;

    [Header("Default Menu Buttons")]
    [SerializeField] private Button[] allDefaultButtons;

    [Header("Other UI")]
    [SerializeField] private TextMeshProUGUI actualMusicNameText;

    [Header("Labels to Translate")]
    [SerializeField] private List<TextMeshProUGUI> uiLabels = new List<TextMeshProUGUI>();

    // State
    private bool canPause = true;
    private bool pauseState = false;
    private string actualSongName = "";
    private string previousSongName = "";

    // Cached
    private AudioController audioController;
    private CursorController cursorController;
    private FadeEffect fadeEffect;
    private FullScreenBackground fullScreenBackground;
    private GameSession gameSession;
    private GameStatusController gameStatusController;
    private LocalizationController localizationController;

    public bool GetCanPause()
    {
        return canPause;
    }

    public void SetCanPause(bool canPause)
    {
        this.canPause = canPause;
    }

    public void SetPreviousSongName(string previousSongName)
    {
        this.previousSongName = previousSongName;
    }

    public void SetActualSongName(string actualSongName)
    {
        this.actualSongName = actualSongName;
    }

    private void Start()
    {
        // Finding objects
        audioController = FindObjectOfType<AudioController>();
        cursorController = FindObjectOfType<CursorController>();
        fadeEffect = FindObjectOfType<FadeEffect>();
        fullScreenBackground = FindObjectOfType<FullScreenBackground>();
        gameSession = FindObjectOfType<GameSession>();
        gameStatusController = FindObjectOfType<GameStatusController>();
        localizationController = FindObjectOfType<LocalizationController>();

        actualMusicNameText.text = string.Empty;

        foreach (GameObject menu in allMenus)
        {
            menu.SetActive(false);
        }

        TranslateLabels();
        BindButtonClickEvents();
    }

    private void Update()
    {
        if (canPause)
        {
            if (InputManager.GetButtonDown("Pause"))
            {
                PauseGame();
            }

            if (actualSongName != previousSongName)
            {
                actualMusicNameText.text = actualSongName;
            }
        }
    }

    // Translate labels based on choosed language
    private void TranslateLabels()
    {
        if (!localizationController) return;
        List<string> labels = localizationController.GetPauseLabels();
        if (labels.Count == 0 || uiLabels.Count == 0 || labels.Count != uiLabels.Count) return;
        for (int index = 0; index < labels.Count; index++)
        {
            uiLabels[index].SetText(labels[index]);
        }
    }

    private void BindButtonClickEvents()
    {
        if (allMenuButtons.Length == 0) return;

        // Back Button
        allMenuButtons[0].onClick.AddListener(() => PauseGame());

        // Select Levels Button
        allMenuButtons[1].onClick.AddListener(() => StartCoroutine(ResetGameCoroutine(SceneManagerController.GetSelectLevelsSceneName())));

        // Main Menu Button
        allMenuButtons[2].onClick.AddListener(() => StartCoroutine(ResetGameCoroutine(SceneManagerController.GetMainMenuSceneName())));
    }

    public void PauseGame()
    {
        // State
        pauseState = !pauseState;
        foreach (GameObject menu in allMenus)
        {
            menu.SetActive(pauseState);
        }

        // Config
        if (pauseState)
        {
            audioController.PlaySFX(audioController.UiCancel, audioController.GetMaxSFXVolume());
            gameSession.SetActualGameState(GameState.PAUSE);
            allDefaultButtons[0].Select();
        }
        else
        {
            gameSession.SetActualGameState(GameState.GAMEPLAY);
        }
    }

    public void MakeSelectOnPointerEnter(Button button)
    {
        if (!button || !button.interactable) return;
        button.Select();
    }

    // Waits to fade out to reset game
    private IEnumerator ResetGameCoroutine(string sceneName)
    {
        gameSession.SetActualGameState(GameState.TRANSITION);

        // Fades Out
        if (!fadeEffect)
        {
            fadeEffect = FindObjectOfType<FadeEffect>();
        }

        fadeEffect.ResetAnimationFunctions();
        float fadeOutLength = fadeEffect.GetFadeOutLength();
        fadeEffect.FadeToLevel();
        yield return new WaitForSecondsRealtime(fadeOutLength);
        gameStatusController.SetIsLevelCompleted(false);
        gameSession.ResetGame(sceneName);
    }
}