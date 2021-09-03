using MVC.Global;
using System.Collections;
using UnityEngine;
using Utilities;

namespace Controllers.Menu
{
    public class LogoController : MonoBehaviour
    {
        // || State

        private bool databaseExists = false;

        // || Config

        private const float TIME_TO_CALL_ANIMATION = 1f;
        private const float TIME_TO_CALL_LOGO_VOICE = 2.5f;
        private const float TIME_TO_WAIT = 2f;

        private void Awake() => UnityUtilities.DisableAnalytics();

        private void Start()
        {
            StartCoroutine(ExtractDatabase());
            StartCoroutine(ShowLogo());
        }

        /// <summary>
        /// Extract database file
        /// </summary>
        private IEnumerator ExtractDatabase()
        {
            if (!FileManager.Exists(Configuration.Properties.DatabasePath))
            {
                FileManager.Copy(Configuration.Properties.DatabaseStreamingAssetsPath, Configuration.Properties.DatabasePath);
            }

            databaseExists = true;
            yield return null;
        }

        /// <summary>
        /// Show Retrogemn's logo
        /// </summary>
        private IEnumerator ShowLogo()
        {
            yield return new WaitUntil(() => databaseExists);
            yield return new WaitForSecondsRealtime(TIME_TO_CALL_ANIMATION);

            int index = Random.Range(0, AudioController.Instance.AllLogoVoices.Length);
            yield return new WaitForSecondsRealtime(TIME_TO_CALL_LOGO_VOICE);
            AudioController.Instance.PlaySFX(AudioController.Instance.AllLogoVoices[index], AudioController.Instance.GetMaxSFXVolume());
            AudioController.Instance.GetTracks();

            yield return new WaitForSecondsRealtime(TIME_TO_WAIT);
            yield return new WaitUntil(() => LocalizationController.Instance != null);
            LocalizationController.Instance.GetSavedLocalization();
            yield return new WaitUntil(() => LocalizationController.Instance.DictionaryCount > 0);

            yield return CallNextScene();
        }

        /// <summary>
        /// Goto loading and title
        /// </summary>
        private IEnumerator CallNextScene()
        {
            // Fade Out
            float fadeOutLength = FadeEffect.Instance.GetFadeOutLength();
            FadeEffect.Instance.FadeToLevel();
            yield return new WaitForSecondsRealtime(fadeOutLength);
            GameStatusController.Instance.NextSceneName = SceneManagerController.TitleSceneName;
            GameStatusController.Instance.CameFromLevel = false;
            SceneManagerController.CallScene(SceneManagerController.LoadingSceneName);
        }
    }
}