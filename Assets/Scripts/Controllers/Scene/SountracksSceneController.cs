﻿using Controllers.Core;
using Effects;
using MVC.Enums;
using MVC.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Controllers.Scene
{
    /// <summary>
    /// Controller for Sountracks Scene
    /// </summary>
    public class SountracksSceneController : MonoBehaviour
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
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color selectedColor;

        [Header("Required UI Elements - Others")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI hourText;
        [SerializeField] private Button quitButton;

        // || State

        private Enumerators.GameStates actualGameState = Enumerators.GameStates.GAMEPLAY;
        private int currentSongIndex = 0;
        private float songEllapsedTime = 0f;
        private float songDuration = 0f;
        private bool isSongPaused = true;
        private bool isSongRepeated = false;
        private bool canEllapseTime = false;

        // || Cached
        private Dictionary<string, Sprite> coversDictionary;
        private TextMeshProUGUI quitButtonLabel;
        private Image repeatButtonImage;
        private Image pauseButtonImage;

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

            defaultColor = repeatButtonImage.color;
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
                quitButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.general_quit);
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
                        pauseButtonImage.color = (isSongPaused ? selectedColor : defaultColor);
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
                    repeatButtonImage.color = (isSongRepeated ? selectedColor : defaultColor);
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
                totalDurationLabel.text = Formatter.GetEllapsedTimeInMinutes((int)songDuration);
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
            string songEllapsedTimeText = Formatter.GetEllapsedTimeInMinutes((int)songEllapsedTime);
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
            GameStatusController.Instance.NextSceneName = SceneManagerController.SceneNames.MainMenu;
            GameStatusController.Instance.CameFromLevel = false;
            SceneManagerController.CallScene(SceneManagerController.SceneNames.Loading);
        }
    }
}