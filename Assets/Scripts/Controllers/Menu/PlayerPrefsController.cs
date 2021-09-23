using UnityEngine;

namespace Controllers.Menu
{
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

        public static float BackgroundMusicVolume
        {
            get => PlayerPrefs.GetFloat(Keys.BackgroundMusicVolume);
            set => PlayerPrefs.SetFloat(Keys.BackgroundMusicVolume, value);
        }

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

        public static float MusicEffectsVolume
        {
            get => PlayerPrefs.GetFloat(Keys.MusicEffectsVolume);
            set => PlayerPrefs.SetFloat(Keys.MusicEffectsVolume, value);
        }

        public static float SoundEffectsVolume
        {
            get => PlayerPrefs.GetFloat(Keys.SoundEffectsVolume);
            set => PlayerPrefs.SetFloat(Keys.SoundEffectsVolume, value);
        }
    }
}