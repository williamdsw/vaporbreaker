using System;
using System.Collections;
using System.Collections.Generic;
using Controllers.Core;
using Luminosity.IO;
using MVC.Enums;
using MVC.Global;
using MVC.Models;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.Menu
{
    public class ScoreboardPanelController : MonoBehaviour
    {
        // || Inspector References

        [Header("Required UI Elements")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private Button backButton;

        [Header("Required Table Elements")]
        [SerializeField] private TextMeshProUGUI scoreColumnLabel;
        [SerializeField] private TextMeshProUGUI timeScoreColumnLabel;
        [SerializeField] private TextMeshProUGUI bestComboColumnLabel;
        [SerializeField] private TextMeshProUGUI momentColumnLabel;
        [SerializeField] private Selectable table;
        [SerializeField] private Transform tableBodyViewportContent;
        [SerializeField] private ScoreboardRow scoreboardRowPrefab;

        // || Cached

        private TextMeshProUGUI backButtonLabel;

        // || Properties

        public static ScoreboardPanelController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            GetRequiredComponents();
            Translate();
            BindEventListeners();
        }

        private void Update() => CaptureCancel();

        /// <summary>
        /// Get required components
        /// </summary>
        private void GetRequiredComponents()
        {
            try
            {
                backButtonLabel = backButton.GetComponentInChildren<TextMeshProUGUI>();
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
                titleLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.general_scoreboard);
                backButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.general_back);
                scoreColumnLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.general_score);
                timeScoreColumnLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.general_timescore);
                bestComboColumnLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.levelcomplete_bestcombo);
                //momentColumnLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.); // TODO!
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
                backButton.onClick.AddListener(() => Close());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Delete all rows
        /// </summary>
        private IEnumerator ClearTable()
        {
            foreach (Transform item in tableBodyViewportContent)
            {
                Destroy(item.gameObject);
            }

            yield return new WaitUntil(() => tableBodyViewportContent.childCount == 0);
        }

        /// <summary>
        /// Lists all items
        /// </summary>
        /// <param name="scoreboards"> List of instances of Scoreboard </param>
        private IEnumerator ListScoreboard(List<Scoreboard> scoreboards)
        {
            yield return ClearTable();

            int index = 0;
            foreach (Scoreboard item in scoreboards)
            {
                ScoreboardRow clone = Instantiate(scoreboardRowPrefab);
                clone.Set(index + 1, item.Score, item.TimeScore, item.BestCombo, item.Moment, (index % 2 == 1));
                clone.gameObject.transform.SetParent(tableBodyViewportContent);
                clone.gameObject.transform.SetAsLastSibling();

                RectTransform rectTransform = clone.GetComponent<RectTransform>();
                rectTransform.localScale = Vector3.one;

                index++;
            }
        }

        /// <summary>
        /// Show this panel
        /// </summary>
        /// <param name="scoreboards"> List of instances of Scoreboard </param>
        public void Show(List<Scoreboard> scoreboards)
        {
            StartCoroutine(ListScoreboard(scoreboards));
            panel.SetActive(true);
            table.Select();
        }

        /// <summary>
        /// Capture ESC key
        /// </summary>
        private void CaptureCancel()
        {
            if (!panel.activeSelf || SelectLevelsController.Instance.ActualGameState != GameState.GAMEPLAY) return;

            if (InputManager.GetButtonDown(Configuration.InputsNames.UiCancel))
            {
                Close();
            }
        }

        /// <summary>
        /// Close this panel
        /// </summary>
        private void Close()
        {
            AudioController.Instance.PlaySFX(AudioController.Instance.UiCancel, AudioController.Instance.MaxSFXVolume);
            panel.SetActive(false);
            SelectLevelsController.Instance.Show();
        }
    }
}