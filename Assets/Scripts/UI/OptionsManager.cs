using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class OptionsManager : MonoBehaviour
{

    public static OptionsManager Instance { get; private set; }
    public Slider musicSlider;
    public Slider sfxSlider;

    public AudioMixer audioMixer;
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
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxVol   = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        musicSlider.SetValueWithoutNotify(musicVol);
        sfxSlider.SetValueWithoutNotify(sfxVol);

        ApplyMusicVolume(musicVol);
        ApplySFXVolume(sfxVol);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }


    public void SetMusicVolume(float value) // called by slider
    {
        ApplyMusicVolume(value);

        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    private void ApplyMusicVolume(float value) // used internally on load
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MusicVolume", dB);
    }


    public void SetSFXVolume(float value)
    {
        ApplySFXVolume(value);

        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }
        private void ApplySFXVolume(float value) 
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("SFXVolume", dB);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }
    
    public void ToggleOptions() // used by the gear icon button
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }
    

}
