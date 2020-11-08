using Luminosity.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header ("UI Elements")]
    [SerializeField] private GameObject[] allMenus;
    [SerializeField] private Button[] labelButtons;
    [SerializeField] private Button[] valueButtons;
    [SerializeField] private Button[] allOtherButtons;
    [SerializeField] private Button[] allDefaultButtons;

    [Header ("Audio Controller Sources")]
    [SerializeField] private AudioSource audioSourceBGM;
    [SerializeField] private AudioSource audioSourceSFX;

    [Header ("Labels to Translate")]
    [SerializeField] private List<TextMeshProUGUI> uiLabels = new List<TextMeshProUGUI> ();

    // Params config
    private List<string> resolutions = new List<string>();
    private string[] fullScreenOptions = { "No", "Yes" };
    private int resolutionIndex = 0;
    private int fullScreenIndex = 0;
    private string actualResolution = "";
    private bool isFullscreen = true;
    private float BGMVolume = 1f;
    private float SFXVolume = 1f;
    private int width;
    private int height;

    // Delay times
    private float timeToWaitUpdateVolume = 0.1f;
    private float startTimeToWaitUpdateVolume = 0.1f;
    private float timeToPlaySound = 1f;
    private float startTimeToPlaySound = 1f;

    // Cached Components
    private List<TextMeshProUGUI> valueButtonsTexts = new List<TextMeshProUGUI>();

    // Cached Others
    private AudioController audioController;
    private BindingMenu bindingMenuController;
    private LocalizationController localizationController;

    //--------------------------------------------------------------------------------//

    private void Start () 
    {
        // Find components
        foreach (Button button in valueButtons)
        {
            valueButtonsTexts.Add (button.GetComponentInChildren<TextMeshProUGUI>());
        }

        // Find Objects
        audioController = FindObjectOfType<AudioController>();
        bindingMenuController = FindObjectOfType<BindingMenu>();
        localizationController = FindObjectOfType<LocalizationController>();

        GetResolutions ();
        LoadSettings ();
        TranslateLabels ();
        BindingClickEvents ();
    }

    private void Update() 
    {
        // Cancel buttons for layout menus
        CaptureCancelButton (allMenus[2]);
        CaptureCancelButton (allMenus[3]);

        // Cancels
        if (!allMenus[1].activeSelf) { return; }

        // Prevents null ?
        if (!audioSourceBGM || !audioSourceSFX)
        {
            audioSourceBGM = GameObject.Find ("SourceBGM").GetComponent<AudioSource>();
            audioSourceSFX = GameObject.Find ("SourceSFX").GetComponent<AudioSource>();
        }

        ChooseResolution ();
        ChooseFullScreen ();
        ChooseVolume (labelButtons[2], valueButtons[2], valueButtonsTexts[2], audioSourceBGM);
        ChooseVolume (labelButtons[3], valueButtons[3], valueButtonsTexts[3], audioSourceSFX);
    }

    //--------------------------------------------------------------------------------//

    private void BindingClickEvents ()
    {
        if (allMenus.Length == 0 || labelButtons.Length == 0 || valueButtons.Length == 0 ||
            allOtherButtons.Length == 0 || allDefaultButtons.Length == 0) { return; }

        // RESOLUTION
        labelButtons[0].onClick.AddListener (delegate
        {
            valueButtons[0].interactable = true;
            ToggleValueButtonColor (valueButtons[0]);
            valueButtons[0].Select ();
            ToggleAllLabelButtonsInteractable ();
        });

        valueButtons[0].onClick.AddListener (delegate
        {
            valueButtons[0].interactable = false;
            ToggleValueButtonColor (valueButtons[0]);
            ToggleAllLabelButtonsInteractable ();
            labelButtons[0].Select ();
        });

        // FULLSCREEN
        labelButtons[1].onClick.AddListener (delegate
        {
            valueButtons[1].interactable = true;
            ToggleValueButtonColor (valueButtons[1]);
            valueButtons[1].Select ();
            ToggleAllLabelButtonsInteractable ();
        });

        valueButtons[1].onClick.AddListener (delegate
        {
            valueButtons[1].interactable = false;
            ToggleValueButtonColor (valueButtons[1]);
            ToggleAllLabelButtonsInteractable ();
            labelButtons[1].Select ();
        });

        // BACKGROUND VOLUME 
        labelButtons[2].onClick.AddListener (delegate
        {
            valueButtons[2].interactable = true;
            ToggleValueButtonColor (valueButtons[2]);
            valueButtons[2].Select ();
            ToggleAllLabelButtonsInteractable ();
        });

        valueButtons[2].onClick.AddListener (delegate
        {
            valueButtons[2].interactable = false;
            ToggleValueButtonColor (valueButtons[2]);
            ToggleAllLabelButtonsInteractable ();
            labelButtons[2].Select ();
            audioSourceBGM.volume = BGMVolume;
        });

        // SOUND EFFECTS VOLUME 
        labelButtons[3].onClick.AddListener (delegate
        {
            valueButtons[3].interactable = true;
            ToggleValueButtonColor (valueButtons[3]);
            valueButtons[3].Select ();
            ToggleAllLabelButtonsInteractable ();
            ApplyValues ();
        });

        valueButtons[3].onClick.AddListener (delegate
        {
            valueButtons[3].interactable = false;
            ToggleValueButtonColor (valueButtons[3]);
            ToggleAllLabelButtonsInteractable ();
            labelButtons[3].Select ();
            ApplyValues ();
        });

        // DEFAULT KEYBOARD LAYOUT MENU
        allOtherButtons[0].onClick.AddListener (delegate
        {
            allMenus[1].SetActive (false);
            allMenus[2].SetActive (true);
        });

        // DEFAULT GAMEPAD LAYOUT MENU
        allOtherButtons[1].onClick.AddListener (delegate
        {
            allMenus[1].SetActive (false);
            allMenus[3].SetActive (true);
        });

        // BINDINGS MENU
        allOtherButtons[2].onClick.AddListener (delegate
        {
            allMenus[1].SetActive (false);
            allMenus[4].SetActive (true);
            allDefaultButtons[2].Select ();
            bindingMenuController.UpdateUI ();
        });

        // BACK BUTTON
        allOtherButtons[3].onClick.AddListener (delegate
        {
            allMenus[1].SetActive (false);
            allMenus[0].SetActive (true);
            allDefaultButtons[0].Select ();
            ApplyValues ();
            SaveSettings ();
        });

        // DEFAULT BUTTON
        allOtherButtons[4].onClick.AddListener (DefaultValues);

        // UI Sounds
        foreach (Button button in labelButtons)
        {
            button.onClick.AddListener (delegate 
            {
                audioController.PlaySFX (audioController.UiSubmit, SFXVolume);
            });
        }

        foreach (Button button in valueButtons)
        {
            button.onClick.AddListener (delegate 
            {
                audioController.PlaySFX (audioController.UiSubmit, SFXVolume);
            });
        }

        for (int index = 0; index < allOtherButtons.Length; index++)
        {
            allOtherButtons[index].onClick.AddListener (delegate 
            {
                AudioClip sound = (index == 3 ? audioController.UiCancel : audioController.UiSubmit);
                audioController.PlaySFX (sound, SFXVolume);
            });
        }
    }

    // Translate labels based on choosed language
    public void TranslateLabels ()
    {
        // CANCELS
        if (!localizationController) { return; }
        List<string> labels = localizationController.GetOptionsLabels ();
        if (labels.Count == 0 || uiLabels.Count == 0) { return; }
        for (int index = 0; index < labels.Count; index++) { uiLabels[index].SetText (labels[index]); }
    }

    //--------------------------------------------------------------------------------//

    private void ToggleAllLabelButtonsInteractable ()
    {
        if (labelButtons.Length == 0) { return; }

        foreach (Button button in labelButtons) { button.interactable = !button.interactable; }
        foreach (Button button in allOtherButtons) { button.interactable = !button.interactable; }
    }

    private void ToggleValueButtonColor (Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = (button.interactable ? Color.yellow : Color.white);
        button.colors = colors;
    }

    //--------------------------------------------------------------------------------//

    public void MakeSelectOnPointerEnter (Button button)
    {
        if (!button && !button.interactable) { return; }
        button.Select ();
    }

    //--------------------------------------------------------------------------------//

    // Controls input resolution
    private void ChooseResolution ()
    {
        if (valueButtons[0].interactable)
        {
            // Horizontal inputs
            if (InputManager.GetButtonDown ("UI_Right", PlayerID.One))
            {
                resolutionIndex++;
                resolutionIndex = (resolutionIndex >= resolutions.Count ? 0 : resolutionIndex);
                valueButtonsTexts[0].text = resolutions[resolutionIndex];
                actualResolution = resolutions[resolutionIndex];
            }
            else if (InputManager.GetButtonDown ("UI_Left", PlayerID.One))
            {
                resolutionIndex--;
                resolutionIndex = (resolutionIndex < 0 ? resolutions.Count - 1 : resolutionIndex);
                valueButtonsTexts[0].text = resolutions[resolutionIndex];
                actualResolution = resolutions[resolutionIndex];
            }

            // Apply effect
            if (InputManager.GetButtonDown ("UI_Right", PlayerID.One) || InputManager.GetButtonDown ("UI_Left", PlayerID.One))
            {
                string[] values = actualResolution.Split ('x');
                width = int.Parse (values[0]);
                height = int.Parse (values[1]);
                ConfigurationsController.SetResolution (width, height, isFullscreen);
            }

            // Case Input is cancel
            if (InputManager.GetButtonDown ("UI_Cancel", PlayerID.One))
            {
                audioController.PlaySFX (audioController.UiCancel, audioController.GetMaxSFXVolume ());
                valueButtons[0].interactable = false;
                ToggleValueButtonColor (valueButtons[0]);
                ToggleAllLabelButtonsInteractable ();
                labelButtons[0].Select ();
            }

            // Case click outside
            if (InputManager.GetMouseButtonDown (0) || InputManager.GetMouseButtonDown (1) || InputManager.GetMouseButtonDown (2))
            {
                valueButtons[0].Select ();
            }
        }
    }

    // Controls input fullscreen
    private void ChooseFullScreen ()
    {
        if (valueButtons[1].interactable)
        {
            // Horizontal inputs
            if (InputManager.GetButtonDown ("UI_Right", PlayerID.One))
            {
                fullScreenIndex++;
                fullScreenIndex = (fullScreenIndex >= fullScreenOptions.Length ? 0 : fullScreenIndex);
                valueButtonsTexts[1].text = fullScreenOptions[fullScreenIndex];
                isFullscreen = (fullScreenOptions[fullScreenIndex] == "Yes");
            }
            else if (InputManager.GetButtonDown ("UI_Left", PlayerID.One))
            {
                fullScreenIndex--;
                fullScreenIndex = (fullScreenIndex < 0 ? fullScreenOptions.Length - 1 : fullScreenIndex);
                valueButtonsTexts[1].text = fullScreenOptions[fullScreenIndex];
                isFullscreen = (fullScreenOptions[fullScreenIndex] == "Yes");
            }

            // Apply effect
            if (InputManager.GetButtonDown ("UI_Right", PlayerID.One) || InputManager.GetButtonDown ("UI_Left", PlayerID.One))
            {
                ConfigurationsController.SetResolution (width, height, isFullscreen);
            }

            // Case Input is cancel
            if (InputManager.GetButtonDown ("UI_Cancel", PlayerID.One))
            {
                audioController.PlaySFX (audioController.UiCancel, audioController.GetMaxSFXVolume ());
                valueButtons[1].interactable = false;
                ToggleValueButtonColor (valueButtons[1]);
                ToggleAllLabelButtonsInteractable ();
                labelButtons[1].Select ();
            }

            // Case click outside
            if (InputManager.GetMouseButtonDown (0) || InputManager.GetMouseButtonDown (1) || InputManager.GetMouseButtonDown (2))
            {
                valueButtons[1].Select ();
                return;
            }
        }
    }

    // Controls input volume
    private void ChooseVolume (Button labelButton, Button volumeButton, TextMeshProUGUI volumeButtonText, AudioSource audioSource)
    {
        if (volumeButton.interactable)
        {
            // Parses
            string volumeText = volumeButtonText.text;
            volumeText = volumeText.Replace ("%", "");
            float volume = float.Parse (volumeText);

            // Horizontal inputs
            if (InputManager.GetButton ("UI_Right", PlayerID.One))
            {
                // Cancels
                if (volume >= 100) { return; }

                timeToWaitUpdateVolume -= Time.fixedDeltaTime;

                if (timeToWaitUpdateVolume <= 0)
                {
                    // Calc
                    volume++;
                    volume = (volume >= 100 ? 100 : volume);
                    volumeButtonText.text = string.Concat (volume, "%");
                    timeToWaitUpdateVolume = startTimeToWaitUpdateVolume;
                }
            }
            else if (InputManager.GetButton ("UI_Left", PlayerID.One))
            {
                // Cancels
                if (volume <= 0) { return; }

                timeToWaitUpdateVolume -= Time.fixedDeltaTime;

                if (timeToWaitUpdateVolume <= 0)
                {
                    // Calc
                    volume--;
                    volume = (volume <= 0 ? 0 : volume);
                    volumeButtonText.text = string.Concat (volume, "%");
                    timeToWaitUpdateVolume = startTimeToWaitUpdateVolume;
                }
            }

            // Config
            if (audioSource == audioSourceBGM)
            {
                BGMVolume = volume / 100f;
                ConfigurationsController.SetAudioSourceVolume (audioSourceBGM, BGMVolume);
            }
            else if (audioSource == audioSourceSFX)
            {
                SFXVolume = volume / 100f;
                ConfigurationsController.SetAudioSourceVolume (audioSourceSFX, SFXVolume);
            }

            // Plays sound only if it's SFX active
            if (InputManager.GetButton ("UI_Left", PlayerID.One) || InputManager.GetButton ("UI_Right", PlayerID.One))
            {
                if (audioSource == audioSourceSFX)
                {
                    timeToPlaySound -= Time.fixedDeltaTime;

                    if (timeToPlaySound <= 0)
                    {
                        audioController.PlaySFX (audioController.PowerUpSound, SFXVolume);
                        timeToPlaySound = startTimeToPlaySound;
                    }
                }
            }

            if (InputManager.GetButtonDown ("UI_Cancel", PlayerID.One))
            {
                audioController.PlaySFX (audioController.UiCancel, SFXVolume);
                volumeButton.interactable = false;
                ToggleValueButtonColor (volumeButton);
                ToggleAllLabelButtonsInteractable ();
                labelButton.Select ();
            }

            // Case click outside
            if (InputManager.GetMouseButtonDown (0) || InputManager.GetMouseButtonDown (1) || InputManager.GetMouseButtonDown (2))
            {
                volumeButton.Select ();
                return;
            }
        }
    }

    // Capture Cancel Button on situations
    private void CaptureCancelButton (GameObject menu)
    {
        if (menu.activeSelf)
        {
            if (InputManager.GetButtonDown ("UI_Cancel"))
            {
                audioController.PlaySFX (audioController.UiCancel, audioController.GetMaxSFXVolume ());
                menu.SetActive (false);
                allMenus[1].SetActive (true);
                allDefaultButtons[1].Select ();
            }
        }
    }

    //--------------------------------------------------------------------------------//
    // MANAGEMENT

    private void GetResolutions ()
    {
        resolutions.Clear ();
        string resolutionsString = FileManager.LoadAsset (FileManager.GetConfigurationFolderPath (), FileManager.GetResolutionsPath ());
        foreach (string res in resolutionsString.Split ('|'))
        {
            if (!string.IsNullOrEmpty (res))  { resolutions.Add (res); }
        }
    }

    private string GetDefaultResolution ()
    {
        Resolution defaultResolution = Screen.currentResolution;
        return string.Concat (defaultResolution.width, "x", defaultResolution.height);
    }

    // Define default values
    private void DefaultValues ()
    {
        actualResolution = GetDefaultResolution ();
        resolutionIndex =  resolutions.IndexOf (actualResolution);
        isFullscreen = true;
        BGMVolume = 0.6f;
        SFXVolume = 0.75f;
        UpdateUI ();
        ApplyValues ();
    }

    // Load settings to PlayPrefs
    private void LoadSettings ()
    {
        actualResolution = PlayerPrefsController.GetResolution ();
        resolutionIndex =  resolutions.IndexOf (actualResolution);
        isFullscreen = (PlayerPrefsController.GetFullScreen () == 1);
        BGMVolume = PlayerPrefsController.GetBGMVolume ();
        SFXVolume = PlayerPrefsController.GetSFXVolume ();

        // Verifies
        if (string.IsNullOrEmpty (actualResolution) || string.IsNullOrWhiteSpace (actualResolution) ||
            resolutionIndex == -1)
        {
            DefaultValues ();
            return;
        }

        audioController.SetMaxBGMVolume (BGMVolume);
        audioController.SetMaxSFXVolume (SFXVolume);

        UpdateUI ();
        ApplyValues ();
    }

    // Save settings to PlayPrefs
    private void SaveSettings ()
    {
        PlayerPrefsController.SetResolution (actualResolution);
        PlayerPrefsController.SetFullScreen (isFullscreen ? 1 : 0);
        PlayerPrefsController.SetBGMVolume (BGMVolume);
        PlayerPrefsController.SetSFXVolume (SFXVolume);
    }

    private void ApplyValues ()
    {
        // Applies Default Changes
        string[] values = actualResolution.Split('x');
        width = int.Parse(values[0]);
        height = int.Parse(values[1]);
        ConfigurationsController.SetResolution(width, height, isFullscreen);

        // Audio
        float divider = (BGMVolume > 1f ? 100f : 1f);
        ConfigurationsController.SetAudioSourceVolume(audioSourceBGM, (BGMVolume / divider));
        divider = (SFXVolume > 1f ? 100f : 1f);
        ConfigurationsController.SetAudioSourceVolume(audioSourceSFX, (SFXVolume / divider));

        audioController.SetMaxBGMVolume (BGMVolume);
        audioController.SetMaxSFXVolume (SFXVolume);
    }

    // Updates UI elements
    private void UpdateUI ()
    {
        valueButtonsTexts[0].text = actualResolution;
        valueButtonsTexts[1].text = isFullscreen ? "Yes" : "No";
        valueButtonsTexts[2].text = string.Concat (BGMVolume * 100, "%");
        valueButtonsTexts[3].text = string.Concat (SFXVolume * 100, "%");
    }
}