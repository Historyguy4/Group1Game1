using UnityEngine;
using UnityEngine.EventSystems;

public class UICursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler
{
    public enum HoverCursor { Pointer, Forbidden, Drag, Default }

    [Header("Cursor on hover")]
    public HoverCursor hoverCursor = HoverCursor.Pointer;

    [Header("Override (Optional)")]
    public Texture2D customCursor;
    public Vector2 customHotspot;

    bool _isDragging;
    RectTransform _rt;

    void Awake() => _rt = (RectTransform)transform;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isDragging) return; // drag owns the cursor
        if (customCursor) { CursorManager.Instance?.Apply(customCursor, customHotspot, this); return; }

        switch (hoverCursor)
        {
            case HoverCursor.Pointer: CursorManager.Instance?.SetPointer(this); break;
            case HoverCursor.Forbidden: CursorManager.Instance?.SetForbidden(this); break;
            case HoverCursor.Drag: CursorManager.Instance?.SetDrag(this); break;
            default: CursorManager.Instance?.SetDefault(this); break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isDragging) return; // don't change while dragging
        CursorManager.Instance?.SetDefault(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;
        // Lock the cursor to drag so other UI hovers can't override it
        CursorManager.Instance?.SetDragLock(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;

        // Release lock first
        CursorManager.Instance?.ReleaseLock(this);

        // If pointer is still over THIS element, restore its hover cursor; else go default
        bool overThis = RectTransformUtility.RectangleContainsScreenPoint(
            _rt, eventData.position, eventData.pressEventCamera);

        if (overThis)
        {
            OnPointerEnter(eventData); // reuse hover logic
        }
        else
        {
            CursorManager.Instance?.SetDefault(this);
        }
    }

    void OnDisable()
    {
        // Safety: if this disables mid-drag, release any lock and reset
        if (_isDragging) { CursorManager.Instance?.ReleaseLock(this); _isDragging = false; }
        CursorManager.Instance?.SetDefault(this);
    }
}
