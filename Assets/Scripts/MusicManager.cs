using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] soundtrack;
    public AudioMixer audioMixer;

    public Slider musicSlider;
    private float savedVolume;


    private int currentSongIndex = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // make sure we have an audiosource
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        Application.runInBackground = true; // keeps music playing even when Unity window is not active. be mindful of this!!
                                            // load saved volume
        savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);

        // apply saved volume to mixer immediately
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(savedVolume, 0.0001f, 1f)) * 20f);
    }


    private void Start()
    {
        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(savedVolume);
            
        }

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

}