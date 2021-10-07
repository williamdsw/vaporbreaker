using UnityEngine;

namespace Controllers.Core
{
    /// <summary>
    /// Controller for Configurations
    /// </summary>
    public class ConfigurationsController : MonoBehaviour
    {
        /// <summary>
        /// Change audio source volume
        /// </summary>
        /// <param name="audioSource"> Instance of Audio Source</param>
        /// <param name="volume"> Volume amount </param>
        public static void SetAudioSourceVolume(AudioSource audioSource, float volume) => audioSource.volume = volume;

        /// <summary>
        /// Show or hide cursor
        /// </summary>
        /// <param name="toShow"></param>
        public static void ToggleCursor(bool toShow) => Cursor.visible = toShow;
    }
}