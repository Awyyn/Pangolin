using System.Collections;
using UnityEngine;

public class Rock : MonoBehaviour, IInteractable
{
    private Vector3 originalPosition;
    public bool rockBlocked = false;

    private void Awake()
    {
        originalPosition = transform.position;
    }

    public void Interact(Vector3 direction)
    {
        rockBlocked = false;

        Vector3 targetPos = transform.position + direction;
        if (GridManager.Instance.CanMoveTo(targetPos))
        {
            StartCoroutine(MoveTo(targetPos));
        }
        else
        {
            rockBlocked = true;
            Debug.Log("Rock blocked!");
        }
    }


    public void ResetState()
    {
        transform.position = originalPosition;
    }

    private IEnumerator MoveTo(Vector3 destination)
    {
        while ((transform.position - destination).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * 5);
            yield return null;
        }
        transform.position = destination;
    }
}
