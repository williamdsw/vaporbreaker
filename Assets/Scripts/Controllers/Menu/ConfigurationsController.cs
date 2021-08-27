using UnityEngine;

public class ConfigurationsController : MonoBehaviour
{
    public static void SetAudioSourceVolume(AudioSource audioSource, float volume)
    {
        if (!audioSource) return;
        audioSource.volume = volume;
    }

    public static void SetResolution(int width, int height, bool isFullscreen)
    {
        Screen.SetResolution(width, height, isFullscreen);
    }
}