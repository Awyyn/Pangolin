using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundtrackManager : MonoBehaviour
{
    public Slider musicSlider;
    public AudioMixer audioMixer;

    private AudioSource audioSource;
    private float savedVolume;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // keep music manager across scenes

        audioSource = GetComponent<AudioSource>();

        // load saved volume first
        savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f); // default 0.5
        SetVolume(savedVolume); // apply to mixer
    }

    private void Start()
    {
        // update slider to reflect saved value
        if (musicSlider != null)
        {
            musicSlider.value = savedVolume;

            // add listener after initial value is applied
            musicSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    public void SetVolume(float value)
    {
        // apply to audiomixer
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MusicVolume", dB);

        // save value for next launch
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }
}
