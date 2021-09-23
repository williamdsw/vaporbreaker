using Controllers.Core;
using Effects;
using MVC.Enums;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.Menu
{
    public class CreditsController : MonoBehaviour
    {
        // || Inspector References

        [Header("Required UI Elements")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI creditsText;
        [SerializeField] private Button backButton;

        // || Cached

        private TextMeshProUGUI backButtonLabel;

        private void Awake()
        {
            GetRequiredComponents();
            Translate();
            BindEventListeners();

            AudioController.Instance.ChangeMusic(AudioController.Instance.AllLoopedSongs[3], false, string.Empty, true, false);
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
                creditsText.text = LocalizationController.Instance.GetWord(LocalizationFields.messages_credits);
                backButtonLabel.text = LocalizationController.Instance.GetWord(LocalizationFields.general_back);
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
                backButton.onClick.AddListener(() => StartCoroutine(CallNextScene()));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Call the next scene
        /// </summary>
        private IEnumerator CallNextScene()
        {
            canvasGroup.interactable = false;
            AudioController.Instance.StopMusic();
            FadeEffect.Instance.FadeToLevel();
            yield return new WaitForSecondsRealtime(FadeEffect.Instance.GetFadeOutLength());
            GameStatusController.Instance.NextSceneName = SceneManagerController.MainMenuSceneName;
            GameStatusController.Instance.CameFromLevel = false;
            SceneManagerController.CallScene(SceneManagerController.LoadingSceneName);
        }
    }
}