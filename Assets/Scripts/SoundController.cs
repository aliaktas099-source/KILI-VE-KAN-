using UnityEngine;

/// <summary>
/// Oyun içindeki tüm ses efektlerini merkezi olarak yöneten Singleton sýnýf.
/// Saldýrý, yürüme, hasar alma, ölme gibi tüm sesler bu sýnýf üzerinden çalýnýr.
/// </summary>
public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSourceMusic;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip bowShootSound;
    [SerializeField] private AudioClip arrowFlySound;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip takeDamageSound;
    [SerializeField] private AudioClip blockSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hitSound;

    [Header("Volume Settings")]
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 1f;
    [SerializeField][Range(0f, 1f)] private float musicVolume = 0.7f;

    [Header("VFX Audio Source Pool")]
    [SerializeField] private int vfxAudioSourceCount = 5;

    private AudioSource[] vfxAudioSources;
    private int currentVFXIndex = 0;

    private AudioSource loopingAudioSource;
    private float loopingSoundCooldown = 0f;

    private static SoundController instance;

    private void Awake()
    {
        // Singleton pattern uygulamasý
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSourceMusic == null)
            audioSourceMusic = gameObject.AddComponent<AudioSource>();

        CreateVFXAudioSourcePool();

        loopingAudioSource = gameObject.AddComponent<AudioSource>();

        ConfigureAudioSources();
    }

    private void CreateVFXAudioSourcePool()
    {
        vfxAudioSources = new AudioSource[vfxAudioSourceCount];

        for (int i = 0; i < vfxAudioSourceCount; i++)
        {
            vfxAudioSources[i] = gameObject.AddComponent<AudioSource>();
            vfxAudioSources[i].playOnAwake = false;
            vfxAudioSources[i].volume = sfxVolume;
        }
    }

    private void ConfigureAudioSources()
    {
        // Music için ayarlar
        audioSourceMusic.playOnAwake = false;
        audioSourceMusic.volume = musicVolume;
        audioSourceMusic.loop = true;

        // Tekrarlý sesler için ayarlar
        loopingAudioSource.playOnAwake = false;
        loopingAudioSource.volume = sfxVolume;
    }

    private void Update()
    {
        if (loopingSoundCooldown > 0)
            loopingSoundCooldown -= Time.deltaTime;
    }

    public static SoundController Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject soundObject = new GameObject("SoundController");
                instance = soundObject.AddComponent<SoundController>();
            }
            return instance;
        }
    }

    public void PlayAttackSound() => PlayVFXSound(attackSound);
    public void PlayArrowShootSound() => PlayVFXSound(bowShootSound);
    public void PlayArrowFlySound() => PlayVFXSound(arrowFlySound);
    public void PlayWalkSound() => PlayLoopingSound(walkSound);
    public void PlayTakeDamageSound() => PlayVFXSound(takeDamageSound);
    public void PlayBlockSound() => PlayVFXSound(blockSound);
    public void PlayDeathSound() => PlayVFXSound(deathSound);
    public void PlayHitSound() => PlayVFXSound(hitSound);
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null) return;

        audioSourceMusic.clip = musicClip;
        audioSourceMusic.Play();
    }
    public void StopMusic() => audioSourceMusic.Stop();
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        audioSourceMusic.volume = musicVolume;
    }
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        loopingAudioSource.volume = sfxVolume;

        foreach (AudioSource source in vfxAudioSources)
            source.volume = sfxVolume;

    }
    public void PlayCustomVFXSound(AudioClip audioClip)=>PlayVFXSound(audioClip);
    private void PlayLoopingSound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Çalýnacak ses boþ (null)!");
            return;
        }

        // Eðer cooldown içindeyse ses çalma
        if (loopingSoundCooldown > 0)
            return;

        loopingAudioSource.PlayOneShot(clip, sfxVolume);
        loopingSoundCooldown = clip.length; // Ses bitene kadar çal
    }
    private void PlayVFXSound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Çalýnacak ses boþ (null)!");
            return;
        }

        if (!vfxAudioSources[currentVFXIndex].isPlaying)
            vfxAudioSources[currentVFXIndex].PlayOneShot(clip, sfxVolume);

        currentVFXIndex = (currentVFXIndex + 1) % vfxAudioSources.Length;
    }
    public void StopAllSounds()
    {
        loopingAudioSource.Stop();

        foreach (AudioSource source in vfxAudioSources)
            source.Stop();

    }
}
