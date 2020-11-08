using UnityEngine;

public class ConfigurationsController : MonoBehaviour
{
    //--------------------------------------------------------------------------------//
    // AUDIO CONFIGURATIONS

    public static void SetAudioSourceVolume(AudioSource audioSource, float volume)
    {
        if (!audioSource)
        {
            return;
        }

        audioSource.volume = volume;
    }

    //--------------------------------------------------------------------------------//
    // VIDEO CONFIGURATIONS

    public static void SetResolution(int width, int height, bool isFullscreen)
    {
        Screen.SetResolution(width, height, isFullscreen);
    }
}