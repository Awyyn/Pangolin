using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource audioSource;
    public AudioClip[] soundtrack;
    public AudioMixer audioMixer;

    private int currentSongIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        Application.runInBackground = true;

        // Load and APPLY saved volume
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        ApplyVolume(savedVolume);
    }

    private void Start()
    {
        if (soundtrack.Length > 0)
            PlayNextSong();
    }

    private void Update()
    {
        if (!audioSource.isPlaying && soundtrack.Length > 0)
            PlayNextSong();
    }

    private void PlayNextSong()
    {
        audioSource.clip = soundtrack[currentSongIndex];
        audioSource.Play();
        currentSongIndex = (currentSongIndex + 1) % soundtrack.Length;
    }

    // Called by OptionsManager
    public void ApplyVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MusicVolume", dB);
    }
}
