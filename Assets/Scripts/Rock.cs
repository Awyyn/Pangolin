using System.Collections;
using UnityEngine;

public class Rock : MonoBehaviour, IInteractable
{
    private Vector3 originalPosition;
    private bool initialized = false;
    public bool rockBlocked = false;

    private void Awake()
    {
        originalPosition = transform.position;
    }

    private void Start()
    {
        Initialize(); // run once when spawned
    }

    public void Initialize()
    {
        if (initialized) return; // prevent double init
        initialized = true;

        // Run any setup that must happen immediately
        originalPosition = transform.position;
        rockBlocked = false;

        Debug.Log("[Rock] Initialize done at time " + Time.time);

        // Run delayed setup only once
        StartCoroutine(InitializeDelayed());
    }

    private IEnumerator InitializeDelayed()
    {
        yield return null; // wait 1 frame
        Debug.Log("[Rock] Initialize done (delayed) at time " + Time.time);
    }

    public void Interact(Vector3 direction)
    {
        Debug.Log("[Rock] Interact called with dir=" + direction + " at time " + Time.time);

        rockBlocked = false;
        Vector3 targetPos = transform.position + direction;

        // Debug info
        var cell = GridManager.Instance.groundTilemap.WorldToCell(targetPos);
        var ground = GridManager.Instance.groundTilemap.GetTile(cell);
        var obstacle = GridManager.Instance.obstacleTilemap.GetTile(cell);
        Debug.Log($"[Rock] Trying move to {targetPos}, Cell={cell}, Ground={ground}, Obstacle={obstacle}");

        if (GridManager.Instance.CanMoveTo(targetPos, GetComponent<Collider2D>()))
        {
            StartCoroutine(MoveTo(targetPos));
        }
        else
        {
            rockBlocked = true;
            Debug.Log("[Rock] Move blocked at " + targetPos);
        }
    }

    public void ResetState()
    {
        StopAllCoroutines();
        rockBlocked = false;
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
