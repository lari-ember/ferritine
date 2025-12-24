using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

/// <summary>
/// Centralized audio manager for UI sounds, SFX, music, and ambient audio.
/// Supports multi-channel audio mixing and spatial audio.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Mixer")]
    public AudioMixer mainMixer;
    
    [Header("UI / Buttons")]
    public AudioClip buttonClick;
    public AudioClip buttonToggle;
    public AudioClip panelOpen;
    public AudioClip panelClose;
    
    [Header("Entity / World")]
    public AudioClip entitySelect;
    public AudioClip teleportWoosh;
    
    [Header("Toasts / Feedback")]
    public AudioClip toastInfo;
    public AudioClip toastSuccess;
    public AudioClip toastError;
    
    [Header("Audio Clips (Legacy Arrays)")]
    public AudioClip[] uiClips;
    public AudioClip[] sfxClips;
    public AudioClip[] musicClips;
    
    [Header("Pool Settings")]
    [Tooltip("Number of AudioSources to pool per channel")]
    public int audioSourcePoolSize = 5;
    
    // Audio source pools organized by mixer group
    private Dictionary<string, Queue<AudioSource>> audioSourcePools = new Dictionary<string, Queue<AudioSource>>();
    private Dictionary<string, List<AudioSource>> activeAudioSources = new Dictionary<string, List<AudioSource>>();
    
    // Audio clip dictionaries for fast lookup
    private Dictionary<string, AudioClip> uiClipDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxClipDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> musicClipDict = new Dictionary<string, AudioClip>();
    
    // Mixer groups
    private AudioMixerGroup uiGroup;
    private AudioMixerGroup sfxGroup;
    private AudioMixerGroup musicGroup;
    private AudioMixerGroup ambientGroup;
    
    void Awake()
    {
        // Singleton pattern with DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        InitializeAudioSystem();
    }
    
    /// <summary>
    /// Initializes the audio system.
    /// </summary>
    void InitializeAudioSystem()
    {
        // Get mixer groups
        if (mainMixer != null)
        {
            AudioMixerGroup[] groups = mainMixer.FindMatchingGroups("Master");
            foreach (var group in groups)
            {
                if (group.name == "UI") uiGroup = group;
                else if (group.name == "SFX") sfxGroup = group;
                else if (group.name == "Music") musicGroup = group;
                else if (group.name == "Ambient") ambientGroup = group;
            }
        }
        
        // Initialize pools
        InitializePool("UI", uiGroup);
        InitializePool("SFX", sfxGroup);
        InitializePool("Music", musicGroup);
        InitializePool("Ambient", ambientGroup);
        
        // Build clip dictionaries
        BuildClipDictionary(uiClips, uiClipDict);
        BuildClipDictionary(sfxClips, sfxClipDict);
        BuildClipDictionary(musicClips, musicClipDict);
        
        // Load saved settings
        LoadSettings();
        
        Debug.Log("[AudioManager] Initialized successfully");
    }
    
    /// <summary>
    /// Initializes an audio source pool for a specific channel.
    /// </summary>
    void InitializePool(string channelName, AudioMixerGroup mixerGroup)
    {
        audioSourcePools[channelName] = new Queue<AudioSource>();
        activeAudioSources[channelName] = new List<AudioSource>();
        
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = mixerGroup;
            source.playOnAwake = false;
            audioSourcePools[channelName].Enqueue(source);
        }
    }
    
    /// <summary>
    /// Builds a dictionary from an array of clips for fast lookup.
    /// </summary>
    void BuildClipDictionary(AudioClip[] clips, Dictionary<string, AudioClip> dict)
    {
        if (clips == null) return;
        
        foreach (var clip in clips)
        {
            if (clip != null)
            {
                dict[clip.name] = clip;
            }
        }
    }
    
    /// <summary>
    /// Gets an audio source from the pool.
    /// </summary>
    AudioSource GetAudioSource(string channelName)
    {
        if (!audioSourcePools.ContainsKey(channelName))
        {
            Debug.LogWarning($"[AudioManager] Unknown channel: {channelName}");
            return null;
        }
        
        AudioSource source;
        
        if (audioSourcePools[channelName].Count > 0)
        {
            source = audioSourcePools[channelName].Dequeue();
        }
        else
        {
            // Create new source if pool exhausted
            source = gameObject.AddComponent<AudioSource>();
            AudioMixerGroup group = channelName == "UI" ? uiGroup : 
                                   channelName == "SFX" ? sfxGroup :
                                   channelName == "Music" ? musicGroup : ambientGroup;
            source.outputAudioMixerGroup = group;
            source.playOnAwake = false;
        }
        
        activeAudioSources[channelName].Add(source);
        return source;
    }
    
    /// <summary>
    /// Returns an audio source to the pool.
    /// </summary>
    void ReturnAudioSource(string channelName, AudioSource source)
    {
        if (source == null || !audioSourcePools.ContainsKey(channelName)) return;
        
        source.Stop();
        source.clip = null;
        source.spatialBlend = 0f;
        
        activeAudioSources[channelName].Remove(source);
        audioSourcePools[channelName].Enqueue(source);
    }
    
    /// <summary>
    /// Plays an AudioClip directly on the UI channel.
    /// Usage: AudioManager.Instance?.Play(AudioManager.Instance.buttonClick);
    /// </summary>
    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        PlayClipOnChannel(clip, "UI");
    }
    
    /// <summary>
    /// Internal method to play a clip on a specific channel.
    /// </summary>
    void PlayClipOnChannel(AudioClip clip, string channelName)
    {
        if (clip == null) return;
        
        AudioSource source = GetAudioSource(channelName);
        if (source == null) return;
        
        source.clip = clip;
        source.loop = false;
        source.spatialBlend = 0f;
        source.Play();
        
        StartCoroutine(ReturnAfterPlayback(channelName, source, clip.length));
    }
    
    /// <summary>
    /// Plays a UI sound effect.
    /// </summary>
    public static void PlayUISound(string clipName)
    {
        if (Instance == null) return;
        Instance.PlaySound(clipName, "UI", Instance.uiClipDict, 0f);
    }
    
    /// <summary>
    /// Plays a spatial SFX sound.
    /// </summary>
    public static void PlaySFX(string clipName, Vector3 position, float spatialBlend = 1f)
    {
        if (Instance == null) return;
        Instance.PlaySound(clipName, "SFX", Instance.sfxClipDict, spatialBlend, position);
    }
    
    /// <summary>
    /// Plays a music track.
    /// </summary>
    public static void PlayMusic(string clipName, bool loop = true)
    {
        if (Instance == null) return;
        Instance.PlaySound(clipName, "Music", Instance.musicClipDict, 0f, Vector3.zero, loop);
    }
    
    /// <summary>
    /// Internal method to play a sound.
    /// </summary>
    void PlaySound(string clipName, string channelName, Dictionary<string, AudioClip> clipDict, 
                   float spatialBlend = 0f, Vector3 position = default, bool loop = false)
    {
        if (!clipDict.ContainsKey(clipName))
        {
            Debug.LogWarning($"[AudioManager] Clip not found: {clipName}");
            return;
        }
        
        AudioClip clip = clipDict[clipName];
        AudioSource source = GetAudioSource(channelName);
        
        if (source == null) return;
        
        source.clip = clip;
        source.loop = loop;
        source.spatialBlend = spatialBlend;
        
        if (spatialBlend > 0f)
        {
            source.transform.position = position;
        }
        
        source.Play();
        
        // Auto-return to pool when finished (if not looping)
        if (!loop)
        {
            StartCoroutine(ReturnAfterPlayback(channelName, source, clip.length));
        }
    }
    
    /// <summary>
    /// Coroutine to return audio source after playback completes.
    /// </summary>
    System.Collections.IEnumerator ReturnAfterPlayback(string channelName, AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        ReturnAudioSource(channelName, source);
    }
    
    /// <summary>
    /// Sets master volume.
    /// </summary>
    public static void SetMasterVolume(float volume)
    {
        if (Instance?.mainMixer != null)
        {
            Instance.mainMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }
    }
    
    /// <summary>
    /// Sets UI channel volume.
    /// </summary>
    public static void SetUIVolume(float volume)
    {
        if (Instance?.mainMixer != null)
        {
            Instance.mainMixer.SetFloat("UIVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("UIVolume", volume);
        }
    }
    
    /// <summary>
    /// Sets SFX channel volume.
    /// </summary>
    public static void SetSFXVolume(float volume)
    {
        if (Instance?.mainMixer != null)
        {
            Instance.mainMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }
    }
    
    /// <summary>
    /// Sets Music channel volume.
    /// </summary>
    public static void SetMusicVolume(float volume)
    {
        if (Instance?.mainMixer != null)
        {
            Instance.mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
    }
    
    /// <summary>
    /// Loads audio settings from PlayerPrefs.
    /// </summary>
    void LoadSettings()
    {
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float uiVol = PlayerPrefs.GetFloat("UIVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        
        SetMasterVolume(masterVol);
        SetUIVolume(uiVol);
        SetSFXVolume(sfxVol);
        SetMusicVolume(musicVol);
    }
    
    /// <summary>
    /// Stops all sounds in a channel.
    /// </summary>
    public static void StopAllInChannel(string channelName)
    {
        if (Instance == null || !Instance.activeAudioSources.ContainsKey(channelName)) return;
        
        List<AudioSource> sources = new List<AudioSource>(Instance.activeAudioSources[channelName]);
        foreach (var source in sources)
        {
            Instance.ReturnAudioSource(channelName, source);
        }
    }
}

