using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] RawImage knobNotchX;
    [SerializeField] RawImage knobNotchY;
    [SerializeField] RectTransform rotateTarget;

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

    float currentAngleZ = 0f;
    private void Update()
    {
        //currentAngleZ = Mathf.MoveTowards(currentAngleZ, targetAngleZ, 1 * Time.unscaledDeltaTime);
        //rotateTarget.localRotation = Quaternion.Euler(0, 0, currentAngleZ);

        if (targetAngleZ != 0f)
        {
            //targetAngleZ = Mathf.MoveTowards(targetAngleZ, 0f, 20f * Time.unscaledDeltaTime);
        }
    }

    float lastDragTime;
    float totalX = 0f;
    private void OnXDeltaPixels(float pixelDelta)
    {
        // Right drag = positive; Left drag = negative
        float dx = pixelDelta * xPerPixel;
        universe?.OnDialMove(dx, 0f);
        if (knobNotchX != null)
        {
            totalX += dx;
            knobNotchX.uvRect = new Rect(knobNotchX.uvRect.x, totalX / 500f, 1, 2);
        }

        ApplyTiltFromDrag(dx, 0f);
        lastDragTime = Time.unscaledTime;
    }

    float totalY = 0f;
    private void OnYDeltaPixels(float pixelDelta)
    {
        // Up drag = positive; Down drag = negative
        float dy = pixelDelta * yPerPixel;
        universe?.OnDialMove(0f, dy);
        if (knobNotchY != null)
        {
            totalY += dy;
            knobNotchY.uvRect = new Rect(knobNotchY.uvRect.x, -totalY / 500f, 1, 2);
        }

        ApplyTiltFromDrag(0f, dy);
        lastDragTime = Time.unscaledTime;
    }

    float targetAngleZ = 0f;
    const float invSqrt2 = 0.7f;
    void ApplyTiltFromDrag(float dx, float dy)
    {
        float proj45 = (dx + dy) * invSqrt2;
        float desired = Mathf.Clamp(proj45 * 1, -10, 10);
        targetAngleZ = Mathf.MoveTowards(45, desired, 1 * Time.unscaledDeltaTime);
    }

    private void OnZoomDeltaDegrees(float deltaDegrees)
    {
        // Rotary: CW => negative degrees; UniverseController treats negative zoom as zoom-in.
        float z = deltaDegrees * zoomPerDegree;
        universe?.OnDialZoom(z);
    }
}
