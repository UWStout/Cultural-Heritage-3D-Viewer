using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AzimuthRotation : MonoBehaviour
{
    //Set the center of rotation for the drag motion
    [SerializeField]
    private Transform rotationCenter;

    //Variable to set the distance between the rotation center and the drag object; the object rotates at 2f units fron the rotation center
    [SerializeField]
    private float radious = 20f;

    // The current angle of rotation
    private float angle;

    //The starting position of the mouse drag
    private Vector3 dragOrigin;

    //The object to rotate
    [SerializeField]
    private GameObject dragObject;

    //Shiny Material
    [SerializeField] 
    private Material shinyMaterial;
    private Material originalMaterial;
    private bool isDragging = false;

    private void Start()
    {
        originalMaterial = this.GetComponent<Renderer>().material;
        // Calculate the initial angle of rotation based on the starting position of the drag object
        angle = Mathf.Atan2(dragObject.transform.position.z - rotationCenter.position.z, dragObject.transform.position.x - rotationCenter.position.x);
    }

    private void Update()
    {
        if (isDragging)
        {
            // Apply shiny material while dragging
            this.GetComponent<Renderer>().material = shinyMaterial;
        }
        else
        {
            // Restore original material when not dragging
            this.GetComponent<Renderer>().material = originalMaterial;
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        // Save the current mouse position as the origin of the drag
        dragOrigin = Input.mousePosition;
    }

    void OnMouseDrag()
    {
        //Desable camera when the object is being dragged
        Camera.main.GetComponent<CameraController>().DisableCamera();

        // Cast a ray from the mouse position to the plane to determine the position of the object to drag
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //Update the position of the object based on the hit point and the radius 
            Vector3 v = (hit.point - rotationCenter.position).normalized;
            angle = Mathf.Atan2(v.z, v.x);
            //dragObject.transform.position = rotationCenter.position + v * radious;
            dragObject.transform.rotation = Quaternion.AngleAxis(-angle * 180 / Mathf.PI, new Vector3(0, 1, 0));
        }
        //// Calculate the change in mouse position and update the angle of rotation accordingly
        //Vector3 currentMousePosition = Input.mousePosition;
        //float deltaAngle = (currentMousePosition - dragOrigin).x * 0.01f;
        //angle += deltaAngle;

        //// Update the position of the object based on the new angle of rotation
        //// float x = rotationCenter.position.x + radious * Mathf.Cos(angle);
        //// float z = rotationCenter.position.z + radious * Mathf.Sin(angle);
        //// dragObject.transform.position = new Vector3(x, dragObject.transform.position.y, z);
        //dragObject.transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 1, 0));

        //// Update the drag origin to the current mouse position
        //dragOrigin = currentMousePosition;
    }

    private void OnMouseUp()
    {
        Camera.main.GetComponent<CameraController>().EnableCamera();
        isDragging = false;
    }
}
