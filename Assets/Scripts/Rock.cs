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

        transform.position = GridManager.Instance.groundTilemap.GetCellCenterWorld(
        GridManager.Instance.groundTilemap.WorldToCell(transform.position)
        );


        // Run any setup that must happen immediately
        originalPosition = transform.position;
        rockBlocked = false;

        // Run delayed setup only once
        StartCoroutine(InitializeDelayed());
    }

    private IEnumerator InitializeDelayed()
    {
        yield return null; // wait 1 frame
    }

    public void Interact(Vector3 direction)
    {

        rockBlocked = false;
        Vector3 targetPos = transform.position + direction;

        // Debug info
        var cell = GridManager.Instance.groundTilemap.WorldToCell(targetPos);
        var ground = GridManager.Instance.groundTilemap.GetTile(cell);
        var obstacle = GridManager.Instance.obstacleTilemap.GetTile(cell);
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
    /* causes offset of the rock over time
    private IEnumerator MoveTo(Vector3 destination)
    {
        while ((transform.position - destination).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * 5);
            yield return null;
        }
        transform.position = destination; //old line, less precission, but works pretty well, just offsets rock over time
        //while ((transform.position - destination).sqrMagnitude > 0.0001f) ; //this literally made Unity crash

    }
    */
    /* works, snaps
    private IEnumerator MoveTo(Vector3 destination)
    {
        Vector3 start = transform.position;
        float t = 0f;
        float speed = 8f;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(start, destination, t);
            yield return null;
        }

        // snap exactly to grid center
        transform.position = GridManager.Instance.groundTilemap.GetCellCenterWorld(
            GridManager.Instance.groundTilemap.WorldToCell(destination)
        );
    }
    */

    private IEnumerator MoveTo(Vector3 destination)
    {
        // Snap destination to grid center
        Vector3Int cell = GridManager.Instance.groundTilemap.WorldToCell(destination);
        destination = GridManager.Instance.groundTilemap.GetCellCenterWorld(cell);

        Vector3 start = transform.position;
        float t = 0f;
        float duration = 0.15f;
        float speed = 1f / duration;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            float eased = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector3.Lerp(start, destination, eased);
            yield return null;
        }

        // Force precise grid alignment at the end
        transform.position = destination;
    }



}

