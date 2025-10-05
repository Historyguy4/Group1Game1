using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class OnVideoEnd : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private UIFade uiFade;
    [SerializeField] private string sceneToLoad = "MainMenu";
    [SerializeField] private float fadeTime = 1f;

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted += OnPrepared;
            videoPlayer.Prepare();
        }
    }

    private void OnPrepared(VideoPlayer vp)
    {
        StartCoroutine(FadeAtEnd(vp));
        vp.Play();
    }

    private IEnumerator FadeAtEnd(VideoPlayer vp)
    {
        yield return new WaitForSecondsRealtime(fadeTime);

        uiFade.FadeIn(() =>
        {
            SceneManager.LoadScene(sceneToLoad);
        });
    }
}
