using UnityEngine;

public class StatueOffering : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool hasActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasActivated && other.CompareTag("Player"))
        {
            hasActivated = true;
            Debug.Log("Player reached the statue.");
            StartOfferingSequence();
        }
    }


    private void StartOfferingSequence()
    {
        Debug.Log("Offering sequence started.");
        animator.SetTrigger("Offer");
    }
}
