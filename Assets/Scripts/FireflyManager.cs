using UnityEngine;

public class FireflyManager : MonoBehaviour
{
    public static FireflyManager Instance;

    public int totalFireflies { get; private set; }

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
            return;
        }

        LoadFireflyTotal();
    }

    private void LoadFireflyTotal()
    {
        totalFireflies = PlayerProgress.GetFireflyTotal();
        FireflyCounterUI.Instance?.UpdateCount(totalFireflies);
    }

    public void AddFirefly(int amount)
    {
        totalFireflies += amount;
        PlayerProgress.SetFireflyTotal(totalFireflies);
        FireflyCounterUI.Instance?.UpdateCount(totalFireflies);

        Debug.Log("[FireflyManager] Fireflies: " + totalFireflies);
    }

    public void ResetFireflies()
    {
        totalFireflies = 0;
        PlayerProgress.SetFireflyTotal(0);
        FireflyCounterUI.Instance?.UpdateCount(totalFireflies);

        Debug.Log("[FireflyManager] Fireflies reset.");
    }
}
