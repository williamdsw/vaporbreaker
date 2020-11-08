using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpsMenu : MonoBehaviour
{
    [Header ("UI Elements")]
    [SerializeField] private Sprite[] powerUpsSpritesList;
    [SerializeField] private Image powerUpImage;
    [SerializeField] private TextMeshProUGUI powerUpDescriptionText;
    [SerializeField] private TextMeshProUGUI poweUpValueText;

    [Header ("Labels to Translate")]
    [SerializeField] private List<TextMeshProUGUI> uiLabels = new List<TextMeshProUGUI> ();

    // Config
    private int currentLevelIndex = 0;
    private List<string> powerUpsDescriptionsList = new List<string> ();
    private List<int> powerUpsMinValueList = new List<int> ();
    private List<int> powerUpsMaxValueList = new List<int> ();

    // Cached
    private AudioController audioController;
    private FadeEffect fadeEffect;
    private GameStatusController gameStatusController;
    private LocalizationController localizationController;
    
    //--------------------------------------------------------------------------------//

    private void Start () 
    {
        // Other
        audioController = FindObjectOfType<AudioController>();
        fadeEffect = FindObjectOfType<FadeEffect>();
        gameStatusController = FindObjectOfType<GameStatusController>();
        localizationController = FindObjectOfType<LocalizationController>();

        // Play music
        audioController.ChangeMusic (audioController.AllLoopedSongs[2], false, "", true, false);

        // Resets for animation works
        Time.timeScale = 1f;
        Cursor.visible = true;

        FillLists ();
        TranslateLabels ();
        UpdateUI ();
    }

    private void Update ()
    {
        ChangePowerUp ();
        CaptureCancelButton ();
    }

    //--------------------------------------------------------------------------------//

    private void FillLists ()
    {
        powerUpsMinValueList.Add (0);
        powerUpsMaxValueList.Add (1000);

        powerUpsMinValueList.Add (0);
        powerUpsMaxValueList.Add (1000);

        powerUpsMinValueList.Add (5000);
        powerUpsMaxValueList.Add (10000);

        powerUpsMinValueList.Add (100);
        powerUpsMaxValueList.Add (1000);

        powerUpsMinValueList.Add (100);
        powerUpsMaxValueList.Add (1000);

        powerUpsMinValueList.Add (0);
        powerUpsMaxValueList.Add (1000);

        powerUpsMinValueList.Add (500);
        powerUpsMaxValueList.Add (2500);

        powerUpsMinValueList.Add (- 1000);
        powerUpsMaxValueList.Add (- 10000);

        powerUpsMinValueList.Add (0);
        powerUpsMaxValueList.Add (- 1000);

        powerUpsMinValueList.Add (100);
        powerUpsMaxValueList.Add (500);

        powerUpsMinValueList.Add (100);
        powerUpsMaxValueList.Add (1000);

        powerUpsMinValueList.Add (10000);
        powerUpsMaxValueList.Add (30000);

        powerUpsMinValueList.Add (0);
        powerUpsMaxValueList.Add (0);

        powerUpsMinValueList.Add (100);
        powerUpsMaxValueList.Add (500);

        powerUpsMinValueList.Add (100);
        powerUpsMaxValueList.Add (500);
    }

    // Translate labels based on choosed language
    private void TranslateLabels ()
    {
        // CANCELS
        if (!localizationController) { return; }
        
        List<string> labels = localizationController.GetPowerUpsSceneLabels ();
        powerUpsDescriptionsList = localizationController.GetPowerUpsDescriptions ();
        if (labels.Count == 0 || uiLabels.Count == 0 || labels.Count != uiLabels.Count ) { return; }
        for (int index = 0; index < labels.Count; index++) { uiLabels[index].SetText (labels[index]); }
    }

    // Change level on left / right
    private void ChangePowerUp ()
    {
        // Cancels
        if (powerUpsSpritesList.Length == 0) { return; }

        if (InputManager.GetButtonDown ("UI_Right"))
        {
            audioController.PlaySFX (audioController.PowerUpSound, audioController.GetMaxSFXVolume ());
            currentLevelIndex++;
            currentLevelIndex = (currentLevelIndex >= powerUpsSpritesList.Length ? 0 : currentLevelIndex);
            UpdateUI ();
        }
        else if (InputManager.GetButtonDown ("UI_Left"))
        {
            audioController.PlaySFX (audioController.PowerUpSound, audioController.GetMaxSFXVolume ());
            currentLevelIndex--;
            currentLevelIndex = (currentLevelIndex < 0 ? powerUpsSpritesList.Length - 1 : currentLevelIndex);
            UpdateUI ();
        }
    }

    // Updates the UI values
    private void UpdateUI ()
    {
        // Cancels
        if (!powerUpImage || !powerUpDescriptionText || !poweUpValueText) { return; }

        // Set texts
        powerUpDescriptionText.text = powerUpsDescriptionsList[currentLevelIndex];
        poweUpValueText.text = string.Concat (powerUpsMinValueList[currentLevelIndex], "/", powerUpsMaxValueList[currentLevelIndex]);
        powerUpImage.sprite = powerUpsSpritesList[currentLevelIndex];
    }

    // Capture Cancel Button on situations
    private void CaptureCancelButton ()
    {
        if (InputManager.GetButtonDown ("UI_Cancel"))
        {
            audioController.StopMusic ();
            audioController.PlaySFX (audioController.UiCancel, audioController.GetMaxSFXVolume ());
            StartCoroutine (CallNextScene (SceneManagerController.GetMainMenuSceneName ()));
        }
    }

    //--------------------------------------------------------------------------------//
    // COROUTINES

    // Wait fade out length to fade out to next scene
    private IEnumerator CallNextScene (string nextSceneName)
    {
        // Fade Out effect
        float fadeOutLength = fadeEffect.GetFadeOutLength ();
        fadeEffect.FadeToLevel ();
        yield return new WaitForSecondsRealtime (fadeOutLength);
        gameStatusController.SetNextSceneName (nextSceneName);
        SceneManagerController.CallScene (SceneManagerController.GetLoadingSceneName ());
    }
}