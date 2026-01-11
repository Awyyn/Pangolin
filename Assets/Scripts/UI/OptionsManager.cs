using UnityEngine;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{

    public static OptionsManager Instance { get; private set; }

    [Header("Audio")]
    public AudioMixer audioMixer;

    [Header("Panels")]
    public GameObject optionsPanel;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        optionsPanel.SetActive(false);
    }

    private void Start()
    {

        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.3f);

        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);
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

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }
    
    public void ToggleOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }
}
