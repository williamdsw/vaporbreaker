using System;
using System.Collections;
using System.Collections.Generic;
using Controllers.Core;
using Effects;
using MVC.Enums;
using MVC.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Controllers.Menu
{
    public class MusicPlayerController : MonoBehaviour
    {
        // || Inspector References

        [Header("Music Player - Others")]
        [SerializeField] private Image coverImage;
        [SerializeField] private Sprite[] coversSprites;

        [Header("Music Player - Information")]
        [SerializeField] private TextMeshProUGUI trackNameLabel;
        [SerializeField] private TextMeshProUGUI artistNameLabel;

        [Header("Music Player - Timing")]
        [SerializeField] private TextMeshProUGUI currentDurationLabel;
        [SerializeField] private Slider trackSlider;
        [SerializeField] private TextMeshProUGUI totalDurationLabel;

        [Header("Music Player - Controls")]
        [SerializeField] private Button previousButton;
        [SerializeField] private Button playButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button repeatButton;

        [Header("Required UI Elements - Others")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI hourText;
        [SerializeField] private Button quitButton;

        // || State

        [SerializeField] private Enumerators.GameStates actualGameState = Enumerators.GameStates.GAMEPLAY;
        private int currentButtonIndex = 0;
        private int currentSongIndex = 0;
        [SerializeField] private bool isSongPaused = true;
        [SerializeField] private bool isSongRepeated = false;
        [SerializeField] private bool canEllapseTime = false;
        [SerializeField] private float songEllapsedTime = 0f;
        [SerializeField] private float songDuration = 0f;

        // || Cached

        private Image repeatButtonImage;
        private Image pauseButtonImage;
        private TextMeshProUGUI quitButtonLabel;
        private Dictionary<string, Sprite> coversDictionary;

        private void Awake()
        {
            coversDictionary = new Dictionary<string, Sprite>();
            Time.timeScale = 1f;
            ConfigurationsController.ToggleCursor(true);

            FillCoversDictionary();
            GetRequiredComponents();
            Translate();
            BindEventListeners();
            GetSongInfo();
        }

        private void Update()
        {
            if (actualGameState == Enumerators.GameStates.GAMEPLAY)
            {
                if (AudioController.Instance.IsSongPlaying && !isSongPaused && canEllapseTime)
                {
                    ShowEllapsedSongTime();
                }
            }

            UpdateHourText();
        }

        /// <summary>
        /// Fill dictionary of covers
        /// </summary>
        private void FillCoversDictionary()
        {
            try
            {
                foreach (Sprite item in coversSprites)
                {
                    coversDictionary.Add(item.name, item);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get required components
        /// </summary>
        private void GetRequiredComponents()
        {
            try
            {
                quitButtonLabel = quitButton.GetComponentInChildren<TextMeshProUGUI>();
                repeatButtonImage = repeatButton.GetComponentInChildren<Image>();
                pauseButtonImage = pauseButton.GetComponentInChildren<Image>();
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
                previousButton.onClick.AddListener(() =>
                {
                    AudioController.Instance.PlaySFX(AudioController.Instance.ClickSound, AudioController.Instance.MaxSFXVolume);
                    currentSongIndex--;
                    currentSongIndex = (currentSongIndex < 0 ? AudioController.Instance.AllNotLoopedSongs.Length - 1 : currentSongIndex);
                    PlaySong(currentSongIndex);
                });

                playButton.onClick.AddListener(() =>
                {
                    AudioController.Instance.PlaySFX(AudioController.Instance.ClickSound, AudioController.Instance.MaxSFXVolume);
                    PlaySong(currentSongIndex);
                });

                pauseButton.onClick.AddListener(() =>
                {
                    if (AudioController.Instance.IsSongPlaying)
                    {
                        AudioController.Instance.PlaySFX(AudioController.Instance.ClickSound, AudioController.Instance.MaxSFXVolume);
                        isSongPaused = !isSongPaused;
                        Color current = pauseButtonImage.color;
                        current.a = (isSongPaused ? 0.5f : 1f);
                        pauseButtonImage.color = current;
                        AudioController.Instance.PauseMusic(isSongPaused);
                    }
                });

                nextButton.onClick.AddListener(() =>
                {
                    AudioController.Instance.PlaySFX(AudioController.Instance.ClickSound, AudioController.Instance.MaxSFXVolume);
                    currentSongIndex++;
                    currentSongIndex = (currentSongIndex >= AudioController.Instance.AllNotLoopedSongs.Length ? 0 : currentSongIndex);
                    PlaySong(currentSongIndex);
                });

                repeatButton.onClick.AddListener(() =>
                {
                    isSongRepeated = !isSongRepeated;
                    AudioController.Instance.ToggleRepeatTrack(isSongRepeated);
                    Color current = repeatButtonImage.color;
                    current.a = (isSongRepeated ? 0.5f : 1f);
                    repeatButtonImage.color = current;
                });

                quitButton.onClick.AddListener(() => StartCoroutine(CallNextScene()));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Play the selected song
        /// </summary>
        /// <param name="index"> Song index at list </param>
        private void PlaySong(int index)
        {
            songEllapsedTime = 0;
            canEllapseTime = true;
            isSongPaused = false;
            AudioController.Instance.StopMusic();
            AudioController.Instance.ChangeMusic(AudioController.Instance.AllNotLoopedSongs[currentSongIndex], false, "", false, false);
            GetSongInfo();
        }

        /// <summary>
        /// Increments song ellapsed time and update UI
        /// </summary>
        private void ShowEllapsedSongTime()
        {
            songEllapsedTime += Time.deltaTime;
            UpdateSongEllapsedTimeText();
        }

        /// <summary>
        /// Recover some information about the song
        /// </summary>
        private void GetSongInfo()
        {
            try
            {
                AudioClip currentSong = AudioController.Instance.AllNotLoopedSongs[currentSongIndex];
                Track track = AudioController.Instance.Tracks.Find(t => t.FileName.Equals(currentSong.name));
                trackNameLabel.text = track.Title;
                artistNameLabel.text = track.Artist;
                songDuration = currentSong.length;
                totalDurationLabel.text = Formatter.FormatEllapsedTimeInMinutes((int)songDuration);
                trackSlider.maxValue = songDuration;
                coverImage.sprite = coversDictionary[track.Cover];
                UpdateSongEllapsedTimeText();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Updates the song ellapsed time text
        /// </summary>
        private void UpdateSongEllapsedTimeText()
        {
            trackSlider.value = songEllapsedTime;
            string songEllapsedTimeText = Formatter.FormatEllapsedTimeInMinutes((int)songEllapsedTime);
            currentDurationLabel.text = songEllapsedTimeText;

            if (songEllapsedTime >= songDuration)
            {
                if (isSongRepeated)
                {
                    songEllapsedTime = 0;
                }
                else
                {
                    AudioController.Instance.IsSongPlaying = false;
                    AudioController.Instance.StopMusic();
                }
            }
        }

        /// <summary>
        /// Updates the current hour text
        /// </summary>
        private void UpdateHourText() => hourText.text = DateTime.Now.ToString("HH:mm tt");

        /// <summary>
        /// Stop all and calls main menu scene
        /// </summary>
        private IEnumerator CallNextScene()
        {
            canvasGroup.interactable = false;
            actualGameState = Enumerators.GameStates.TRANSITION;
            AudioController.Instance.StopMusic();

            FadeEffect.Instance.FadeToLevel();
            yield return new WaitForSecondsRealtime(FadeEffect.Instance.GetFadeOutLength());
            GameStatusController.Instance.NextSceneName = SceneManagerController.MainMenuSceneName;
            GameStatusController.Instance.CameFromLevel = false;
            SceneManagerController.CallScene(SceneManagerController.LoadingSceneName);
        }
    }
}