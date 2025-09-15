using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{   
    [SerializeField] public Camera Camera;

    private Vector3 _origin;
    private Vector3 _difference;

    private Camera _camera;

    private bool _isDragging;

    private void Awake()
    {
        _camera = Camera;
    }

    public void OnDrag(InputAction.CallbackContext ctx)
    {
        if (ctx.started) _origin = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        _isDragging = ctx.started || ctx.performed;
    }

    private void LateUpdate()
    {
        if (!_isDragging) return;   

        _difference = GetMousePosition - transform.position;
        transform.position = _origin - _difference;

    }

    private Vector3 GetMousePosition => _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
}   