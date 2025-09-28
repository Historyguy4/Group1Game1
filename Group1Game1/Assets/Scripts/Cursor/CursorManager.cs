using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Default")]
    [Tooltip("Shown whenever no special cursor is active.")]
    public Texture2D defaultCursor;
    public Vector2 defaultHotspot = new Vector2(4, 4);

    [Header("Common UI States")]
    public Texture2D pointerCursor;   // e.g., hand for buttons/links
    public Vector2 pointerHotspot = new Vector2(8, 2);

    public Texture2D dragCursor;      // while dragging
    public Vector2 dragHotspot = new Vector2(16, 16);

    public Texture2D forbiddenCursor; // disabled/interact-blocked
    public Vector2 forbiddenHotspot = new Vector2(16, 16);

    [Tooltip("Auto uses hardware cursor where supported; ForceSoftware allows animation or unusual sizes.")]
    public CursorMode cursorMode = CursorMode.Auto;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // Optional: DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SetDefault(); // apply default at boot
    }

    public void SetDefault()
    {
        Apply(defaultCursor, defaultHotspot);
    }

    public void SetPointer() { Apply(pointerCursor, pointerHotspot); }
    public void SetDrag() { Apply(dragCursor, dragHotspot); }
    public void SetForbidden() { Apply(forbiddenCursor, forbiddenHotspot); }

    public void Apply(Texture2D tex, Vector2 hotspot)
    {
        if (!tex) { Cursor.SetCursor(null, Vector2.zero, cursorMode); return; }
        // Unity expects hotspot in pixels from top-left
        Cursor.SetCursor(tex, hotspot, cursorMode);
    }
}
