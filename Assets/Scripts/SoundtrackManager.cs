using UnityEngine;
using UnityEngine.UI;

public class SoundtrackManager : MonoBehaviour
{
    public AudioClip[] soundtrack;  // Array to hold your songs
    private AudioSource audioSource;

    private int currentSongIndex = 0; // Track the current song

    public Slider musicSlider;  // Reference to your slider
    private float savedVolume;  // To store the saved volume for the music

    private void Start()
    {
        Application.runInBackground = true;                                                        // BE MINDFUL OF THIS. Keeps playing songs when the window is not active

        audioSource = GetComponent<AudioSource>();
        PlayNextSong();  // Play the first song at the start

        // Initialize the music volume slider if it exists
        if (musicSlider != null)
        {
            savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);  // Default volume is 50%
            musicSlider.value = savedVolume;

            // Add listener for slider value change
            musicSlider.onValueChanged.AddListener(AdjustVolume);
        }

        // Set the initial music volume
        audioSource.volume = savedVolume;
    }

    private void Update()
    {
        // Check if the song has finished playing
        if (!audioSource.isPlaying)
        {
            PlayNextSong(); // Play the next song when the current one ends
        }
    }

    // Play the next song in the array
    private void PlayNextSong()
    {
        if (soundtrack.Length == 0) return;

        audioSource.clip = soundtrack[currentSongIndex];
        audioSource.Play();

        currentSongIndex++;
        if (currentSongIndex >= soundtrack.Length)
        {
            currentSongIndex = 0;  // Restart the soundtrack when it reaches the end
        }
    }

    // Method to adjust the volume based on the slider's value
    private void AdjustVolume(float volume)
    {
        audioSource.volume = volume;

        // Save the music volume for the next game start
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
}
