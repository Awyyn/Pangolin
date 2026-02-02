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

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    void Start()
    {
        if (musicSlider == null || sfxSlider == null || audioMixer == null)
        {
            Debug.LogError("OptionsManager: Missing references in Inspector!");
            return;
        }

        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxVol   = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        musicSlider.SetValueWithoutNotify(musicVol);
        sfxSlider.SetValueWithoutNotify(sfxVol);

        ApplyMusicVolume(musicVol);
        ApplySFXVolume(sfxVol);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }


    public void SetMusicVolume(float value)
    {
        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        ApplySFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }


    private void ApplyMusicVolume(float value) // used internally on load
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MusicVolume", dB);
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
        PlayerPrefs.Save();   // Save once when closing
        optionsPanel.SetActive(false);
    }
    

}
