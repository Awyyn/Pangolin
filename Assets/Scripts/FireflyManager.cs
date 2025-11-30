using UnityEngine;

public class FireflyManager : MonoBehaviour
{
    public static FireflyManager Instance;

    private const string FireflyKey = "TotalFireflies";
    public int totalFireflies { get; private set; } = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadFireflies();
        UpdateUI();
    }

    public void AddFirefly(int amount)
    {
        totalFireflies += amount;
        SaveFireflies();
        UpdateUI();
        Debug.Log("Fireflies: " + totalFireflies);
    }

    public void ResetFireflies()
    {
        totalFireflies = 0;
        SaveFireflies();
        UpdateUI();
        Debug.Log("Fireflies reset!");
    }

    private void SaveFireflies()
    {
        PlayerPrefs.SetInt(FireflyKey, totalFireflies);
        PlayerPrefs.Save();
    }

    private void LoadFireflies()
    {
        totalFireflies = PlayerPrefs.GetInt(FireflyKey, 0);
    }

    private void UpdateUI()
    {
        if (FireflyCounterUI.Instance != null)
            FireflyCounterUI.Instance.UpdateCount(totalFireflies);
    }
}
