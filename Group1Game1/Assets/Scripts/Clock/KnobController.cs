using UnityEngine;

public class KnobController : MonoBehaviour
{
    [Header("Dials")]
    [SerializeField] private DialController xDial;       // set Mode = LinearX
    [SerializeField] private DialController yDial;       // set Mode = LinearY
    [SerializeField] private DialController zoomPointer; // set Mode = Rotary

    [Header("Sensitivities")]
    [Tooltip("How much X-axis input per pixel dragged (LinearX).")]
    [SerializeField] private float xPerPixel = 0.01f;

    [Tooltip("How much Y-axis input per pixel dragged (LinearY).")]
    [SerializeField] private float yPerPixel = 0.01f;

    [Tooltip("How much zoom input per degree turned (Rotary). Positive * deltaDegrees applied to UniverseController.")]
    [SerializeField] private float zoomPerDegree = 1.0f;

    [Header("Targets")]
    [SerializeField] private UniverseController universe;

    private void Reset()
    {
        universe = FindAnyObjectByType<UniverseController>();
    }

    private void OnEnable()
    {
        if (xDial) xDial.onDeltaDegrees.AddListener(OnXDeltaPixels);
        if (yDial) yDial.onDeltaDegrees.AddListener(OnYDeltaPixels);
        if (zoomPointer) zoomPointer.onDeltaDegrees.AddListener(OnZoomDeltaDegrees);
    }

    private void OnDisable()
    {
        if (xDial) xDial.onDeltaDegrees.RemoveListener(OnXDeltaPixels);
        if (yDial) yDial.onDeltaDegrees.RemoveListener(OnYDeltaPixels);
        if (zoomPointer) zoomPointer.onDeltaDegrees.RemoveListener(OnZoomDeltaDegrees);
    }

    private void OnXDeltaPixels(float pixelDelta)
    {
        // Right drag = positive; Left drag = negative
        float dx = pixelDelta * xPerPixel;
        universe?.OnDialMove(dx, 0f);
    }

    private void OnYDeltaPixels(float pixelDelta)
    {
        // Up drag = positive; Down drag = negative
        float dy = pixelDelta * yPerPixel;
        universe?.OnDialMove(0f, dy);
    }

    private void OnZoomDeltaDegrees(float deltaDegrees)
    {
        // Rotary: CW => negative degrees; UniverseController treats negative zoom as zoom-in.
        float z = deltaDegrees * zoomPerDegree;
        universe?.OnDialZoom(z);
    }
}
