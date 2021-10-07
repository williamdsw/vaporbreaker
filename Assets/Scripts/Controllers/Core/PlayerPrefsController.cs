using UnityEngine;

namespace Controllers.Core
{
    /// <summary>
    /// Controller for PlayerPrefs
    /// </summary>
    public class PlayerPrefsController
    {
        private class Keys
        {
            public static string BackgroundMusicVolume => "BackgroundMusicVolume";
            public static string HasPlayerPrefs => "HasPlayerPrefs";
            public static string Language => "Language";
            public static string MusicEffectsVolume => "MusicEffectsVolume";
            public static string SoundEffectsVolume => "SoundEffectsVolume";
        }

        // || Properties

        /// <summary>
        /// Has stored player prefs?
        /// </summary>
        public static bool HasPlayerPrefs
        {
            get => !string.IsNullOrEmpty(PlayerPrefs.GetString(Keys.HasPlayerPrefs));
            set
            {
                if (!string.IsNullOrEmpty(value.ToString()) && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    PlayerPrefs.SetString(Keys.HasPlayerPrefs, value.ToString());
                }
            }
        }

        /// <summary>
        /// Current selected language
        /// </summary>
        public static string Language
        {
            get => PlayerPrefs.GetString(Keys.Language);
            set
            {
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                {
                    PlayerPrefs.SetString(Keys.Language, value);
                }
            }
        }

        /// <summary>
        /// Background Music volume
        /// </summary>
        public static float BackgroundMusicVolume
        {
            get => PlayerPrefs.GetFloat(Keys.BackgroundMusicVolume);
            set => PlayerPrefs.SetFloat(Keys.BackgroundMusicVolume, value);
        }

        /// <summary>
        /// Music Effects volume
        /// </summary>
        public static float MusicEffectsVolume
        {
            get => PlayerPrefs.GetFloat(Keys.MusicEffectsVolume);
            set => PlayerPrefs.SetFloat(Keys.MusicEffectsVolume, value);
        }

        /// <summary>
        /// Sound Effects volume
        /// </summary>
        public static float SoundEffectsVolume
        {
            get => PlayerPrefs.GetFloat(Keys.SoundEffectsVolume);
            set => PlayerPrefs.SetFloat(Keys.SoundEffectsVolume, value);
        }

        /// <summary>
        /// Delete all player prefs
        /// </summary>
        public static void DeleteAll() => PlayerPrefs.DeleteAll();
    }
}