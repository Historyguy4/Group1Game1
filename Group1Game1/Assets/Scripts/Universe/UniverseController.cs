using System;
using TMPro;
using UnityEngine;

public class UniverseController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Min(0f), SerializeField] float moveSpeed = 5f;
    [Min(0f), SerializeField] float moveScaleSpeed = 100f;
    [Min(0f), SerializeField] float zoomSpeed = 5f;

    [Min(0f), SerializeField] float moveSpeedClock = 5f;
    [Min(0f), SerializeField] float zoomSpeedClock = 5f;

    [Header("Constraints")]
    [Min(0f), SerializeField] float minZoom = 1f;
    public static float MinZoom;
    [Min(0f), SerializeField] float maxZoom = 5f;
    public static float MaxZoom;
    [Min(0f), SerializeField] float maxX = 8f;
    public static float MaxX;
    [Min(0f), SerializeField] float maxY = 5f;
    public static float MaxY;

    [Header("Clock Settings")]
    [SerializeField, Min(0f)] float dialMoveDamp = 8f;  // higher = quicker decay
    [SerializeField, Min(0f)] float dialZoomDamp = 8f;  // higher = quicker decay

    // How strong dial input can get per frame
    [SerializeField] float dialMoveClamp = 1.5f;
    [SerializeField] float dialZoomClamp = 10f;

    [Header("Zoom Bounce")]
    [SerializeField] bool bounceZoomAtBounds = true;  // toggle behavior
    [SerializeField, Min(0f)] float sineEase = 1f;    // 0 = hard triangle; 1 = full sine-like
    float _unboundedZoom;                             // accumulates without clamping
    
    [Header("References")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] Camera cameraComponent;
    [SerializeField] TMP_Text cameraText;

    [SerializeField] Vector3 universePosition = Vector3.one;
    //Use this to see if camera is close to story point
    public Vector3 UniversePosition => universePosition;

    Vector2 moveInput;
    float zoomInput;

    Vector2 dialMove;           // accumulated from dials this frame
    float dialZoom;

    float currentZoom;
    float currentZoom01 => Mathf.InverseLerp(minZoom, maxZoom, currentZoom);
    float currentZoom10 => 1f - currentZoom01; // accumulated from dials this frame

    private void Awake()
    {
        _unboundedZoom = cameraComponent ? cameraComponent.orthographicSize : minZoom;
        currentZoom = _unboundedZoom;

        MinZoom = minZoom;
        MaxZoom = maxZoom;
        MaxX = maxX;
        MaxY = maxY;
    }

    private void Update()
    {
        var keyMove = ReadKeyInputMove();
        var keyZoom = ReadZoomInputMove();

        moveInput = keyMove + dialMove;
        zoomInput = keyZoom + dialZoom;

        if (!canMove) return;

        ApplyMovement(moveInput, zoomInput);

        float dampPos = Mathf.Exp(-dialMoveDamp * Time.deltaTime);
        float dampZoom = Mathf.Exp(-dialZoomDamp * Time.deltaTime);
        dialMove *= dampPos;
        dialZoom *= dampZoom;

        // existing position → universePosition UI
        universePosition = new Vector3(cameraTransform.position.x, cameraTransform.position.y, cameraComponent.orthographicSize);
        cameraText.text = $"Day: {universePosition.x:0.0} Month: {universePosition.y:0.0} Year: {universePosition.z:0.0}";
    }
    public void OnDialMove(float dx, float dy)
    {
        dialMove.x = Mathf.Clamp(dialMove.x + dx, -dialMoveClamp, dialMoveClamp) * moveSpeedClock;
        dialMove.y = Mathf.Clamp(dialMove.y + dy, -dialMoveClamp, dialMoveClamp) * moveSpeedClock;
    }

    public void OnDialZoom(float dz)
    {
        dialZoom = Mathf.Clamp(dialZoom + dz, -dialZoomClamp, dialZoomClamp) * zoomSpeedClock;
    }

    //This is where the zoom and movement is applied to the camera
    private void ApplyMovement(Vector2 input, float zoom)
    {
        //Zoom----
        if (bounceZoomAtBounds)
        {
            // accumulate the raw/unbounded zoom first
            _unboundedZoom += zoom * Time.deltaTime;

            float range = Mathf.Max(0.0001f, maxZoom - minZoom);

            // Triangle-wave mapping into [min,max]:  min -> max -> min -> ...
            float tri = Mathf.PingPong(_unboundedZoom - minZoom, range) / range; // 0..1..0

            // sineEase in [0,1]: 0 = pure triangle; 1 = fully cosine-smoothed.
            float eased = (sineEase <= 0f)
                ? tri
                : Mathf.Lerp(tri, 0.5f - 0.5f * Mathf.Cos(Mathf.PI * tri), Mathf.Clamp01(sineEase));

            currentZoom = minZoom + eased * range;
            cameraComponent.orthographicSize = currentZoom;
        }
        else
        {
            // Original clamp behavior
            float newZoom = cameraComponent.orthographicSize + zoom * Time.deltaTime;
            currentZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
            _unboundedZoom = currentZoom; // keep in sync when not bouncing
            cameraComponent.orthographicSize = currentZoom;
        }

        //Movement----
        float moveScale = Mathf.Lerp(0.05f, 1.0f, currentZoom01) * moveScaleSpeed;

        //Calculate movement step based on input, speed, deltaTime, and zoom scale
        float moveStepX = input.x * moveSpeed * Time.deltaTime * moveScale;
        float moveStepY = input.y * moveSpeed * Time.deltaTime * moveScale;

        //This is used to clamp the cmaera into the screen and keep in universe bounds
        float newX = Mathf.Clamp(cameraTransform.position.x + moveStepX, -maxX * currentZoom10, maxX * currentZoom10);
        float newY = Mathf.Clamp(cameraTransform.position.y + moveStepY, -maxY * currentZoom10, maxY * currentZoom10);

        float z = cameraTransform.position.z;
        cameraTransform.position = new Vector3(newX, newY, z);
    }

    //This is a placeholder for future input systems (like the mouse drag on UI clock)
    //For now I am using the arrow keys or WASD to move the camera up/down/left/right
    private Vector2 ReadKeyInputMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        //This is important to keep diagonal movement from being faster than horizontal or vertical movement
        return (dir.sqrMagnitude > 1f ? dir.normalized : dir);
    }

    //For now I am using Q and E to zoom in and out
    private float ReadZoomInputMove()
    {
        float zoom = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            zoom = 1f;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            zoom = -1f;
        }

        return zoom * zoomSpeed;
    }

    bool canMove = true;
    internal void StopMovement()
    {
        canMove = false;
    }
    internal void StartMovement()
    {
        canMove = true;
    }
}
