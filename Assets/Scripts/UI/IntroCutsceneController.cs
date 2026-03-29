using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class IntroCutsceneController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    
    public void PlayVideo()
    {
        gameObject.SetActive(true);

        if (videoPlayer != null)
        {
            videoPlayer.targetCameraAlpha = 1f;    // make it visible again
            videoPlayer.url = Path.Combine(Application.streamingAssetsPath, "Intro.mp4");
            videoPlayer.prepareCompleted += OnPrepared;
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Prepare();
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FinishCutscene();
        }
        if (videoPlayer != null && videoPlayer.isPrepared && !videoPlayer.isPlaying)
        {
            // Video has finished
            FinishCutscene();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        FinishCutscene();
    }

    private void FinishCutscene()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();                   // stops playback
            videoPlayer.targetCameraAlpha = 0f;   // hide video from camera
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.prepareCompleted -= OnPrepared;
        }

        // Hide cutscene GameObject
        gameObject.SetActive(false);

        // Notify GameManager
        GameManager.Instance.OnIntroFinished();
    }
    

    private void OnPrepared(VideoPlayer vp)
    {
        vp.Play();
    }
    
}


/* this worked when I was using curtain and not url for the Intro
 
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
}*/