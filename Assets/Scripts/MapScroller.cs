using UnityEngine;

public class MapScroller : MonoBehaviour
{
    public float scrollSpeed = 2f;
    private bool scrolling = false;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    public void ResetPosition()
    {
        transform.position = startPos;
    }

    void Update()
    {
        if (!scrolling) return;

        // move the level left continuously
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;
    }

    public void StartScrolling()
    {
        scrolling = true;
    }

    public void StopScrolling()
    {
        scrolling = false;
    }
}

