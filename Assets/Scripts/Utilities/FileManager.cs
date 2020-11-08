using UnityEngine;

public class FileManager
{
    // Folders
    private const string CONFIGURATION_FOLDER_PATH = "Configuration/";
    private const string LOCALIZATION_ENGLISH_FOLDER_PATH = "Localization/en/";
    private const string LOCALIZATION_ITALIAN_FOLDER_PATH = "Localization/it/";
    private const string LOCALIZATION_PORTUGUESE_FOLDER_PATH = "Localization/pt-br/";
    private const string LOCALIZATION_SPANISH_FOLDER_PATH = "Localization/es/";
    private const string OTHER_FOLDER_PATH = "Other/";

    // Files
    private const string CREDITS_PATH = "Credits";
    private const string LEVELS_NAMES_PATH = "LevelsNames";
    private const string POWER_UPS_DESCRIPTIONS_PATH = "PowerUpsDescriptions";
    private const string POWER_UPS_NAMES_PATH = "PowerUpsNames";
    private const string RESOLUTIONS_PATH = "Resolutions";
    private const string UI_LABELS_PATH = "UILabels";

    //--------------------------------------------------------------------------------//
    // GETTERS

    // Folders
    public static string GetConfigurationFolderPath () { return CONFIGURATION_FOLDER_PATH; }
    public static string GetLocalizationEnglishFolderPath () { return LOCALIZATION_ENGLISH_FOLDER_PATH; }
    public static string GetLocalizationItalianFolderPath () { return LOCALIZATION_ITALIAN_FOLDER_PATH; }
    public static string GetLocalizationPortugueseFolderPath () { return LOCALIZATION_PORTUGUESE_FOLDER_PATH; }
    public static string GetLocalizationSpanishFolderPath () { return LOCALIZATION_SPANISH_FOLDER_PATH; }
    public static string GetOtherFolderPath () { return OTHER_FOLDER_PATH; }

    // Files
    public static string GetCreditsPath () { return CREDITS_PATH; }
    public static string GetLevelsNamesPath () { return LEVELS_NAMES_PATH; }
    public static string GetPowerUpsDescriptionsPath () { return POWER_UPS_DESCRIPTIONS_PATH; }
    public static string GetPowerUpsNamesPath () { return POWER_UPS_NAMES_PATH; }
    public static string GetResolutionsPath () { return RESOLUTIONS_PATH; }
    public static string GetUILabelsPath () { return UI_LABELS_PATH; }

    //--------------------------------------------------------------------------------//

    // Loads an file
    public static string LoadAsset (string folderName, string filePath)
    {
        // Config
        string newPath = (string.IsNullOrEmpty (folderName) ? filePath : string.Concat (folderName, filePath));
        newPath = string.Concat ("Files/", newPath);
        TextAsset textAsset = Resources.Load (newPath) as TextAsset;
        string content = (textAsset ? textAsset.text : string.Empty);
        return content;
    }
}