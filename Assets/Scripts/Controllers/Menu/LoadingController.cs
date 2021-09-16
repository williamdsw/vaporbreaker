using Controllers.Core;
using Effects;
using MVC.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.Menu
{
    public class LoadingController : MonoBehaviour
    {
        // || Inspector References

        [Header("UI Elements")]
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private GameObject[] instructionsPanels;
        [SerializeField] private Button[] gotoButtons;
        [SerializeField] private Button continueButton;
        [SerializeField] private TextMeshProUGUI powerUpNameLabel;

        [Header("Labels to Translate Global")]
        [SerializeField] private TextMeshProUGUI keyboardLayoutHeader;
        [SerializeField] private TextMeshProUGUI gamepadLayoutHeader;
        [SerializeField] private TextMeshProUGUI blocksHeader;
        [SerializeField] private TextMeshProUGUI powerUpHeader;
        [SerializeField] private List<TextMeshProUGUI> movePaddleLabels;

        [Header("Keyboard / Gamepad Layouts Labels")]
        [SerializeField] private List<TextMeshProUGUI> impulsePaddleLabels;
        [SerializeField] private List<TextMeshProUGUI> pauseLabels;
        [SerializeField] private List<TextMeshProUGUI> changeSongLabels;
        [SerializeField] private List<TextMeshProUGUI> releaseBallShootLabels;

        [Header("Blocks Labels")]
        [SerializeField] private TextMeshProUGUI oneHitBlockLabel;
        [SerializeField] private TextMeshProUGUI twoHitsBlockLabel;
        [SerializeField] private TextMeshProUGUI threeHitsBlockLabel;
        [SerializeField] private TextMeshProUGUI fourHitsBlockLabel;
        [SerializeField] private TextMeshProUGUI fiveHitsBlockLabel;
        [SerializeField] private TextMeshProUGUI randomHitsBlockLabel;
        [SerializeField] private TextMeshProUGUI powerUpBlockLabel;
        [SerializeField] private TextMeshProUGUI unbreakableBlockLabel;

        // || Config
        private const float TIME_TO_WAIT = 1f;

        // || Cached

        private TextMeshProUGUI continueButtonLabel;

        private void Awake() => GetRequiredComponents();

        private void Start()
        {
            if (GameStatusController.Instance.NextSceneName.Equals(SceneManagerController.LevelSceneName))
            {
                Translate();
                BindEventListeners();
            }
            else
            {
                powerUpNameLabel.text = string.Empty;
                loadingPanel.SetActive(false);
                StartCoroutine(CallNextScene());
            }
        }

        /// <summary>
        /// Get required components
        /// </summary>
        private void GetRequiredComponents()
        {
            try
            {
                continueButtonLabel = continueButton.GetComponentInChildren<TextMeshProUGUI>();
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
                keyboardLayoutHeader.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_keyboardlayout);
                gamepadLayoutHeader.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_gamepadlayout);
                blocksHeader.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_blocks);
                powerUpHeader.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_powerups);
                movePaddleLabels.ForEach(label => label.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_movepaddle));
                impulsePaddleLabels.ForEach(label => label.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_impulsepaddle));
                pauseLabels.ForEach(label => label.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_pause));
                changeSongLabels.ForEach(label => label.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_changesong));
                releaseBallShootLabels.ForEach(label => label.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_releaseball));
                oneHitBlockLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_onehitblock);
                twoHitsBlockLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_twohitsblock);
                threeHitsBlockLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_threehitsblock);
                fourHitsBlockLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_fourhitsblock);
                fiveHitsBlockLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_fivehitsblock);
                randomHitsBlockLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_randomhitblock);
                powerUpBlockLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_powerupblock);
                unbreakableBlockLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.loading_unbreakableblock);
                continueButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.general_continue);
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
                // LAYOUT KEYBOARD PANEL - RIGHT BUTTON
                gotoButtons[0].onClick.AddListener(() =>
                {
                    instructionsPanels[0].SetActive(false);
                    instructionsPanels[1].SetActive(true);
                });

                // GAMEPAD KEYBOARD PANEL - LEFT BUTTON
                gotoButtons[1].onClick.AddListener(() =>
                {
                    instructionsPanels[1].SetActive(false);
                    instructionsPanels[0].SetActive(true);
                });

                // GAMEPAD KEYBOARD PANEL - RIGHT BUTTON
                gotoButtons[2].onClick.AddListener(() =>
                {
                    instructionsPanels[1].SetActive(false);
                    instructionsPanels[2].SetActive(true);
                });

                // BLOCKS PANEL - LEFT BUTTON
                gotoButtons[3].onClick.AddListener(() =>
                {
                    instructionsPanels[2].SetActive(false);
                    instructionsPanels[1].SetActive(true);
                });

                // BLOCKS PANEL - RIGHT BUTTON
                gotoButtons[4].onClick.AddListener(() =>
                {
                    instructionsPanels[2].SetActive(false);
                    instructionsPanels[3].SetActive(true);
                });

                // POWER UPS PANEL - LEFT BUTTON
                gotoButtons[5].onClick.AddListener(() =>
                {
                    powerUpNameLabel.text = string.Empty;
                    instructionsPanels[3].SetActive(false);
                    instructionsPanels[2].SetActive(true);
                });

                continueButton.onClick.AddListener(() =>
                {
                    continueButton.interactable = false;
                    StartCoroutine(CallNextScene());
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Calls the next scene
        /// </summary>
        private IEnumerator CallNextScene()
        {
            yield return new WaitForSecondsRealtime(TIME_TO_WAIT);
            FadeEffect.Instance.FadeToLevel();
            yield return new WaitForSecondsRealtime(FadeEffect.Instance.GetFadeOutLength());
            AsyncOperation operation = SceneManagerController.CallSceneAsync(GameStatusController.Instance.NextSceneName);
        }

        /// <summary>
        /// Set power up name in label by hovering icons
        /// </summary>
        /// <param name="field"> Desired field </param>
        public void SetPowerUpName(string sentence)
        {
            try
            {
                LocalizationFields field = (LocalizationFields)Enum.Parse(typeof(LocalizationFields), sentence);
                powerUpNameLabel.text = LocalizationController.Instance.GetWord(field);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}