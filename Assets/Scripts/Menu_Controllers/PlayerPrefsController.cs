using UnityEngine;

public class PlayerPrefsController
{
    // keys
    private const string BGM_VOLUME_KEY = "BGM_Volume_Key";
    private const string FULLSCREEN_KEY = "Fullscreen";
    private const string LANGUAGE_KEY = "Language";
    private const string RESOLUTION_KEY = "Resolution";
    private const string SFX_VOLUME_KEY = "SFX_Volume_Key";

    //values
    private const float BGM_MIN_VOLUME = 0f;
    private const float BGM_MAX_VOLUME = 1f;
    private const float SFX_MIN_VOLUME = 0f;
    private const float SFX_MAX_VOLUME = 1f;

    //--------------------------------------------------------------------------------//

    // GETTERS / SETTERS

    public static float GetBGMVolume () { return PlayerPrefs.GetFloat (BGM_VOLUME_KEY); }
    public static float GetSFXVolume () { return PlayerPrefs.GetFloat (SFX_VOLUME_KEY); }
    public static int GetFullScreen () { return PlayerPrefs.GetInt (FULLSCREEN_KEY); }
    public static bool HasPlayerPrefs () { return !string.IsNullOrEmpty (PlayerPrefs.GetString (RESOLUTION_KEY)); }
    public static string GetLanguage () { return PlayerPrefs.GetString (LANGUAGE_KEY); }
    public static string GetResolution () { return PlayerPrefs.GetString (RESOLUTION_KEY); }

    public static void SetBGMVolume (float volume)
    {
        if (volume >= BGM_MIN_VOLUME && volume <= BGM_MAX_VOLUME)
        {
            PlayerPrefs.SetFloat (BGM_VOLUME_KEY, volume);
        }
    }

    public static void SetSFXVolume (float volume)
    {
        if (volume >= SFX_MIN_VOLUME && volume <= SFX_MAX_VOLUME)
        {
            PlayerPrefs.SetFloat (SFX_VOLUME_KEY, volume);
        }
    }

    public static void SetFullScreen (int fullScreen)
    {
        if (fullScreen == 0 || fullScreen == 1)
        {
            PlayerPrefs.SetInt (FULLSCREEN_KEY, fullScreen);
        }
    }

    public static void SetLanguage (string language)
    {
        if (!string.IsNullOrEmpty (language) && !string.IsNullOrWhiteSpace (language))
        {
            PlayerPrefs.SetString (LANGUAGE_KEY, language);    
        }
    }

    public static void SetResolution (string resolution)
    {
        if (!string.IsNullOrEmpty (resolution) && !string.IsNullOrWhiteSpace (resolution) && resolution.Contains ("x"))
        {
            PlayerPrefs.SetString (RESOLUTION_KEY, resolution);    
        }
    }
}