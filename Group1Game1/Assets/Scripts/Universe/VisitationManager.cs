using UnityEngine;
using UnityEngine.SceneManagement;

public class VisitationManager : MonoBehaviour
{
    [SerializeField] UniverseController universeController;
    [SerializeField] LocationManager locationManager;
    [SerializeField] VideoManager videoManager;
    [SerializeField] float xyDistance;
    [SerializeField] float zDistance;
    [SerializeField] UIFade uiFade;

    void Update()
    {
        foreach (var location in locationManager.Locations)
        {
            if (location.InRange(universeController.UniversePosition, xyDistance, xyDistance, zDistance))
            {
                universeController.StopMovement();

                var video = location.visitLocation();

                uiFade.FadeIn(() =>
                {
                    videoManager.PlayVideo(video);
                    uiFade.FadeOut();
                });
            }
        }
    }
}
