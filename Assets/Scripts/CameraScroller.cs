using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    public static CameraScroller Instance { get; private set; }

    [Header("Scroll")]
    public float scrollSpeed = 1.15f;

    private Transform camTransform;
    private Vector3 startPosition;
    private bool isScrolling = false;

    // Convenience check
    public bool IsScrolling() => isScrolling;
    
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
    }

    // Stop horizontal scrolling (keeps camera where it is)
    public void StopScrolling()
    {
        if (!isScrolling) return;
        isScrolling = false;
    }

    // Reset camera to initial position and clear boss mode
    public void ResetCamera()
    {
        isScrolling = false;
        camTransform.position = startPosition;
        if (GameManager.Instance != null) GameManager.Instance.bossMode = false;
    }
    
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