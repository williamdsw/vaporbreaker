using Controllers.Core;
using MVC.Enums;
using MVC.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utilities;

namespace Controllers.Panel
{
    /// <summary>
    /// Controller for Scoreboard Panel
    /// </summary>
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
                momentColumnLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.general_moment);
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
        /// Show this panel
        /// </summary>
        /// <param name="scoreboards"> List of instances of Scoreboard </param>
        public void Show(List<Scoreboard> scoreboards)
        {
            StartCoroutine(ListScoreboard(scoreboards));
            panel.SetActive(true);
        }

        /// <summary>
        /// Close this panel
        /// </summary>
        private void Close()
        {
            AudioController.Instance.PlaySFX(AudioController.Instance.UiCancelSound, AudioController.Instance.MaxSFXVolume);
            panel.SetActive(false);
            SelectLevelsPanelController.Instance.Show();
        }

        /// <summary>
        /// Get back to previous screen
        /// </summary>
        /// <param name="callbackContext"> Context with parameters </param>
        public void OnCancel(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed && callbackContext.ReadValueAsButton())
            {
                if (!panel.activeSelf || SelectLevelsPanelController.Instance.ActualGameState != Enumerators.GameStates.GAMEPLAY) return;

                Close();
            }
        }
    }
}