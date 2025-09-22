using System;
using TMPro;
using UnityEngine;

public class UniverseController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Min(0f), SerializeField] float moveSpeed = 5f;
    [Min(0f), SerializeField] float moveScaleSpeed = 100f;
    [Min(0f), SerializeField] float zoomSpeed = 5f;

    [Header("Constraints")]
    [Min(0f), SerializeField] float minZoom = 1f;
    public static float MinZoom;
    [Min(0f), SerializeField] float maxZoom = 5f;
    public static float MaxZoom;
    [Min(0f), SerializeField] float maxX = 8f;
    public static float MaxX;
    [Min(0f), SerializeField] float maxY = 5f;
    public static float MaxY;

    [Header("References")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] Camera cameraComponent;
    [SerializeField] TMP_Text cameraText;

    [SerializeField] Vector3 universePosition = Vector3.one;
    //Use this to see if camera is close to story point
    public Vector3 UniversePosition => universePosition;

    Vector2 moveInput;
    float zoomInput;

    private void Awake()
    {
        MinZoom = minZoom;
        MaxZoom = maxZoom;
        MaxX = maxX;
        MaxY = maxY;
    }

    private void Update()
    {
        //Read input from keyboard or other input systems (clamped -1/1 for X, Y, and zoom)
        moveInput = ReadKeyInputMove();
        zoomInput = ReadZoomInputMove();

        if (!canMove)
            return;

        //Use this function here to apply movement and zoom to the camera
        ApplyMovement(moveInput, zoomInput);

        //Update the universe position based on the camera position
        //Use this to see if camera is close to story point
        universePosition = new Vector3(cameraTransform.position.x, cameraTransform.position.y, cameraComponent.orthographicSize);
        //cameraText.text = $"Day: {LocationInSpace.GetDayString(universePosition.x)} " +
        //    $"Month: {LocationInSpace.GetMonthString(universePosition.y)} " +
        //    $"Year: {universePosition.z:0.0}";
        cameraText.text = $"Day: {universePosition.x:0.0} " +
            $"Month: {universePosition.y:0.0} " +
            $"Year: {universePosition.z:0.0}";
    }

    float currentZoom;
    float currentZoom01 => Mathf.InverseLerp(minZoom, maxZoom, currentZoom);
    float currentZoom10 => 1f - currentZoom01;

    //This is where the zoom and movement is applied to the camera
    private void ApplyMovement(Vector2 input, float zoom)
    {
        //Zoom----
        float newZoom = cameraComponent.orthographicSize + zoom * Time.deltaTime;
        currentZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
        cameraComponent.orthographicSize = currentZoom;

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
