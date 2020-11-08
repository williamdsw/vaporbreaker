using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    // Config
    [SerializeField] private GameObject pressAnyKeyText;

    [Header ("Labels to Translate")]
    [SerializeField] private List<TextMeshProUGUI> uiLabels = new List<TextMeshProUGUI> ();

    // State
    private bool canPressKey = false;
    private float timeToWait = 2f;

    // Cached Components
    private FlashTextEffect flashTextEffect;

    // Cached Others 
    private AudioController audioController;
    private FadeEffect fadeEffect;
    private GameStatusController gameStatusController;
    private LocalizationController localizationController;

    //--------------------------------------------------------------------------------//

    private void Start ()
    {
        // Find Components 
        flashTextEffect = pressAnyKeyText.GetComponent<FlashTextEffect>();

        // Find Others
        audioController = FindObjectOfType<AudioController>();
        fadeEffect = FindObjectOfType<FadeEffect>();
        gameStatusController = FindObjectOfType<GameStatusController>();
        localizationController = FindObjectOfType<LocalizationController>();

        TranslateLabels ();
        StartCoroutine (WaitToShowPressAnyKey ());
    }

    private void Update ()
    {
        CaptureAnyKey ();
    }

    //--------------------------------------------------------------------------------//

    // Translate labels based on choosed language
    private void TranslateLabels ()
    {
        // CANCELS
        if (!localizationController) { return; }
        
        List<string> labels = localizationController.GetTitleLabels ();
        if (labels.Count == 0 || uiLabels.Count == 0) { return; }
        for (int index = 0; index < labels.Count; index++) { uiLabels[index].SetText (labels[index]); }
    }

    // Captures any key
    private void CaptureAnyKey ()
    {
        if (canPressKey)
        {
            if (InputManager.anyKeyDown)
            {
                canPressKey = false;
                StartCoroutine (CallNextScene (SceneManagerController.GetMainMenuSceneName ()));
            }
        }
    }

    //--------------------------------------------------------------------------------//
    // COROUTINES

    private IEnumerator WaitToShowPressAnyKey ()
    {
        int index = Random.Range (0, audioController.AllTitleVoices.Length);
        float duration = audioController.GetClipLength (audioController.AllTitleVoices[index]);
        audioController.PlayME (audioController.AllTitleVoices[index], audioController.GetMaxMEVolume (), false);
        yield return new WaitForSecondsRealtime (duration);
        canPressKey = true;
        pressAnyKeyText.SetActive (true);
    }

    // Wait fade out length to fade out to next scene
    private IEnumerator CallNextScene (string nextSceneName)
    {
        // Stop 
        audioController.PlaySFX (audioController.UiSubmit, audioController.GetMaxSFXVolume ());
        flashTextEffect.SetTimeToFlick (0.1f);
        yield return new WaitForSecondsRealtime (timeToWait);    

        // Fade Out
        float fadeOutLength = fadeEffect.GetFadeOutLength ();
        fadeEffect.FadeToLevel ();
        yield return new WaitForSecondsRealtime (fadeOutLength);
        gameStatusController.SetNextSceneName (nextSceneName);
        gameStatusController.SetCameFromLevel (false);
        SceneManagerController.CallScene (SceneManagerController.GetLoadingSceneName ());
    }
}