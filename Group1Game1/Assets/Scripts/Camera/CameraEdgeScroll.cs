using UnityEngine;
using UnityEngine.EventSystems;

public class CameraEdgeScroll : MonoBehaviour
{
    [Header("Target")]
    public Transform cameraTarget; // usually your Camera transform

    [Header("Edge Scroll")]
    public float edgeSize = 20f;      // pixels from each screen border
    public float edgePanSpeed = 15f;  // world units/sec

    [Header("UI Blocking")]
    public bool blockWhenPointerOverUI = true;

    void Reset()
    {
        if (cameraTarget == null && Camera.main != null)
            cameraTarget = Camera.main.transform;
    }

    void Update()
    {
        if (cameraTarget == null) return;

        if (blockWhenPointerOverUI && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 move = Vector3.zero;
        Vector3 mouse = Input.mousePosition;

        // Left/right
        if (mouse.x <= edgeSize) move.x -= 1f;
        else if (mouse.x >= Screen.width - edgeSize) move.x += 1f;

        // Bottom/top → in 2D we use Y axis
        if (mouse.y <= edgeSize) move.y -= 1f;
        else if (mouse.y >= Screen.height - edgeSize) move.y += 1f;

        if (move.sqrMagnitude > 0f)
        {
            move.Normalize();
            cameraTarget.position += move * edgePanSpeed * Time.deltaTime;
        }
    }
}
