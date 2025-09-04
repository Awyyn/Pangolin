using UnityEngine;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager instance;

    [Header("Audio")]
    public AudioMixer audioMixer; // assign your mixer here

    [Header("Panels")]
    public GameObject optionsPanel; // assign your UI panel

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // load saved volumes and apply to mixer
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);

        if (optionsPanel) optionsPanel.SetActive(false);
    }

    public void SetMusicVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MusicVolume", dB);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }


    public void SetSFXVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("SFXVolume", dB);
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    public void OpenOptions() => optionsPanel.SetActive(true);
    public void CloseOptions() => optionsPanel.SetActive(false);
}
