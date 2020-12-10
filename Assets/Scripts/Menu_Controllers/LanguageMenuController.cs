using Luminosity.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LanguageMenuController : MonoBehaviour
{
    [Header ("Menus")]
    [SerializeField] private GameObject languageMenu;
    [SerializeField] private GameObject mainMenu;

    [Header ("Buttons")]
    [SerializeField] private Button[] allButtons;
    [SerializeField] private Button[] allDefaultButtons;
    [SerializeField] private Button backButton;

    [Header ("Labels to Translate")]
    [SerializeField] private List<TextMeshProUGUI> uiLabels = new List<TextMeshProUGUI> ();

    // State
    private List<string> languages = new List<string>() {"English", "Portuguese", "Spanish", "Italian"};
    private int currentButtonIndex;
    private string currentLanguage;
    private string currentFolderPath;

    // Cached
    private AudioController audioController;
    private BindingMenu bindingMenuController;
    private DefaultLayoutMenu defaultLayoutMenu;
    private InputControllerUI inputControllerUI;
    private LocalizationController localizationController;
    private MainMenu mainMenuController;
    private OptionsMenu optionsMenuController;

    //--------------------------------------------------------------------------------//

    private void Start () 
    {
        // Find Objects
        audioController = FindObjectOfType<AudioController>();
        bindingMenuController = FindObjectOfType<BindingMenu>();
        defaultLayoutMenu = FindObjectOfType<DefaultLayoutMenu>();
        inputControllerUI = FindObjectOfType<InputControllerUI>();
        localizationController = FindObjectOfType<LocalizationController>();
        mainMenuController = FindObjectOfType<MainMenu>();
        optionsMenuController = FindObjectOfType<OptionsMenu>();

        BindingClickEvents ();
        TranslateLabels ();

        // Default Language
        string language = PlayerPrefsController.GetLanguage ();
        currentButtonIndex = languages.IndexOf (language);
    }

    private void Update () 
    {
        if (!languageMenu.activeSelf) { return; }
        CaptureInputs ();
    }

    //--------------------------------------------------------------------------------//
    // START USED

    private void BindingClickEvents ()
    {
        if (!languageMenu || !mainMenu || allButtons.Length == 0 || allDefaultButtons.Length == 0) { return; }

        // BACK
        backButton.onClick.AddListener (delegate
        {
            PlayerPrefsController.SetLanguage (currentLanguage);
            audioController.PlaySFX (audioController.UiCancel, audioController.GetMaxSFXVolume ());
            languageMenu.SetActive (false);
            mainMenu.SetActive (true);
            allDefaultButtons[1].Select ();
        });
    }

    // Translate labels based on choosed language
    private void TranslateLabels ()
    {
        if (!localizationController) { return; }
        List<string> labels = localizationController.GetLanguageMenuLabels ();
        if (labels.Count == 0 || uiLabels.Count == 0) { return; }
        for (int index = 0; index < labels.Count; index++) { uiLabels[index].SetText (labels[index]); }
    }

    //--------------------------------------------------------------------------------//
    // POINTER ENTER

    public void MakeSelectOnPointerEnter (Button button)
    {
        if (!button || !button.interactable) { return; }
        button.Select ();
    }

    public void SetCurrentButtonIndex (int index)
    {
        currentButtonIndex = index;
    }

    //--------------------------------------------------------------------------------//
    // UPDATE EVENTS

    // Capture User Inputs
    private void CaptureInputs ()
    {
        // Cancels 
        if (allButtons.Length == 0) { return; }

        // Right / Left
        if (InputManager.GetButtonDown ("UI_Right"))
        {
            currentButtonIndex++;
            currentButtonIndex = (currentButtonIndex >= allButtons.Length ? 0 : currentButtonIndex);
            allButtons[currentButtonIndex].Select();
        }
        else if (InputManager.GetButtonDown ("UI_Left"))
        {
            currentButtonIndex--;
            currentButtonIndex = (currentButtonIndex < 0 ? allButtons.Length - 1 : currentButtonIndex);
            allButtons[currentButtonIndex].Select();
        }

        // Submit
        if (currentButtonIndex == -1) { return; }
        if (EventSystem.current.currentSelectedGameObject)
        {
            if (InputManager.GetButtonDown ("UI_Submit"))
            {
                ActionButton (currentButtonIndex);
            }
        }
    }

    // Sets the action for button click
    private void ActionButton (int index)
    {
        // Play SFX
        audioController.PlaySFX (audioController.ClickSound, audioController.GetMaxSFXVolume ());

        switch (currentButtonIndex)
        {
            // ENGLISH
            case 0:
            {
                currentLanguage = "English";
                currentFolderPath = FileManager.LocalizationEnglishFolderPath;
                break;
            }

            // PORTUGUESE
            case 1:
            {
                currentLanguage = "Portuguese";
                currentFolderPath = FileManager.LocalizationPortugueseFolderPath;
                break;
            }

            // SPANISH
            case 2:
            {
                currentLanguage = "Spanish";
                currentFolderPath = FileManager.LocalizationSpanishFolderPath;
                break;
            }

            // ITALIAN
            case 3:
            {
                currentLanguage = "Italian";
                currentFolderPath = FileManager.LocalizationItalianFolderPath;
                break;
            }

            default: { break; }
        }

        localizationController.LoadLocalization (currentFolderPath);
        TranslateLabels ();
        mainMenuController.TranslateLabels ();
        optionsMenuController.TranslateLabels ();
        bindingMenuController.TranslateLabels ();
        inputControllerUI.TranslateLabels ();
        defaultLayoutMenu.TranslateLabels ();
    }
}