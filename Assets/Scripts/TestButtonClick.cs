using UnityEngine;
using UnityEngine.UI; // Button component

public class TestButtonClick : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        Debug.Log("Restart Button Clicked!");
    }
}
