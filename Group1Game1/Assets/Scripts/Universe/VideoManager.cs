using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    [SerializeField] UniverseController universeController;
    [SerializeField] GameObject videoScreen;
    [SerializeField] VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        videoScreen.SetActive(false);
        universeController.StartMovement();
    }

    public void PlayVideo(VideoClip clip)
    {
        if (videoPlayer.clip == clip && videoPlayer.isPlaying)
            return;
        videoPlayer.clip = clip;
        videoPlayer.Play();
        videoScreen.SetActive(true);
    }
}
