using Controllers.Core;
using Effects;
using MVC.Global;
using System.Collections;
using UnityEngine;
using Utilities;

namespace Controllers.Scene
{
    /// <summary>
    /// Controller for Logo Scene
    /// </summary>
    public class LogoSceneController : MonoBehaviour
    {
        // || State

        private bool databaseExists = false;

        // || Config

        private const float TIME_TO_CALL_ANIMATION = 1f;
        private const float TIME_TO_CALL_LOGO_VOICE = 2.5f;
        private const float TIME_TO_WAIT = 2f;

        private void Awake() => UnityUtilities.DisableAnalytics();

        private IEnumerator Start()
        {
            yield return ExtractDatabase();
            yield return ShowLogo();
        }

        /// <summary>
        /// Extract database file
        /// </summary>
        private IEnumerator ExtractDatabase()
        {
            if (!FileManager.Exists(Configuration.Properties.DatabasePath))
            {
                FileManager.Copy(Configuration.Properties.DatabaseStreamingAssetsPath, Configuration.Properties.DatabasePath);
                PlayerPrefsController.DeleteAll();
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
            AudioController.Instance.PlaySFX(AudioController.Instance.AllLogoVoices[index], AudioController.Instance.MaxSFXVolume);
            AudioController.Instance.GetTracks();

            yield return new WaitForSecondsRealtime(TIME_TO_WAIT);
            yield return new WaitUntil(() => LocalizationController.Instance != null);
            LocalizationController.Instance.GetLocalization();
            yield return new WaitUntil(() => LocalizationController.Instance.DictionaryCount > 0);

            yield return CallNextScene();
        }

        /// <summary>
        /// Goto loading and title
        /// </summary>
        private IEnumerator CallNextScene()
        {
            FadeEffect.Instance.FadeToLevel();
            yield return new WaitForSecondsRealtime(FadeEffect.Instance.GetFadeOutLength());
            GameStatusController.Instance.NextSceneName = SceneManagerController.SceneNames.Title;
            GameStatusController.Instance.CameFromLevel = false;
            SceneManagerController.CallScene(SceneManagerController.SceneNames.Loading);
        }
    }
}