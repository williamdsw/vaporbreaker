using MVC.BL;
using MVC.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private AudioClip tvStaticEffect;
        [SerializeField] private AudioClip fireEffect;

        [Header("SFX")]
        [SerializeField] private AudioClip blipSound;
        [SerializeField] private AudioClip boomSound;
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private AudioClip explosionSound;
        [SerializeField] private AudioClip hittingFaceSound;
        [SerializeField] private AudioClip hittingWallSound;
        [SerializeField] private AudioClip laserPewSound;
        [SerializeField] private AudioClip powerUpSound;
        [SerializeField] private AudioClip showUpSound;
        [SerializeField] private AudioClip slamSound;
        [SerializeField] private AudioClip tvSwitchSound;
        [SerializeField] private AudioClip uiCancelSound;
        [SerializeField] private AudioClip uiSubmitSound;

        // || State

        private AudioClip nextTrackClip;
        private Track nextTrackInfo;
        private string nextSceneName;
        private bool isToChangeScene;
        private bool isToChangeOnTrackEnd = false;
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
        public AudioClip TvStaticEffect => tvStaticEffect;
        public AudioClip FireEffect => fireEffect;

        // SFX
        public AudioClip BlipSound => blipSound;
        public AudioClip BoomSound => boomSound;
        public AudioClip ClickSound => clickSound;
        public AudioClip ExplosionSound => explosionSound;
        public AudioClip HittingFaceSound => hittingFaceSound;
        public AudioClip HittingWallSound => hittingWallSound;
        public AudioClip LaserPewSound => laserPewSound;
        public AudioClip PowerUpSound => powerUpSound;
        public AudioClip ShowUpSound => showUpSound;
        public AudioClip SlamSound => slamSound;
        public AudioClip TvSwitchSound => tvSwitchSound;
        public AudioClip UiCancelSound => uiCancelSound;
        public AudioClip UiSubmitSound => uiSubmitSound;

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
            if (FindObjectsOfType(GetType()).Length > 1)
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
        /// Play SFX at camera
        /// </summary>
        /// <param name="clip"> Clip to be played </param>
        /// <param name="volume"> Volume amount </param>
        public void PlaySoundAtPoint(AudioClip clip, float volume)
        {
            float temporaryVolume = (volume > MaxSFXVolume ? MaxSFXVolume : volume);
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, temporaryVolume);
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
        /// Pass values to change music
        /// </summary>
        /// <param name="nextTrackClip"> Next track to be played </param>
        /// <param name="isToChangeScene"> Is to change scene ? </param>
        /// <param name="nextSceneName"> Next scene name </param>
        /// <param name="isToLoopTrack"> Is to loop current track ? </param>
        /// <param name="isToChangeOnTrackEnd"> Is to change on track ending ? </param>
        /// <param name="nextTrackInfo"> Next track information </param>
        public void ChangeMusic(AudioClip nextTrackClip, bool isToChangeScene, string nextSceneName, bool isToLoopTrack, bool isToChangeOnTrackEnd, Track nextTrackInfo = null)
        {
            this.nextTrackClip = nextTrackClip;
            this.isToChangeScene = isToChangeScene;
            this.nextSceneName = nextSceneName;
            this.isToLoopTrack = isToLoopTrack;
            this.isToChangeOnTrackEnd = isToChangeOnTrackEnd;
            this.nextTrackInfo = nextTrackInfo;

            StartCoroutine(ChangeMusicCoroutine());
        }

        /// <summary>
        /// Do all process to change current track
        /// </summary>
        private IEnumerator ChangeMusicCoroutine()
        {
            yield return DropVolume();

            IsSongPlaying = false;
            AudioSourceBGM.volume = 0;
            AudioSourceBGM.clip = nextTrackClip;
            AudioSourceBGM.loop = isToLoopTrack;
            AudioSourceBGM.Play();
            IsSongPlaying = true;

            if (PauseController.Instance && nextTrackInfo != null)
            {
                PauseController.Instance.SetTrackInfo(nextTrackInfo);
            }

            yield return GainVolume();

            if (!isToLoopTrack && isToChangeOnTrackEnd && PauseController.Instance)
            {
                yield return new WaitForSecondsRealtime(AudioSourceBGM.clip.length);
                int index = Random.Range(0, allNotLoopedSongs.Length);
                AudioClip nextClip = allNotLoopedSongs[index];
                Track nextRandomTrack = Tracks.Find(t => t.FileName.Equals(nextClip.name));
                ChangeMusic(nextClip, false, string.Empty, false, true, nextRandomTrack);
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