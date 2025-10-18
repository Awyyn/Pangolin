using UnityEngine;

public class DoorController : MonoBehaviour
{
    public PlateIndicator[] indicators; // assign via inspector
    public int requiredPlates;
    public Animator doorAnimator;

    public Transform dustSpawnPoint;
    public GameObject dustOpenPrefab;
    public GameObject dustClosePrefab;

    public Collider2D doorCollider; // assign the collider of the door here

    private int pressedCount = 0;

    private bool isOpen = false;

    private bool lastOpenState = false;

    private void Awake()
    {
        if (doorAnimator == null)
            doorAnimator = GetComponent<Animator>();

        if (doorCollider == null)
            doorCollider = GetComponent<Collider2D>();

        // force initial door state without spawning dust
        isOpen = pressedCount >= requiredPlates;
        lastOpenState = isOpen;
        doorAnimator.SetBool("Open", isOpen);
        if (doorCollider != null) doorCollider.enabled = !isOpen;
    }

    private void UpdateDoorState()
    {
        bool shouldBeOpen = pressedCount >= requiredPlates;

        // only act if the state actually changed
        if (shouldBeOpen != lastOpenState)
        {
            isOpen = shouldBeOpen;
            doorAnimator.SetBool("Open", isOpen);

            if (isOpen)
                SpawnDust(dustOpenPrefab);
            else
                SpawnDust(dustClosePrefab);

            if (doorCollider != null)
                doorCollider.enabled = !isOpen;

            lastOpenState = isOpen;
        }
    }




    public void PlatePressed(PressurePlate plate, bool pressed)
    {
        pressedCount += pressed ? 1 : -1;
        pressedCount = Mathf.Clamp(pressedCount, 0, requiredPlates); // ensure it doesn't go negative or exceed indicators in case more Indicators fire at the same time
        UpdateIndicators();
        UpdateDoorState();
    }

    private void UpdateIndicators()
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            indicators[i].SetActive(i < pressedCount);
        }
    }


    private void SpawnDust(GameObject prefab)
    {
        if (prefab == null) return;

        Vector3 spawnPos = dustSpawnPoint != null ? dustSpawnPoint.position : transform.position;
        Quaternion spawnRot = dustSpawnPoint != null ? dustSpawnPoint.rotation : Quaternion.identity;

        var go = Instantiate(prefab, spawnPos, spawnRot, transform); // parent to door
    }



    public void ResetState()
    {
        // Destroy all running dust animations
        foreach (Transform child in transform)
        {
            if (child.CompareTag("DustFX"))
                Destroy(child.gameObject);
        }


        // Reset door Animator instantly
        if (doorAnimator != null)
            doorAnimator.Play("Closed", 0, 0f); // replace "Closed" with your door closed state name

        // Reset door state
        pressedCount = 0;
        lastOpenState = false;
        isOpen = false;
        UpdateIndicators();

        // Reset collider
        if (doorCollider != null)
            doorCollider.enabled = true;
    }



    private void SetOpen(bool open)
    {
        if (doorAnimator != null) doorAnimator.SetBool("Open", open);
        if (doorCollider != null) doorCollider.enabled = !open;
    }

}





