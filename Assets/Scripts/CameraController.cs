using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class for camera controls in the viewer.
 * Holding right click toggles camera panning across the x and y axes.
 * Holding left click toggles camera rotation (around a point at which the camera is looking).
 * Holding both only allows the latest held button to register
 */
public class CameraController : MonoBehaviour
{
    public GameObject cameraTarget;
    private float zoomModifier = 2.0f;
    private float dragModifer = 0.01f;
    [SerializeField]
    float rotationModifer = 0.1f;
    private float minAngle = -88f;
    private float maxAngle = 88f;
    private float maxZoomDistance = 20.0f;
    private float minZoomDistance = 2.0f;
    public bool dragCamera;
    public bool rotateCamera;
    private Vector2 lastMousePosition;
    private Vector3 startPosition;
    private Quaternion startRotation;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;

        dragCamera = false;
        rotateCamera = false;
        lastMousePosition = Input.mousePosition;
        // Ensure rotation point is defined
        if (!cameraTarget)
        {
            cameraTarget = GameObject.Find("RotationPoint");
        }
        transform.LookAt(cameraTarget.transform);
        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

        cameraTarget.transform.rotation = transform.rotation;
        // Input checks

        // Check for scroll wheel motion
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f)
        {
            changeZoomLevel(zoomDelta);
        }

        // Enable the ability to rotate the camera around when left click is held
        if (Input.GetMouseButtonDown(0))
        {
            // Controls only accept the last pressed mouse button
            dragCamera = false;
            rotateCamera = true;
        }

        // Disable the rotate camera variable when left click is released
        if (Input.GetMouseButtonUp(0))
        {
            rotateCamera = false;
        }

        // Enable the ability to drag the camera around when right click is held
        if (Input.GetMouseButtonDown(1))
        {
            // Controls only accept the last pressed mouse button
            rotateCamera = false;
            dragCamera = true;
        }
        
        // Disable the drag camera variable when right click is released
        if (Input.GetMouseButtonUp(1))
        {
            dragCamera = false;
        }

        // End input checks

        // Mouse drag behavior
        if (dragCamera)
        {
            // Snapshot and convert mouse position into a vector2
            Vector2 currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 deltaMouse = currentMousePosition - lastMousePosition;
            transform.Translate(new Vector3(-deltaMouse.x, -deltaMouse.y, 0f) * dragModifer, Space.Self);
            // Update the transform of the camera target to align with the camera
            cameraTarget.transform.Translate(new Vector3(-deltaMouse.x, -deltaMouse.y, 0f) * dragModifer, Space.Self);
        }
        else if (rotateCamera)
        {
            Vector2 currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 deltaMouse = currentMousePosition - lastMousePosition;
            transform.RotateAround(cameraTarget.transform.position, cameraTarget.transform.up, deltaMouse.x * rotationModifer);
            if (Vector3.Angle(transform.forward, Vector3.up) < 6f)
            {
                if (-deltaMouse.y > 0)
                {
                    transform.RotateAround(cameraTarget.transform.position, cameraTarget.transform.right, -deltaMouse.y * rotationModifer);
                }
            }
            else if (Vector3.Angle(transform.forward, Vector3.up) > 174f)
            {
                if (-deltaMouse.y < 0)
                {
                    transform.RotateAround(cameraTarget.transform.position, cameraTarget.transform.right, -deltaMouse.y * rotationModifer);
                }
            }
            // Normal case
            else
            {
                transform.RotateAround(cameraTarget.transform.position, cameraTarget.transform.right, -deltaMouse.y * rotationModifer);

            }
            transform.LookAt(cameraTarget.transform);
        }
        // At the end of each frame, update the last mouse position
        lastMousePosition = Input.mousePosition;
    }

    // Moves the camera forward and backward depending on scroll wheel delta
    void changeZoomLevel(float deltaZoom)
    {
        float currentDist = Vector3.Distance(transform.position, cameraTarget.transform.position);
        if (deltaZoom > 0)
        {
            if (currentDist > minZoomDistance)
            {
                transform.position += transform.forward * deltaZoom * zoomModifier;
            }
        }
        else
        {
            if (currentDist < maxZoomDistance)
            {
                transform.position += transform.forward * deltaZoom * zoomModifier;
            }
        } 
    }

    void resetCameraPosition()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}
