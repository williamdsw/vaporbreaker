using System;
using System.IO;
using UnityEngine;

namespace Utilities
{
    public class FileManager
    {
        public static string ConfigurationFolderPath => "Configuration/";
        public static string LocalizationEnglishFolderPath => "Localization/en/";
        public static string LocalizationItalianFolderPath => "Localization/it/";
        public static string LocalizationPortugueseFolderPath => "Localization/pt-br/";
        public static string LocalizationSpanishFolderPath => "Localization/es/";
        public static string OtherFolderPath => "Other/";
        public static string CreditsPath => "Credits";
        public static string LevelsNamesPath => "LevelsNames";
        public static string PowerUpsDescriptionsPath => "PowerUpsDescriptions";
        public static string PowerUpsNamesPath => "PowerUpsNames";
        public static string ResolutionsPath => "Resolutions";
        public static string UILabelsPath => "UILabels";

        public static string LoadAsset(string folderName, string filePath)
        {
            string newPath = (string.IsNullOrEmpty(folderName) ? filePath : string.Concat(folderName, filePath));
            newPath = string.Concat("Files/", newPath);
            TextAsset textAsset = Resources.Load(newPath) as TextAsset;
            string content = (textAsset ? textAsset.text : string.Empty);
            return content;
        }

        public static bool Exists(string path) => File.Exists(path);

        public static bool Copy(string source, string destination)
        {
            try
            {
                File.Copy(source, destination);
                return Exists(destination);
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("FileManager::Copy -> {0}", ex.Message);
                throw ex;
            }
        }

        public static bool WriteAllBytes(string path, byte[] bytes)
        {
            try
            {
                File.WriteAllBytes(path, bytes);
                return Exists(path);
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("FileManager::WriteAllBytes -> {0}", ex.Message);
                throw ex;
            }
        }
    }
}