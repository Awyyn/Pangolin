using UnityEngine;

public class FireflyManager : MonoBehaviour
{
    public static FireflyManager Instance;

    public int totalFireflies = 0;

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

        // Ensure UI is updated immediately
        if (FireflyCounterUI.Instance != null)
            FireflyCounterUI.Instance.UpdateCount(totalFireflies);
    }


    public void AddFirefly(int amount)
    {
        totalFireflies += amount;
        if (FireflyCounterUI.Instance != null) FireflyCounterUI.Instance.UpdateCount(totalFireflies);

        Debug.Log("Fireflies: " + totalFireflies);
    }

    public bool SpendFireflies(int amount)
    {
        if (totalFireflies < amount)
            return false;

        totalFireflies -= amount;
        Debug.Log("Spent " + amount + ", remaining: " + totalFireflies);
        return true;
    }

}
