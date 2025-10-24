using UnityEngine;

public class MapScroller : MonoBehaviour
{
    public float scrollSpeed = 2f;
    private Vector3 startPos;
    private bool isScrolling = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        Debug.Log("Scrolling active: " + isScrolling);


        if (isScrolling)
            transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

    }

    public void StartScrolling()
    {
        isScrolling = true;
    }

}
