using System.Collections.Generic;
using UnityEngine;

public class LocalizationController : MonoBehaviour
{
    // Menus
    private List<string> bindingsLabels = new List<string> ();
    private List<string> defaultKeyboardLayoutLabels = new List<string> ();
    private List<string> defaultGamepadLayoutLabels = new List<string> ();
    private List<string> languageMenuLabels = new List<string> ();
    private List<string> levelCompleteLabels = new List<string> ();
    private List<string> levelInformationLabels = new List<string> ();
    private List<string> mainMenuLabels = new List<string> ();
    private List<string> optionsLabels = new List<string> ();
    private List<string> pauseLabels = new List<string> ();
    private List<string> powerUpsSceneLabels = new List<string> ();
    private List<string> pressAnyKeyLabels = new List<string> ();
    private List<string> progressLabels = new List<string> ();
    private List<string> selectLevelsLabels = new List<string> ();
    private List<string> soundtracksLabels = new List<string> ();
    private List<string> titleLabels = new List<string> ();
    private List<string> uiIconsLabels = new List<string> ();

    // Others
    private List<string> levelsNames = new List<string> ();
    private List<string> powerUpsNames = new List<string> ();
    private List<string> powerUpsDescriptions = new List<string> ();

    //--------------------------------------------------------------------------------//
    // GETTERS

    // Menus
    public List<string> GetBindingsLabels () { return bindingsLabels; }
    public List<string> GetDefaultKeyboardLayoutLabels () { return defaultKeyboardLayoutLabels; }
    public List<string> GetDefaultGamepadLayoutLabels () { return defaultGamepadLayoutLabels; }
    public List<string> GetLanguageMenuLabels () { return languageMenuLabels; }
    public List<string> GetLevelCompleteLabels () { return levelCompleteLabels; }
    public List<string> GetLevelInformationLabels () { return levelInformationLabels; }
    public List<string> GetMainMenuLabels () { return mainMenuLabels; }
    public List<string> GetOptionsLabels () { return optionsLabels; }
    public List<string> GetPauseLabels () { return pauseLabels; }
    public List<string> GetPowerUpsSceneLabels () { return powerUpsSceneLabels; }
    public List<string> GetPressAnyKeyLabels () { return pressAnyKeyLabels; }
    public List<string> GetProgressLabels () { return progressLabels; }
    public List<string> GetSelectLevelsLabels () { return selectLevelsLabels; }
    public List<string> GetSoundtracksLabels () { return soundtracksLabels; }
    public List<string> GetTitleLabels () { return titleLabels; }
    public List<string> GetUiIconsLabels () { return uiIconsLabels; }

    // Others
    public List<string> GetLevelsNames () { return levelsNames; }
    public List<string> GetPowerUpsNames () { return powerUpsNames; }
    public List<string> GetPowerUpsDescriptions () { return powerUpsDescriptions; }

    //--------------------------------------------------------------------------------//

    private void Awake () 
    {
        SetupSingleton ();
        DefineLocalization ();
    }

    //--------------------------------------------------------------------------------//

    // Implements singleton
    private void SetupSingleton ()
    {
        int numberOfInstances = FindObjectsOfType (GetType ()).Length;
        if (numberOfInstances > 1)
        {
            DestroyImmediate (this.gameObject);
        }
        else 
        {
            DontDestroyOnLoad (this.gameObject);
        }
    }

    //--------------------------------------------------------------------------------//

    private void ClearLists ()
    {
        // Panels
        bindingsLabels.Clear ();
        defaultKeyboardLayoutLabels.Clear ();
        defaultGamepadLayoutLabels.Clear ();
        languageMenuLabels.Clear ();
        levelCompleteLabels.Clear ();
        levelInformationLabels.Clear ();
        mainMenuLabels.Clear ();
        optionsLabels.Clear ();
        pauseLabels.Clear ();
        powerUpsSceneLabels.Clear ();
        pressAnyKeyLabels.Clear ();
        progressLabels.Clear ();
        selectLevelsLabels.Clear ();
        soundtracksLabels.Clear ();
        titleLabels.Clear ();
        uiIconsLabels.Clear ();

        // Others
        levelsNames.Clear ();
        powerUpsNames.Clear ();
        powerUpsDescriptions.Clear ();
    }

    private void DefineLocalization ()
    {
        string language = PlayerPrefsController.GetLanguage ();
        string folderPath = string.Empty;
        if (string.IsNullOrEmpty (language) || string.IsNullOrWhiteSpace (language))
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.English: { language = SystemLanguage.English.ToString (); break;}
                case SystemLanguage.Italian: { language = SystemLanguage.Italian.ToString (); break;}
                case SystemLanguage.Portuguese: { language = SystemLanguage.Portuguese.ToString (); break;}
                case SystemLanguage.Spanish: { language = SystemLanguage.Spanish.ToString (); break;}
                default: { language = SystemLanguage.English.ToString (); break; }
            }

            PlayerPrefsController.SetLanguage (language);
        }

        switch (language)
        {
            case "English": { folderPath = FileManager.GetLocalizationEnglishFolderPath (); break; }
            case "Italian": { folderPath = FileManager.GetLocalizationItalianFolderPath (); break; }
            case "Portuguese": { folderPath = FileManager.GetLocalizationPortugueseFolderPath (); break; }
            case "Spanish": { folderPath = FileManager.GetLocalizationSpanishFolderPath (); break; }
            default: { folderPath = FileManager.GetLocalizationEnglishFolderPath (); break; }
        }

        LoadLocalization (folderPath);
    }

    public void LoadLocalization (string folderPath)
    {
        ClearLists ();

        // Panels
        string uiLabels = FileManager.LoadAsset (folderPath, FileManager.GetUILabelsPath ());
        string[] panels = uiLabels.Split ('\n');

        foreach (string label in panels[0].Split ('|')) { if (!string.IsNullOrEmpty (label)) { titleLabels.Add (label); } }
        foreach (string label in panels[1].Split ('|')) { if (!string.IsNullOrEmpty (label)) { mainMenuLabels.Add (label); } }
        foreach (string label in panels[2].Split ('|')) { if (!string.IsNullOrEmpty (label)) { progressLabels.Add (label); } }
        foreach (string label in panels[3].Split ('|')) { if (!string.IsNullOrEmpty (label)) { optionsLabels.Add (label); } }
        foreach (string label in panels[4].Split ('|')) { if (!string.IsNullOrEmpty (label)) { defaultKeyboardLayoutLabels.Add (label); } }
        foreach (string label in panels[5].Split ('|')) { if (!string.IsNullOrEmpty (label)) { defaultGamepadLayoutLabels.Add (label); } }
        foreach (string label in panels[6].Split ('|')) { if (!string.IsNullOrEmpty (label)) { bindingsLabels.Add (label); } }
        foreach (string label in panels[7].Split ('|')) { if (!string.IsNullOrEmpty (label)) { uiIconsLabels.Add (label); } }
        foreach (string label in panels[8].Split ('|')) { if (!string.IsNullOrEmpty (label)) { powerUpsSceneLabels.Add (label); } }
        foreach (string label in panels[9].Split ('|')) { if (!string.IsNullOrEmpty (label)) { soundtracksLabels.Add (label); } }
        foreach (string label in panels[10].Split ('|')) { if (!string.IsNullOrEmpty (label)) { selectLevelsLabels.Add (label); } }
        foreach (string label in panels[11].Split ('|')) { if (!string.IsNullOrEmpty (label)) { levelInformationLabels.Add (label); } }
        foreach (string label in panels[12].Split ('|')) { if (!string.IsNullOrEmpty (label)) { levelCompleteLabels.Add (label); } }
        foreach (string label in panels[13].Split ('|')) { if (!string.IsNullOrEmpty (label)) { pauseLabels.Add (label); } }
        foreach (string label in panels[14].Split ('|')) { if (!string.IsNullOrEmpty (label) || !string.IsNullOrWhiteSpace (label)) { languageMenuLabels.Add (label); } }
        if (!string.IsNullOrEmpty (panels[15]) || !string.IsNullOrWhiteSpace (panels[15])) { pressAnyKeyLabels.Add (panels[15]); }

        // Levels Names
        string levelsNamesString = FileManager.LoadAsset (folderPath, FileManager.GetLevelsNamesPath ());
        foreach (string name in levelsNamesString.Split ('\n')) { levelsNames.Add (name); }

        // Power ups names
        string powerUpsNamesString = FileManager.LoadAsset (folderPath, FileManager.GetPowerUpsNamesPath ());
        foreach (string name in powerUpsNamesString.Split ('\n')) { powerUpsNames.Add (name); }

        // Power ups descriptions
        string powerUpsDescriptionsString = FileManager.LoadAsset (folderPath, FileManager.GetPowerUpsDescriptionsPath ());
        foreach (string description in powerUpsDescriptionsString.Split ('\n')) { powerUpsDescriptions.Add (description); }
    }
}