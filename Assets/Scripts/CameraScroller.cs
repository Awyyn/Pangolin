using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    public static CameraScroller Instance { get; private set; }

    [Header("Scroll")]
    public float scrollSpeed = 1.5f;

    private Transform camTransform;
    private Vector3 startPosition;
    private bool isScrolling = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        camTransform = Camera.main != null ? Camera.main.transform : transform;
        startPosition = camTransform.position;
    }

    void Update()
    {
        if (!isScrolling) return;
        camTransform.position += Vector3.right * scrollSpeed * Time.deltaTime;
    }

    // Start horizontal scrolling
    public void StartScrolling()
    {
        if (isScrolling) return;
        isScrolling = true;
        Debug.Log("[CameraScroller] StartScrolling()");
    }

    // Stop horizontal scrolling (keeps camera where it is)
    public void StopScrolling()
    {
        if (!isScrolling) return;
        isScrolling = false;
        Debug.Log("[CameraScroller] StopScrolling()");
    }

    // Reset camera to initial position and clear boss mode
    public void ResetCamera()
    {
        isScrolling = false;
        camTransform.position = startPosition;
        if (GameManager.Instance != null) GameManager.Instance.bossMode = false;
        Debug.Log("[CameraScroller] ResetCamera() to " + startPosition);
    }

    // Convenience check
    public bool IsScrolling() => isScrolling;
}


/*




using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 2f;
    public bool scrolling = false; // must stay false by default

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (!scrolling) return;
        transform.position += Vector3.right * scrollSpeed * Time.deltaTime;
    }

    public void StartScrolling() => scrolling = true;
    public void StopScrolling() => scrolling = false;
    public void ResetCamera()
    {
        scrolling = false;
        transform.position = startPosition;
    }

}
*/