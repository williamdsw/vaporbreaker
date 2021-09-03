using System.Collections;
using System.Collections.Generic;
using MVC.BL;
using MVC.Models;
using UnityEngine;

public class AudioController : MonoBehaviour
{
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
    [SerializeField] private AudioClip eightiesRiff;
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
    [SerializeField] private AudioClip hitButton;
    [SerializeField] private AudioClip hittingFace;
    [SerializeField] private AudioClip powerUpSound;
    [SerializeField] private AudioClip showUpSound;
    [SerializeField] private AudioClip slamSound;
    [SerializeField] private AudioClip tvSwitch;
    [SerializeField] private AudioClip uiCancel;
    [SerializeField] private AudioClip uiSubmit;

    // Config
    private float maxBGMVolume = 1f;
    private float maxMEVolume = 1f;
    private float maxSFXVolume = 1f;
    private AudioClip nextMusic;
    private bool changeScene;
    private string nextSceneName;
    private bool changeOnMusicEnd = false;
    private bool loopMusic = false;

    // Cached
    private Pause pauseController;
    private TrackBL trackBL;

    //--------------------------------------------------------------------------------//
    // GETTERS / SETTERS

    public float GetMaxBGMVolume() { return maxBGMVolume; }
    public float GetMaxMEVolume() { return maxMEVolume; }
    public float GetMaxSFXVolume() { return maxSFXVolume; }

    public void SetMaxBGMVolume(float volume) { this.maxBGMVolume = volume; }
    public void SetMaxMEVolume(float volume) { this.maxMEVolume = volume; }
    public void SetMaxSFXVolume(float volume) { this.maxSFXVolume = volume; }

    //--------------------------------------------------------------------------------//

    // PROPERTIES

    public static AudioController Instance { get; private set; }

    // BGM
    public AudioClip[] AllLoopedSongs { get { return allLoopedSongs; } }
    public AudioClip[] AllNotLoopedSongs { get { return allNotLoopedSongs; } }

    // ME
    public AudioClip[] AllLogoVoices { get { return allLogoVoices; } }
    public AudioClip[] AllTitleVoices { get { return allTitleVoices; } }
    public AudioClip EightiesRiff { get { return eightiesRiff; } }
    public AudioClip NewScoreEffect { get { return newScoreEffect; } }
    public AudioClip SuccessEffect { get { return successEffect; } }
    public AudioClip TvStatic { get { return tvStatic; } }

    // SFX
    public AudioClip BlipSound { get { return blipSound; } }
    public AudioClip BoomSound { get { return boomSound; } }
    public AudioClip ClickSound { get { return clickSound; } }
    public AudioClip ExplosionSound { get { return explosionSound; } }
    public AudioClip HitButton { get { return hitButton; } }
    public AudioClip HittingFace { get { return hittingFace; } }
    public AudioClip LaserPewSound { get { return laserPewSound; } }
    public AudioClip MetalPingSound { get { return metalPingSound; } }
    public AudioClip PowerUpSound { get { return powerUpSound; } }
    public AudioClip ShowUpSound { get { return showUpSound; } }
    public AudioClip SlamSound { get { return slamSound; } }
    public AudioClip TvSwitch { get { return tvSwitch; } }
    public AudioClip UiCancel { get { return uiCancel; } }
    public AudioClip UiSubmit { get { return uiSubmit; } }

    public AudioSource AudioSourceBGM => audioSourceBGM;
    public AudioSource AudioSourceME => audioSourceME;
    public AudioSource AudioSourceSFX => audioSourceSFX;

    public List<Track> Tracks { get; private set; } = new List<Track>();
    public bool IsSongPlaying { get; set; }

    //--------------------------------------------------------------------------------//

    private void Awake()
    {
        SetupSingleton();
        trackBL = new TrackBL();
    }

    private void Start()
    {
        pauseController = FindObjectOfType<Pause>();
    }

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

    //--------------------------------------------------------------------------------//
    // SFX FUNCTIONS

    // Play one shot of clip
    public void PlaySFX(AudioClip clip, float volume)
    {
        if (!clip) return;
        float temporaryVolume = (volume > maxSFXVolume ? maxSFXVolume : volume);
        AudioSourceSFX.volume = temporaryVolume;
        AudioSourceSFX.PlayOneShot(clip);
    }

    // Play clip at point
    public void PlaySoundAtPoint(AudioClip clip)
    {
        if (!clip) return;
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    //--------------------------------------------------------------------------------//

    // ME FUNCTIONS

    // Plays Music Effect
    public void PlayME(AudioClip clip, float volume, bool loop)
    {
        if (!clip) return;
        float temporaryVolume = (volume > maxMEVolume ? maxMEVolume : volume);
        AudioSourceME.volume = temporaryVolume;
        AudioSourceME.clip = clip;
        AudioSourceME.loop = loop;
        AudioSourceME.Play();
    }

    public void StopME()
    {
        AudioSourceME.Stop();
    }

    //--------------------------------------------------------------------------------//
    // CLIP / SONG PROPERTIES

    // Formats string
    public string FormatMusicName(string musicName)
    {
        if (string.IsNullOrEmpty(musicName) || string.IsNullOrWhiteSpace(musicName))
        {
            return "Music Name";
        }

        musicName = musicName.Replace("__", " - ").Replace("_", " ");
        return musicName;
    }

    public string GetActualMusicName()
    {
        return (AudioSourceBGM.clip ? AudioSourceBGM.clip.name : string.Empty);
    }

    // Get clip length
    public float GetClipLength(AudioClip clip)
    {
        return (clip ? clip.length : 0f);
    }

    //--------------------------------------------------------------------------------//
    // BGM

    public void ChangeMusic(AudioClip nextMusic, bool changeScene, string nextSceneName, bool loopMusic, bool changeOnMusicEnd)
    {
        if (!nextMusic) return;

        this.nextMusic = nextMusic;
        this.changeScene = changeScene;
        this.nextSceneName = nextSceneName;
        this.loopMusic = loopMusic;
        this.changeOnMusicEnd = changeOnMusicEnd;

        StartCoroutine(ChangeMusicCoroutine());
    }

    public void PauseMusic(bool pause)
    {
        if (pause)
        {
            AudioSourceBGM.Pause();
        }
        else
        {
            AudioSourceBGM.UnPause();
        }
    }

    public void RepeatMusic(bool repeat) => AudioSourceBGM.loop = repeat;

    public void StopMusic()
    {
        StopAllCoroutines();
        StartCoroutine(StopMusicCoroutine());
        IsSongPlaying = false;
    }

    //--------------------------------------------------------------------------------//
    // COROUTINES

    private IEnumerator ChangeMusicCoroutine()
    {
        yield return DropVolume();

        // Change and play
        IsSongPlaying = false;
        AudioSourceBGM.volume = 0;
        AudioSourceBGM.clip = nextMusic;
        AudioSourceBGM.loop = loopMusic;
        AudioSourceBGM.Play();
        IsSongPlaying = true;

        // Information to pause controller
        if (!pauseController)
        {
            pauseController = FindObjectOfType<Pause>();
        }

        if (pauseController)
        {
            pauseController.SetActualSongName(FormatMusicName(nextMusic.name));
        }

        // Drops up volume
        yield return GainVolume();

        if (!loopMusic && changeOnMusicEnd)
        {
            // Cancel
            if (!pauseController) yield return null;

            yield return new WaitForSecondsRealtime(AudioSourceBGM.clip.length);
            pauseController.SetPreviousSongName(FormatMusicName(nextMusic.name));
            int index = Random.Range(0, allNotLoopedSongs.Length);
            ChangeMusic(allNotLoopedSongs[index], false, "", false, true);

            // Information to pause controller
            pauseController.SetActualSongName(FormatMusicName(allNotLoopedSongs[index].name));
        }
    }

    private IEnumerator StopMusicCoroutine()
    {
        yield return DropVolume();

        // Change and play
        AudioSourceBGM.volume = 0;
        AudioSourceBGM.Stop();
    }

    private IEnumerator DropVolume()
    {
        for (float volume = maxBGMVolume; volume >= 0; volume -= 0.1f)
        {
            AudioSourceBGM.volume = volume;
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    private IEnumerator GainVolume()
    {
        for (float volume = 0; volume <= maxBGMVolume; volume += 0.1f)
        {
            AudioSourceBGM.volume = volume;
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    public void GetTracks()
    {

        Tracks = trackBL.ListAll();
    }
}