using System.Collections;
using System.Collections.Generic;
using MVC.BL;
using MVC.Models;
using UnityEngine;

namespace Controllers.Core
{
    public class AudioController : MonoBehaviour
    {
        // || Inspector References

        [Header("Audio Sources")]
        [SerializeField] private AudioSource audioSourceBGM;
        [SerializeField] private AudioSource audioSourceME;
        [SerializeField] private AudioSource audioSourceSFX;

        [Header("BGM")]
        [SerializeField] private AudioClip[] allLoopedSongs;
        [SerializeField] private AudioClip[] allNotLoopedSongs;

        [Header("ME")]
        [SerializeField] private AudioClip[] allLogoVoices;
        [SerializeField] private AudioClip[] allTitleVoices;
        [SerializeField] private AudioClip newScoreEffect;
        [SerializeField] private AudioClip successEffect;
        [SerializeField] private AudioClip tvStatic;

        [Header("SFX")]
        [SerializeField] private AudioClip blipSound;
        [SerializeField] private AudioClip boomSound;
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private AudioClip explosionSound;
        [SerializeField] private AudioClip laserPewSound;
        [SerializeField] private AudioClip metalPingSound;
        [SerializeField] private AudioClip hittingFace;
        [SerializeField] private AudioClip powerUpSound;
        [SerializeField] private AudioClip showUpSound;
        [SerializeField] private AudioClip slamSound;
        [SerializeField] private AudioClip tvSwitch;
        [SerializeField] private AudioClip uiCancel;
        [SerializeField] private AudioClip uiSubmit;

        // || State

        private AudioClip nextTrack;
        private string nextSceneName;
        private bool changeScene;
        private bool changeOnTrackEnd = false;
        private bool isToLoopTrack = false;

        // || Cached

        private TrackBL trackBL;

        // || Properties

        public static AudioController Instance { get; private set; }

        // Config
        public float MaxBGMVolume { get; set; } = 1f;
        public float MaxMEVolume { get; set; } = 1f;
        public float MaxSFXVolume { get; set; } = 1f;

        // BGM
        public AudioClip[] AllLoopedSongs => allLoopedSongs;
        public AudioClip[] AllNotLoopedSongs => allNotLoopedSongs;

        // ME
        public AudioClip[] AllLogoVoices => allLogoVoices;
        public AudioClip[] AllTitleVoices => allTitleVoices;
        public AudioClip NewScoreEffect => newScoreEffect;
        public AudioClip SuccessEffect => successEffect;
        public AudioClip TvStatic => tvStatic;

        // SFX
        public AudioClip BlipSound => blipSound;
        public AudioClip BoomSound => boomSound;
        public AudioClip ClickSound => clickSound;
        public AudioClip ExplosionSound => explosionSound;
        public AudioClip HittingFace => hittingFace;
        public AudioClip LaserPewSound => laserPewSound;
        public AudioClip MetalPingSound => metalPingSound;
        public AudioClip PowerUpSound => powerUpSound;
        public AudioClip ShowUpSound => showUpSound;
        public AudioClip SlamSound => slamSound;
        public AudioClip TvSwitch => tvSwitch;
        public AudioClip UiCancel => uiCancel;
        public AudioClip UiSubmit => uiSubmit;

        public AudioSource AudioSourceBGM => audioSourceBGM;
        public AudioSource AudioSourceME => audioSourceME;
        public AudioSource AudioSourceSFX => audioSourceSFX;

        public List<Track> Tracks { get; private set; } = new List<Track>();
        public bool IsSongPlaying { get; set; }

        private void Awake()
        {
            SetupSingleton();
            trackBL = new TrackBL();
        }

        /// <summary>
        /// Setup singleton instance
        /// </summary>
        private void SetupSingleton()
        {
            int numberOfInstances = FindObjectsOfType(GetType()).Length;
            if (numberOfInstances > 1)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Play SFX at volume
        /// </summary>
        /// <param name="clip"> Clip to be played </param>
        /// <param name="volume"> Volume amount </param>
        public void PlaySFX(AudioClip clip, float volume)
        {
            float temporaryVolume = (volume > MaxSFXVolume ? MaxSFXVolume : volume);
            AudioSourceSFX.volume = temporaryVolume;
            AudioSourceSFX.PlayOneShot(clip);
        }

        /// <summary>
        /// Play ME at volume with loop
        /// </summary>
        /// <param name="clip"> Clip to be played </param>
        /// <param name="volume"> Volume amount </param>
        /// <param name="toLoop"> Is to loop ? </param>
        public void PlayME(AudioClip clip, float volume, bool toLoop)
        {
            float temporaryVolume = (volume > MaxMEVolume ? MaxMEVolume : volume);
            AudioSourceME.volume = temporaryVolume;
            AudioSourceME.clip = clip;
            AudioSourceME.loop = toLoop;
            AudioSourceME.Play();
        }

        /// <summary>
        /// Stops the current ME
        /// </summary>
        public void StopME()
        {
            AudioSourceME.Stop();
            audioSourceME.loop = false;
        }

        /// <summary>
        /// Get the duration of a track
        /// </summary>
        /// <param name="clip"></param>
        /// <returns> Duration of the track </returns>
        public float GetClipLength(AudioClip clip) => (clip ? clip.length : 0f);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextMusic"></param>
        /// <param name="changeScene"></param>
        /// <param name="nextSceneName"></param>
        /// <param name="loopMusic"></param>
        /// <param name="changeOnMusicEnd"></param>
        public void ChangeMusic(AudioClip nextMusic, bool changeScene, string nextSceneName, bool loopMusic, bool changeOnMusicEnd)
        {
            this.nextTrack = nextMusic;
            this.changeScene = changeScene;
            this.nextSceneName = nextSceneName;
            this.isToLoopTrack = loopMusic;
            this.changeOnTrackEnd = changeOnMusicEnd;

            StartCoroutine(ChangeMusicCoroutine());
        }

        /// <summary>
        /// Do all process to change current track
        /// </summary>
        private IEnumerator ChangeMusicCoroutine()
        {
            yield return DropVolume();

            // Change and play
            IsSongPlaying = false;
            AudioSourceBGM.volume = 0;
            AudioSourceBGM.clip = nextTrack;
            AudioSourceBGM.loop = isToLoopTrack;
            AudioSourceBGM.Play();
            IsSongPlaying = true;

            if (Pause.Instance)
            {
                //pauseController.SetActualSongName(FormatMusicName(nextMusic.name));
            }

            // Drops up volume
            yield return GainVolume();

            if (!isToLoopTrack && changeOnTrackEnd)
            {
                // Cancel
                if (!Pause.Instance) yield return null;

                yield return new WaitForSecondsRealtime(AudioSourceBGM.clip.length);
                //pauseController.SetPreviousSongName(FormatMusicName(nextMusic.name));
                int index = Random.Range(0, allNotLoopedSongs.Length);
                ChangeMusic(allNotLoopedSongs[index], false, "", false, true);

                // Information to pause controller
                //pauseController.SetActualSongName(FormatMusicName(allNotLoopedSongs[index].name));
            }
        }

        /// <summary>
        /// Fade in volume to zero
        /// </summary>
        private IEnumerator DropVolume()
        {
            for (float volume = MaxBGMVolume; volume >= 0; volume -= 0.1f)
            {
                AudioSourceBGM.volume = volume;
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }

        /// <summary>
        /// Fade out volume to MaxBGMVolume
        /// </summary>
        private IEnumerator GainVolume()
        {
            for (float volume = 0; volume <= MaxBGMVolume; volume += 0.1f)
            {
                AudioSourceBGM.volume = volume;
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }

        /// <summary>
        /// Pause or unpause current track
        /// </summary>
        /// <param name="isToPause"> Is to pause current track ? </param>
        public void PauseMusic(bool isToPause)
        {
            if (isToPause)
            {
                AudioSourceBGM.Pause();
            }
            else
            {
                AudioSourceBGM.UnPause();
            }
        }

        /// <summary>
        /// Toggle track's loop property
        /// </summary>
        /// <param name="isToRepeat"> Is to repeat current track ? </param>
        public void ToggleRepeatTrack(bool isToRepeat) => AudioSourceBGM.loop = isToRepeat;

        /// <summary>
        /// Stop current track
        /// </summary>
        public void StopMusic()
        {
            StopAllCoroutines();
            StartCoroutine(StopMusicCoroutine());
            IsSongPlaying = false;
        }

        /// <summary>
        /// Stop current track
        /// </summary>
        private IEnumerator StopMusicCoroutine()
        {
            yield return DropVolume();

            AudioSourceBGM.volume = 0;
            AudioSourceBGM.Stop();
        }

        /// <summary>
        /// List all tracks
        /// </summary>
        public void GetTracks() => Tracks = trackBL.ListAll();
    }
}