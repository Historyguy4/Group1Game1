using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class DialController : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public enum DialMode { Rotary, LinearX, LinearY }
    [System.Serializable] public class FloatEvent : UnityEvent<float> { } // Rotary: degrees; Linear: pixels

    [Header("Mode")]
    [SerializeField] private DialMode mode = DialMode.Rotary;

    [Header("Visuals (optional)")]
    [SerializeField] private RectTransform pointer;

    [Header("Jitter Control (Rotary)")]
    [Tooltip("Ignore tiny angular steps (degrees)")]
    [SerializeField] private float angularDeadzoneDeg = 0.6f;
    [Tooltip("Extra 'stickiness' after a move starts; prevents micro back-and-forth flips")]
    [SerializeField] private float angularHysteresisDeg = 0.4f;
    [Tooltip("Visual smoothing time (seconds) for the pointer hand")]
    [SerializeField] private float pointerSmoothingTime = 0.06f;

    [Header("Jitter Control (Linear)")]
    [Tooltip("Ignore tiny pixel steps in Linear modes")]
    [SerializeField] private float linearDeadzonePx = 1.5f;

    [Header("Output")]
    public FloatEvent onDeltaDegrees;

    private RectTransform _rt;
    private Camera _pressCam;

    // Rotary state
    private Vector2 _screenCenter;        // stable screen-space center
    private float _startAngle;            // angle at pointer down
    private float _lastRawAngle;          // last raw angle
    private float _displayAngle;          // smoothed angle used for pointer visual
    private float _pointerStartZ;
    private bool _hasLast;

    // Hysteresis
    private float _lastEmittedAngle;      // last angle at which we emitted
    private float _holdDir = 0f;          // sign of last meaningful rotation (-1,0,1)

    // Linear state
    private Vector2 _lastScreenPos;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressCam = eventData.pressEventCamera;

        if (mode == DialMode.Rotary)
        {
            // Compute a reliable screen-space center of this dial
            _screenCenter = RectTransformUtility.WorldToScreenPoint(_pressCam, _rt.position);

            _startAngle = GetScreenAngle(eventData.position);
            _lastRawAngle = _startAngle;
            _lastEmittedAngle = _startAngle;
            _displayAngle = _startAngle;
            _pointerStartZ = pointer ? pointer.localEulerAngles.z : 0f;
            _hasLast = true;
            _holdDir = 0f;

            // Ensure the pointer starts aligned with the current angle
            if (pointer)
                pointer.localRotation = Quaternion.Euler(0f, 0f, _pointerStartZ + 0f);
        }
        else
        {
            _lastScreenPos = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mode == DialMode.Rotary)
        {
            float now = GetScreenAngle(eventData.position);
            if (_hasLast)
            {
                // Raw step from last angle (signed shortest path)
                float rawStep = Mathf.DeltaAngle(_lastRawAngle, now);

                // Deadzone: ignore micro steps
                if (Mathf.Abs(rawStep) < angularDeadzoneDeg)
                {
                    // still update visual smoothing toward current raw angle
                    SmoothPointerToward(now);
                    return;
                }

                // Establish direction & hysteresis (prevents quick flip-flop)
                float dir = Mathf.Sign(rawStep);
                if (_holdDir == 0f) _holdDir = dir; // latch initial direction
                float fromLastEmit = Mathf.DeltaAngle(_lastEmittedAngle, now);

                // Require movement to exceed (deadzone + hysteresis) when reversing direction
                if (Mathf.Sign(fromLastEmit) != _holdDir && Mathf.Abs(fromLastEmit) < (angularDeadzoneDeg + angularHysteresisDeg))
                {
                    SmoothPointerToward(now);
                    _lastRawAngle = now;
                    return;
                }

                // Emit meaningful delta relative to last emit (keeps events stable)
                float emitDelta = Mathf.DeltaAngle(_lastEmittedAngle, now);
                onDeltaDegrees?.Invoke(emitDelta);

                _lastEmittedAngle = now;
                _lastRawAngle = now;
                _holdDir = Mathf.Sign(emitDelta == 0f ? _holdDir : emitDelta);

                // Pointer visual = smoothed absolute angle from start
                SmoothPointerToward(now);
            }
            else
            {
                _lastRawAngle = now;
                _hasLast = true;
                SmoothPointerToward(now);
            }
        }
        else
        {
            // Linear slide: use pixel delta along the main axis with a small deadzone
            Vector2 curr = eventData.position;
            Vector2 step = curr - _lastScreenPos;
            float signed = (mode == DialMode.LinearX) ? step.x : step.y;

            if (Mathf.Abs(signed) >= linearDeadzonePx)
            {
                onDeltaDegrees?.Invoke(signed); // pixels
                _lastScreenPos = curr;
            }
        }
    }

    private float GetScreenAngle(Vector2 screenPos)
    {
        Vector2 v = screenPos - _screenCenter; // vector from center to cursor (screen-space)
        // 0° = right, CCW positive
        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }

    private void SmoothPointerToward(float rawAngle)
    {
        if (!pointer) return;

        // Absolute delta from drag start for the visual (stable; no accumulation drift)
        float absFromStart = Mathf.DeltaAngle(_startAngle, rawAngle);

        // Exponential smoothing for visual stability (unscaled time for UI)
        float k = 1f - Mathf.Exp(-Mathf.Max(0.0001f, 1f / Mathf.Max(0.0001f, pointerSmoothingTime)) * Time.unscaledDeltaTime);
        _displayAngle = Mathf.LerpAngle(_displayAngle, rawAngle, k);

        float visAbsFromStart = Mathf.DeltaAngle(_startAngle, _displayAngle);
        pointer.localRotation = Quaternion.Euler(0f, 0f, _pointerStartZ + visAbsFromStart);
    }
}
