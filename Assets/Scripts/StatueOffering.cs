using UnityEngine;

public class StatueOffering : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
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
