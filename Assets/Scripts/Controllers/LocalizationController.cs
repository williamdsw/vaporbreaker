using System.Collections.Generic;
using UnityEngine;

public class LocalizationController : MonoBehaviour
{
    // Menus
    [SerializeField] private List<string> bindingsLabels = new List<string>();
    private List<string> defaultKeyboardLayoutLabels = new List<string>();
    private List<string> defaultGamepadLayoutLabels = new List<string>();
    private List<string> languageMenuLabels = new List<string>();
    private List<string> levelCompleteLabels = new List<string>();
    private List<string> levelInformationLabels = new List<string>();
    private List<string> mainMenuLabels = new List<string>();
    private List<string> optionsLabels = new List<string>();
    private List<string> pauseLabels = new List<string>();
    private List<string> powerUpsSceneLabels = new List<string>();
    private List<string> pressAnyKeyLabels = new List<string>();
    private List<string> progressLabels = new List<string>();
    private List<string> selectLevelsLabels = new List<string>();
    private List<string> soundtracksLabels = new List<string>();
    private List<string> titleLabels = new List<string>();
    private List<string> uiIconsLabels = new List<string>();

    // Others
    private List<string> levelsNames = new List<string>();
    private List<string> powerUpsNames = new List<string>();
    private List<string> powerUpsDescriptions = new List<string>();

    // Menus
    public List<string> GetBindingsLabels()
    {
        return bindingsLabels;
    }

    public List<string> GetDefaultKeyboardLayoutLabels()
    {
        return defaultKeyboardLayoutLabels;
    }

    public List<string> GetDefaultGamepadLayoutLabels()
    {
        return defaultGamepadLayoutLabels;
    }

    public List<string> GetLanguageMenuLabels()
    {
        return languageMenuLabels;
    }

    public List<string> GetLevelCompleteLabels()
    {
        return levelCompleteLabels;
    }

    public List<string> GetLevelInformationLabels()
    {
        return levelInformationLabels;
    }

    public List<string> GetMainMenuLabels()
    {
        return mainMenuLabels;
    }

    public List<string> GetOptionsLabels()
    {
        return optionsLabels;
    }

    public List<string> GetPauseLabels()
    {
        return pauseLabels;
    }

    public List<string> GetPowerUpsSceneLabels()
    {
        return powerUpsSceneLabels;
    }

    public List<string> GetPressAnyKeyLabels()
    {
        return pressAnyKeyLabels;
    }

    public List<string> GetProgressLabels()
    {
        return progressLabels;
    }

    public List<string> GetSelectLevelsLabels()
    {
        return selectLevelsLabels;
    }

    public List<string> GetSoundtracksLabels()
    {
        return soundtracksLabels;
    }

    public List<string> GetTitleLabels()
    {
        return titleLabels;
    }

    public List<string> GetUiIconsLabels()
    {
        return uiIconsLabels;
    }

    // Others
    public List<string> GetLevelsNames()
    {
        return levelsNames;
    }

    public List<string> GetPowerUpsNames()
    {
        return powerUpsNames;
    }

    public List<string> GetPowerUpsDescriptions()
    {
        return powerUpsDescriptions;
    }

    private void Awake()
    {
        SetupSingleton();
        DefineLocalization();
    }

    private void SetupSingleton()
    {
        int numberOfInstances = FindObjectsOfType(GetType()).Length;
        if (numberOfInstances > 1)
        {
            DestroyImmediate(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void ClearLists()
    {
        // Panels
        bindingsLabels.Clear();
        defaultKeyboardLayoutLabels.Clear();
        defaultGamepadLayoutLabels.Clear();
        languageMenuLabels.Clear();
        levelCompleteLabels.Clear();
        levelInformationLabels.Clear();
        mainMenuLabels.Clear();
        optionsLabels.Clear();
        pauseLabels.Clear();
        powerUpsSceneLabels.Clear();
        pressAnyKeyLabels.Clear();
        progressLabels.Clear();
        selectLevelsLabels.Clear();
        soundtracksLabels.Clear();
        titleLabels.Clear();
        uiIconsLabels.Clear();

        // Others
        levelsNames.Clear();
        powerUpsNames.Clear();
        powerUpsDescriptions.Clear();
    }

    private void DefineLocalization()
    {
        string language = PlayerPrefsController.GetLanguage();
        string folderPath = string.Empty;
        if (string.IsNullOrEmpty(language) || string.IsNullOrWhiteSpace(language))
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.English: default: { language = SystemLanguage.English.ToString(); break; }
                case SystemLanguage.Italian: { language = SystemLanguage.Italian.ToString(); break; }
                case SystemLanguage.Portuguese: { language = SystemLanguage.Portuguese.ToString(); break; }
                case SystemLanguage.Spanish: { language = SystemLanguage.Spanish.ToString(); break; }
            }

            PlayerPrefsController.SetLanguage(language);
        }

        switch (language)
        {
            case "English": default: { folderPath = FileManager.LocalizationEnglishFolderPath; break; }
            case "Italian": { folderPath = FileManager.LocalizationItalianFolderPath; break; }
            case "Portuguese": { folderPath = FileManager.LocalizationPortugueseFolderPath; break; }
            case "Spanish": { folderPath = FileManager.LocalizationSpanishFolderPath; break; }
        }

        LoadLocalization(folderPath);
    }

    public void LoadLocalization(string folderPath)
    {
        ClearLists();

        // Panels
        string uiLabels = FileManager.LoadAsset(folderPath, FileManager.UILabelsPath);
        string[] panels = uiLabels.Split('\n');

        List<string>[] labelsList = {
            titleLabels, mainMenuLabels, progressLabels, optionsLabels, defaultKeyboardLayoutLabels,
            defaultGamepadLayoutLabels, bindingsLabels, uiIconsLabels, powerUpsSceneLabels, soundtracksLabels,
            selectLevelsLabels, levelInformationLabels, levelCompleteLabels, pauseLabels, languageMenuLabels,
            pressAnyKeyLabels
        };

        for (int index = 0; index < panels.Length; index++)
        {
            bool toSplit = (index != 15);
            SplitToLabels(panels[index], labelsList[index], toSplit);
        }

        // Levels Names
        string levelsNamesString = FileManager.LoadAsset(folderPath, FileManager.LevelsNamesPath);
        foreach (string name in levelsNamesString.Split('\n'))
        {
            levelsNames.Add(name);
        }

        // Power ups names
        string powerUpsNamesString = FileManager.LoadAsset(folderPath, FileManager.PowerUpsNamesPath);
        foreach (string name in powerUpsNamesString.Split('\n'))
        {
            powerUpsNames.Add(name);
        }

        // Power ups descriptions
        string powerUpsDescriptionsString = FileManager.LoadAsset(folderPath, FileManager.PowerUpsDescriptionsPath);
        foreach (string description in powerUpsDescriptionsString.Split('\n'))
        {
            powerUpsDescriptions.Add(description);
        }
    }

    private void SplitToLabels(string line, List<string> fields, bool toSplit)
    {
        if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line) || fields == null) return;

        if (toSplit)
        {
            foreach (string label in line.Split('|'))
            {
                if (!string.IsNullOrEmpty(label) && !string.IsNullOrWhiteSpace(label))
                {
                    fields.Add(label);
                }
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(line) && !string.IsNullOrWhiteSpace(line))
            {
                fields.Add(line);
            }
        }
    }
}