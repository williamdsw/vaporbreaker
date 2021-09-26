using Controllers.Core;
using Effects;
using MVC.Enums;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Menu
{
    public class TitleScreenController : MonoBehaviour
    {
        // || Inspector References

        [Header("Required UI Elements")]
        [SerializeField] private TextMeshProUGUI copyrightText;
        [SerializeField] private TextMeshProUGUI pressText;

        // || Config

        private const float TIME_TO_WAIT = 2f;

        // || State

        private bool canPress = false;

        // || Cached

        private FlashTextEffect flashTextEffect;

        private void Awake()
        {
            GetRequiredComponents();
            Translate();
        }

        private IEnumerator Start()
        {
            yield return WaitToShowPressAnyKey();
        }

        /// <summary>
        /// Get required components
        /// </summary>
        private void GetRequiredComponents()
        {
            try
            {
                flashTextEffect = pressText.GetComponent<FlashTextEffect>();
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
                copyrightText.text = LocalizationController.Instance.GetWord(LocalizationFields.about_rights);
                pressText.text = LocalizationController.Instance.GetWord(LocalizationFields.logo_pressanykey);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Play the title voice and waits to show press any key text
        /// </summary>
        private IEnumerator WaitToShowPressAnyKey()
        {
            int index = UnityEngine.Random.Range(0, AudioController.Instance.AllTitleVoices.Length);
            float duration = AudioController.Instance.GetClipLength(AudioController.Instance.AllTitleVoices[index]);
            AudioController.Instance.PlayME(AudioController.Instance.AllTitleVoices[index], AudioController.Instance.MaxMEVolume, false);
            yield return new WaitForSecondsRealtime(duration);
            pressText.gameObject.SetActive(true);
            canPress = true;
        }

        /// <summary>
        /// Go to loading then main menu
        /// </summary>
        private IEnumerator CallNextScene()
        {
            canPress = false;
            AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmitSound, AudioController.Instance.MaxSFXVolume);
            flashTextEffect.TimeToFlick = 0.1f;
            yield return new WaitForSecondsRealtime(TIME_TO_WAIT);

            FadeEffect.Instance.FadeToLevel();
            yield return new WaitForSecondsRealtime(FadeEffect.Instance.GetFadeOutLength());
            GameStatusController.Instance.NextSceneName = SceneManagerController.MainMenuSceneName;
            GameStatusController.Instance.CameFromLevel = false;
            SceneManagerController.CallScene(SceneManagerController.LoadingSceneName);
        }

        /// <summary>
        /// On any key or button has been pressed
        /// </summary>
        /// <param name="callbackContext"> Context with parameters </param>
        public void OnAnyKeyOrButtonPressed(InputAction.CallbackContext callbackContext)
        {
            if (!canPress) return;

            if (callbackContext.performed)
            {
                StartCoroutine(CallNextScene());
            }
        }
    }
}