using UnityEngine;
using UnityEngine.Video;

public class IntroCutsceneController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    void Start()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;

        gameObject.SetActive(false); // start hidden
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FinishCutscene();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        FinishCutscene();
    }

    void FinishCutscene()
    {
        // Tell GameManager that the intro finished
        GameManager.Instance.OnIntroFinished();

        // Disable this cutscene object
        gameObject.SetActive(false);
    }
}