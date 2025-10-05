using System;
using UnityEngine;
using UnityEngine.Video;
using static UnityEngine.Rendering.DebugManager;

public class VideoManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] UniverseController universeController;
    [SerializeField] GameObject videoScreen;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] LocationManager locationManager;

    [Header("UI")]
    [SerializeField] GameObject interactionPanel; // assign your InteractionPanel here
    [SerializeField] UIFade uiFade;

    void Awake()
    {
        if (videoPlayer) videoPlayer.loopPointReached += OnVideoFinished;

        // Ensure panel starts hidden
        if (interactionPanel && interactionPanel.activeSelf) interactionPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (videoPlayer) videoPlayer.loopPointReached -= OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer source)
    {

        uiFade.FadeIn(() =>
        {
            if (videoScreen) videoScreen.SetActive(false);

            // Show the interaction panel and freeze player/world input until user closes it
            if (interactionPanel) interactionPanel.SetActive(true);

            if (universeController) universeController.StopMovement();

            uiFade.FadeOut();
        });
    }

    public void PlayVideo(VideoClip clip)
    {
        if (!videoPlayer || clip == null) return;

        if (videoPlayer.clip == clip && videoPlayer.isPlaying) return;

        videoPlayer.clip = clip;
        videoPlayer.Play();

        if (videoScreen) videoScreen.SetActive(true);

        // Hide panel while playing (if it was left open)
        if (interactionPanel && interactionPanel.activeSelf) interactionPanel.SetActive(false);
    }

    // Hook this to the InteractionPanel's button OnClick in the Inspector
    public void CloseInteractionPanel()
    {
        if (interactionPanel) interactionPanel.SetActive(false);
        if (universeController) universeController.StartMovement();

        locationManager.CheckWin();
    }
}
