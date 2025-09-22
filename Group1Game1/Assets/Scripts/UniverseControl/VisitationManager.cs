using UnityEngine;

public class VisitationManager : MonoBehaviour
{
    [SerializeField] UniverseController universeController;
    [SerializeField] LocationManager locationManager;
    [SerializeField] VideoManager videoManager;
    [SerializeField] float xyDistance;
    [SerializeField] float zDistance;

    void Update()
    {
        foreach (var location in locationManager.Locations)
        {
            if (location.InRange(universeController.UniversePosition, xyDistance, xyDistance, zDistance))
            {
                universeController.StopMovement();
                videoManager.PlayVideo(location.visitLocation());
            }
        }
    }
}
