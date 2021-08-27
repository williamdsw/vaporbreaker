using System;
using System.Collections;
using System.Collections.Generic;
using Luminosity.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

public class MusicPlayer : MonoBehaviour
{
    // Config 
    [Header("Music Informations")]
    [SerializeField] private TextMeshProUGUI artistNameText;
    [SerializeField] private TextMeshProUGUI songNameText;
    [SerializeField] private TextMeshProUGUI songDurationText;
    [SerializeField] private TextMeshProUGUI hourText;

    [Header("Music Buttons")]
    [SerializeField] private Button[] musicControllerButtons;

    [Header("Cursors")]
    [SerializeField] private Texture2D defaultGameCursor;
    [SerializeField] private Texture2D oldPointerCursor;
    [SerializeField] private Texture2D oldHandCursor;

    [Header("Labels to Translate")]
    [SerializeField] private List<TextMeshProUGUI> uiLabels = new List<TextMeshProUGUI>();

    // State
    private GameState actualGameState = GameState.GAMEPLAY;
    private int currentButtonIndex = 0;
    private int currentSongIndex = 0;
    private bool isSongPaused = false;
    private bool isSongRepeated = false;
    private bool canEllapseTime = false;
    private float songEllapsedTime = 0f;
    private float songDuration = 0f;

    // Input control
    private bool canChangeMusicInGame = true;
    private bool canCalculate = false;
    private float timeToUnlockButton = 1f;
    private float startTimeToUnlockButton = 1f;

    // Cached Components
    private Image repeatButtonImage;
    private TextMeshProUGUI pauseResumeText;

    // Cached Other Objects
    private AudioController audioController;
    private FadeEffect fadeEffect;
    private GameStatusController gameStatusController;
    private LocalizationController localizationController;

    private void Start()
    {
        // Components
        pauseResumeText = musicControllerButtons[2].GetComponentInChildren<TextMeshProUGUI>();
        repeatButtonImage = musicControllerButtons[3].GetComponentInChildren<Image>();

        // Other
        audioController = FindObjectOfType<AudioController>();
        fadeEffect = FindObjectOfType<FadeEffect>();
        gameStatusController = FindObjectOfType<GameStatusController>();
        localizationController = FindObjectOfType<LocalizationController>();

        // Play music
        audioController.ChangeMusic(audioController.AllLoopedSongs[0], false, "", true, false);

        // Resets for animation works
        Time.timeScale = 1f;
        musicControllerButtons[0].Select();

        TranslateLabels();
        UpdateUI();
        ChangeCursor(false);
    }

    private void Update()
    {
        if (actualGameState == GameState.GAMEPLAY)
        {
            CaptureInputs();

            if (audioController.GetIsSongPlaying() && !isSongPaused && canEllapseTime)
            {
                ShowEllapedSongTime();
            }

            // Time to unlock buttons
            if (canCalculate)
            {
                timeToUnlockButton -= Time.fixedDeltaTime;
                if (timeToUnlockButton <= 0)
                {
                    timeToUnlockButton = startTimeToUnlockButton;
                    canCalculate = false;
                    canChangeMusicInGame = true;
                }
            }

            UpdateHourText();
        }
    }

    // Translate labels based on choosed language
    private void TranslateLabels()
    {
        // CANCELS
        if (!localizationController) return;
        List<string> labels = localizationController.GetSoundtracksLabels();
        if (labels.Count == 0 || uiLabels.Count == 0) return;
        for (int index = 0; index < labels.Count; index++)
        {
            uiLabels[index].SetText(labels[index]);
        }
    }

    // Capture User Inputs
    private void CaptureInputs()
    {
        // Cancels 
        if (musicControllerButtons.Length == 0) return;

        // Right / Left
        if (InputManager.GetButtonDown("UI_Right"))
        {
            currentButtonIndex++;
            currentButtonIndex = (currentButtonIndex >= musicControllerButtons.Length ? 0 : currentButtonIndex);
            musicControllerButtons[currentButtonIndex].Select();
        }
        else if (InputManager.GetButtonDown("UI_Left"))
        {
            currentButtonIndex--;
            currentButtonIndex = (currentButtonIndex < 0 ? musicControllerButtons.Length - 1 : currentButtonIndex);
            musicControllerButtons[currentButtonIndex].Select();
        }

        // Submit
        if (EventSystem.current.currentSelectedGameObject)
        {
            if (InputManager.GetButtonDown("UI_Submit"))
            {
                ActionButton(currentButtonIndex);
            }
        }
    }

    // Sets the action for button click
    private void ActionButton(int index)
    {
        // Play SFX
        audioController.PlaySFX(audioController.ClickSound, audioController.GetMaxSFXVolume());

        switch (currentButtonIndex)
        {
            // PREVIOUS
            case 0:
            {
                if (canChangeMusicInGame)
                {
                    // bools 
                    canChangeMusicInGame = false;
                    canCalculate = true;
                    currentSongIndex--;
                    currentSongIndex = (currentSongIndex < 0 ? audioController.AllNotLoopedSongs.Length - 1 : currentSongIndex);
                    PlaySong(currentSongIndex);
                }

                break;
            }

            // PLAY
            case 1:
            {
                if (canChangeMusicInGame)
                {
                    // bools 
                    canChangeMusicInGame = false;
                    canCalculate = true;
                    PlaySong(currentSongIndex);
                }

                break;
            }

            // PAUSE / RESUME
            case 2:
            {
                if (audioController.GetIsSongPlaying())
                {
                    isSongPaused = !isSongPaused;
                    audioController.PauseMusic(isSongPaused);
                    if (!pauseResumeText) return;
                    pauseResumeText.text = (isSongPaused ? "Resume" : "Pause");
                }

                break;
            }

            // REPEAT
            case 3:
            {
                isSongRepeated = !isSongRepeated;
                audioController.RepeatMusic(isSongRepeated);
                if (!repeatButtonImage) return;
                repeatButtonImage.color = (isSongRepeated ? Color.yellow : Color.white);
                break;
            }

            // NEXT
            case 4:
            {
                if (canChangeMusicInGame)
                {
                    // bools 
                    canChangeMusicInGame = false;
                    canCalculate = true;
                    currentSongIndex++;
                    currentSongIndex = (currentSongIndex >= audioController.AllNotLoopedSongs.Length ? 0 : currentSongIndex);
                    PlaySong(currentSongIndex);
                }

                break;
            }

            // QUIT
            case 5:
            {
                StartCoroutine(CallNextScene(SceneManagerController.GetMainMenuSceneName()));
                break;
            }

            default: break;
        }
    }

    private void PlaySong(int index)
    {
        songEllapsedTime = 0;
        canEllapseTime = true;
        isSongPaused = false;
        audioController.StopMusic();
        audioController.ChangeMusic(audioController.AllNotLoopedSongs[currentSongIndex], false, "", false, false);
        UpdateUI();
    }

    // Increments ellapsed time
    private void ShowEllapedSongTime()
    {
        songEllapsedTime += Time.deltaTime;
        UpdateSongDurationText();
    }

    // Updates the UI values
    private void UpdateUI()
    {
        if (!artistNameText || !songNameText || !songDurationText) return;

        AudioClip currentSong = audioController.AllNotLoopedSongs[currentSongIndex];
        string fileName = audioController.FormatMusicName(currentSong.name);
        string[] splitted = fileName.Split('-');
        if (splitted.Length == 0) return;
        artistNameText.text = splitted[0];
        songNameText.text = splitted[1];
        songDuration = currentSong.length;
        UpdateSongDurationText();
    }

    // Shows the formatted song duration
    private void UpdateSongDurationText()
    {
        string songEllapsedTimeText = Formatter.FormatEllapsedTime((int) songEllapsedTime);
        string currentSongDurationText = Formatter.FormatEllapsedTime((int) songDuration);
        songDurationText.text = string.Concat(songEllapsedTimeText, " / ", currentSongDurationText);

        if (songEllapsedTime >= songDuration)
        {
            if (isSongRepeated)
            {
                songEllapsedTime = 0;
            }
            else
            {
                audioController.SetIsSongPlaying(false);
                audioController.StopMusic();
            }
        }
    }

    private void UpdateHourText()
    {
        if (!hourText) return;
        int hour = DateTime.Now.Hour;
        int minute = DateTime.Now.Minute;
        hourText.text = string.Concat(hour.ToString("00"), ":", minute.ToString("00"));
    }

    public void MakeSelectOnPointerEnter(Button button)
    {
        if (!button || !button.interactable) return;
        button.Select();
    }

    public void SetCurrentButtonIndex(int index)
    {
        currentButtonIndex = index;
    }

    // Wait fade out length to fade out to next scene
    private IEnumerator CallNextScene(string nextSceneName)
    {
        actualGameState = GameState.TRANSITION;
        audioController.StopMusic();

        // Fade Out effect
        float fadeOutLength = fadeEffect.GetFadeOutLength();
        fadeEffect.FadeToLevel();
        yield return new WaitForSecondsRealtime(fadeOutLength);
        gameStatusController.SetNextSceneName(nextSceneName);
        SceneManagerController.CallScene(SceneManagerController.GetLoadingSceneName());
    }

    public void ChangeCursor(bool hand)
    {
        Texture2D cursor = (hand ? oldHandCursor : oldPointerCursor);
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
    }
}