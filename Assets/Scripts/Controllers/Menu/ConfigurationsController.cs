using UnityEngine;

namespace Controllers.Menu
{
    public class ConfigurationsController : MonoBehaviour
    {
        /// <summary>
        /// Change audio source volume
        /// </summary>
        /// <param name="audioSource"> Instance of Audio Source</param>
        /// <param name="volume"> Volume amount </param>
        public static void SetAudioSourceVolume(AudioSource audioSource, float volume) => audioSource.volume = volume;

        /// <summary>
        /// Change resolution
        /// </summary>
        /// <param name="width"> Desired Width </param>
        /// <param name="height"> Desired Height </param>
        /// <param name="isFullscreen"> Is fullscreen? </param>
        public static void SetResolution(int width, int height, bool isFullscreen) => Screen.SetResolution(width, height, isFullscreen);

        /// <summary>
        /// Show or hide cursor
        /// </summary>
        /// <param name="toShow"></param>
        public static void ToggleCursor(bool toShow)
        {
            Cursor.visible = toShow;
            Cursor.lockState = (toShow ? CursorLockMode.None : CursorLockMode.Locked);
        }
    }
}