using UnityEngine;
using UnityEngine.UI; 

public class SFXVolumeControl : MonoBehaviour
{
    public AudioSource sfxAudioSource; // Assign the AudioSource with SFX sounds
    public Slider sfxSlider; // Drag the slider for SFX volume control

    void Start()
    {
        // Set the initial value of the slider to match the current volume
        sfxSlider.value = sfxAudioSource.volume;
    }

    public void OnSFXVolumeChange()
    {
        sfxAudioSource.volume = sfxSlider.value; // Adjust the volume based on slider
    }
}
