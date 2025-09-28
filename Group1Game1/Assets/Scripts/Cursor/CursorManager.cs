using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Default")]
    public Texture2D defaultCursor;
    public Vector2 defaultHotspot = new Vector2(4, 4);

    [Header("Common UI States")]
    public Texture2D pointerCursor;
    public Vector2 pointerHotspot = new Vector2(8, 2);

    public Texture2D dragCursor;
    public Vector2 dragHotspot = new Vector2(16, 16);

    public Texture2D forbiddenCursor;
    public Vector2 forbiddenHotspot = new Vector2(16, 16);

    public CursorMode cursorMode = CursorMode.Auto;

    // --- Lock state ---
    MonoBehaviour _lockOwner;  // who currently “owns” the cursor (e.g., a dragging UI)
    bool Locked => _lockOwner != null;
    bool CanChange(MonoBehaviour caller) => !Locked || ReferenceEquals(_lockOwner, caller);

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() => SetDefault();

    // Public API with optional caller
    public void SetDefault(MonoBehaviour caller = null) { if (!CanChange(caller)) return; Apply(defaultCursor, defaultHotspot); }
    public void SetPointer(MonoBehaviour caller = null) { if (!CanChange(caller)) return; Apply(pointerCursor, pointerHotspot); }
    public void SetDrag(MonoBehaviour caller = null) { if (!CanChange(caller)) return; Apply(dragCursor, dragHotspot); }
    public void SetForbidden(MonoBehaviour caller = null) { if (!CanChange(caller)) return; Apply(forbiddenCursor, forbiddenHotspot); }

    public void Apply(Texture2D tex, Vector2 hotspot, MonoBehaviour caller = null)
    {
        if (!CanChange(caller)) return;
        if (!tex) { Cursor.SetCursor(null, Vector2.zero, cursorMode); return; }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.SetCursor(tex, hotspot, cursorMode);
    }

    // Drag lock helpers
    public void SetDragLock(MonoBehaviour owner)
    {
        _lockOwner = owner;
        Apply(dragCursor, dragHotspot, owner);
    }

    public void ReleaseLock(MonoBehaviour owner)
    {
        if (!ReferenceEquals(_lockOwner, owner)) return;
        _lockOwner = null;
        SetDefault(); // or keep last hover; up to you
    }
}
