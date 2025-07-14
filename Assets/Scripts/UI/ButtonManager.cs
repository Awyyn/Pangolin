using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject optionsMenu;

    public void ToggleOptionsMenu()
    {
        // Toggle the active state of the options menu
        optionsMenu.SetActive(!optionsMenu.activeSelf);
    }
}
