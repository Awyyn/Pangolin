using UnityEngine;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{
<<<<<<< HEAD
    public static OptionsManager Instance;
=======
    public static OptionsManager Instance { get; private set; }
>>>>>>> 3b00293 (level menu page fix in progress)

    [Header("Audio")]
    public AudioMixer audioMixer;

    [Header("Panels")]
    public GameObject optionsPanel;

    private void Awake()
    {
<<<<<<< HEAD
        Instance = this;
        optionsPanel.SetActive(false);  // hide by default
=======
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        optionsPanel.SetActive(false);
>>>>>>> 3b00293 (level menu page fix in progress)
    }

    private void Start()
    {
<<<<<<< HEAD
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
=======
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.3f);
>>>>>>> 3b00293 (level menu page fix in progress)

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
<<<<<<< HEAD
=======
    
    public void ToggleOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

>>>>>>> 3b00293 (level menu page fix in progress)

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }
}
