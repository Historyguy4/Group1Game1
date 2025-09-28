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

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse Entered!");

        if (customCursor)
        {
            CursorManager.Instance?.Apply(customCursor, customHotspot);
            return;
        }

        switch (hoverCursor)
        {
            case HoverCursor.Pointer: CursorManager.Instance?.SetPointer(); break;
            case HoverCursor.Forbidden: CursorManager.Instance?.SetForbidden(); break;
            case HoverCursor.Drag: CursorManager.Instance?.SetDrag(); break;
            default: CursorManager.Instance?.SetDefault(); break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.Instance?.SetDefault();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CursorManager.Instance?.SetDrag();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Return to hover state if still over the element; otherwise default
        if (customCursor)
            CursorManager.Instance?.Apply(customCursor, customHotspot);
        else
            OnPointerEnter(eventData);
    }

    void OnDisable()
    {
        // Safety: if the object disables while hovered, restore default
        if (CursorManager.Instance) CursorManager.Instance.SetDefault();
    }
}
