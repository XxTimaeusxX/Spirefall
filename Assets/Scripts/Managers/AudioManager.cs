using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip mainBGM;
    public AudioClip playerShootSFX;
    public AudioClip enemyDieSFX;

    void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure music source exists
        if (!musicSource)
        {
            var go = new GameObject("MusicSource");
            go.transform.SetParent(transform, false);
            musicSource = go.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.spatialBlend = 0f; // 2D
        }
        
        // Ensure SFX source exists
        if (!sfxSource)
        {
            var go = new GameObject("SFXSource");
            go.transform.SetParent(transform, false);
            sfxSource = go.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.spatialBlend = 0f; // 2D
        }
    }

    // --- Specific Audio Methods ---

    public static void PlayMainBGM() => PlayMusic(Instance?.mainBGM);
    public static void PlayPlayerShoot() => PlaySFX(Instance?.playerShootSFX, 1f);
    public static void PlayEnemyDie() => PlaySFX(Instance?.enemyDieSFX, 1f);


    // --- Core Audio Logic ---

    /// <summary>
    /// Plays background music. Replaces any currently playing music.
    /// </summary>
    private static void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (!Instance || !clip) return;
        Instance.musicSource.clip = clip;
        Instance.musicSource.loop = loop;
        Instance.musicSource.Play();
    }

    /// <summary>
    /// Stops the currently playing background music.
    /// </summary>
    public static void StopMusic()
    {
        if (!Instance) return;
        Instance.musicSource.Stop();
    }

    /// <summary>
    /// Plays a single-shot sound effect. Overlapping calls will play multiple sounds at once.
    /// </summary>
    public static void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (!Instance || !clip) return;
        Instance.sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}
