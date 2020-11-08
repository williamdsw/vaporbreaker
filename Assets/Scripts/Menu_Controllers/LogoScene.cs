using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogoScene : MonoBehaviour
{
    // Config
    [SerializeField] private GameObject jumpText;

    [Header ("Labels to Translate")]
    [SerializeField] private List<TextMeshProUGUI> uiLabels = new List<TextMeshProUGUI> ();

    // State
    private float duration = 0f;
    private float timeToCallLogoVoice = 2.5f;
    private float timeToWait = 5f;
    private bool canPressButton = false;

    // Cached
    private AudioController audioController;
    private FadeEffect fadeEffect;
    private GameStatusController gameStatusController;
    private LocalizationController localizationController;

    //--------------------------------------------------------------------------------//

    private void Start () 
    {
        // Find Objects
        audioController = FindObjectOfType<AudioController>();
        fadeEffect = FindObjectOfType<FadeEffect>();
        gameStatusController = FindObjectOfType<GameStatusController>();
        localizationController = FindObjectOfType<LocalizationController>();

        // Play logo sound
        audioController.PlayME (audioController.EightiesRiff, audioController.GetMaxMEVolume (), false);
        duration = audioController.GetClipLength (audioController.EightiesRiff);

        TranslateLabels ();
        StartCoroutine (PlayAndShowLogo ());
    }

    private void Update ()
    {
        CaptureStartInput ();
    }

    //--------------------------------------------------------------------------------//

    // Translate labels based on choosed language
    public void TranslateLabels ()
    {
        if (!localizationController) { return; }
        List<string> labels = localizationController.GetPressAnyKeyLabels ();
        if (labels.Count == 0 || uiLabels.Count == 0) { return; }
        for (int index = 0; index < labels.Count; index++) { uiLabels[index].SetText (labels[index]); }
    }

    // Captures Pause Button
    private void CaptureStartInput ()
    {
        if (canPressButton)
        {
            if (InputManager.anyKeyDown)
            {
                canPressButton = false;
                StopCoroutine ("CallNextScene");
                StartCoroutine (CallNextScene (SceneManagerController.GetTitleSceneName ()));
            }
        }
    }

    //--------------------------------------------------------------------------------//

    private IEnumerator PlayAndShowLogo ()
    {
        // Logo's voice
        yield return new WaitForSecondsRealtime (0.5f);
        canPressButton = true;
        jumpText.SetActive (true);
        int index = Random.Range (0, audioController.AllLogoVoices.Length);
        yield return new WaitForSecondsRealtime (timeToCallLogoVoice);
        audioController.PlaySFX (audioController.AllLogoVoices[index], audioController.GetMaxSFXVolume ());
        yield return new WaitForSecondsRealtime (duration / 2f);

        StopCoroutine ("CallNextScene");
        StartCoroutine (CallNextScene (SceneManagerController.GetTitleSceneName ()));
    }

    // Wait fade out length to fade out to next scene
    private IEnumerator CallNextScene (string nextSceneName)
    {
        // Fade Out
        float fadeOutLength = fadeEffect.GetFadeOutLength ();
        fadeEffect.FadeToLevel ();
        yield return new WaitForSecondsRealtime (fadeOutLength);
        audioController.StopME ();
        gameStatusController.SetNextSceneName (nextSceneName);
        gameStatusController.SetCameFromLevel (false);
        SceneManagerController.CallScene (SceneManagerController.GetLoadingSceneName ());
    }
}