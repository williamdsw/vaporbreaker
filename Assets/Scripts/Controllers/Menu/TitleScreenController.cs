using Controllers.Core;
using Luminosity.IO;
using MVC.Enums;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Controllers.Menu
{
    public class TitleScreenController : MonoBehaviour
    {
        // || Inspector References

        [Header("Required UI Elements")]
        [SerializeField] private TextMeshProUGUI copyrightText;
        [SerializeField] private TextMeshProUGUI pressAnyKeyText;

        // || State

        private bool canPressKey = false;

        // || Config

        private float TIME_TO_WAIT = 2f;

        // Cached Components
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

        private void Update() => CaptureAnyKey();

        /// <summary>
        /// Get required components
        /// </summary>
        private void GetRequiredComponents()
        {
            try
            {
                flashTextEffect = pressAnyKeyText.GetComponent<FlashTextEffect>();
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
                pressAnyKeyText.text = LocalizationController.Instance.GetWord(LocalizationFields.logo_pressanykey);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Captures any key to redirect to Main Menu
        /// </summary>
        private void CaptureAnyKey()
        {
            if (canPressKey)
            {
                if (InputManager.anyKeyDown)
                {
                    canPressKey = false;
                    StartCoroutine(CallNextScene());
                }
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
            canPressKey = true;
            pressAnyKeyText.gameObject.SetActive(true);
        }

        /// <summary>
        /// Go to loading then main menu
        /// </summary>
        private IEnumerator CallNextScene()
        {
            AudioController.Instance.PlaySFX(AudioController.Instance.UiSubmit, AudioController.Instance.MaxSFXVolume);
            flashTextEffect.SetTimeToFlick(0.1f);
            yield return new WaitForSecondsRealtime(TIME_TO_WAIT);

            float fadeOutLength = FadeEffect.Instance.GetFadeOutLength();
            FadeEffect.Instance.FadeToLevel();
            yield return new WaitForSecondsRealtime(fadeOutLength);
            GameStatusController.Instance.NextSceneName = SceneManagerController.MainMenuSceneName;
            GameStatusController.Instance.CameFromLevel = false;
            SceneManagerController.CallScene(SceneManagerController.LoadingSceneName);
        }
    }
}