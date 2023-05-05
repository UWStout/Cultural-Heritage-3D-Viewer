using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDragAzimuth : MonoBehaviour
{
    public Transform AzimuthRotation;
    private Vector3 p, xz, result;
    private float x, y, z, teta;
    private RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Get the current mouse position
        p = Input.mousePosition;

        // Raycast from the camera to the mouse position and store the result
        if(Physics.Raycast(Camera.main.ScreenPointToRay(p), out hit))
        {
            //Get the x, y, and z coordinates of the hit point 
            x = hit.point.x;
            y = hit.point.y;
            z = hit.point.z;

            //Calculate the normalized vector from x and z coordinates
            xz = new Vector3(x, 0, z);
            Vector3 v = Vector3.Normalize(xz);

            //Calculate the angle (teta) using the arctg function
            teta = Mathf.Atan2(z, x);

            //Set the y rotation of the Azimuth Rotation transform to the teta angle
            AzimuthRotation.rotation = Quaternion.Euler(0, teta * Mathf.Rad2Deg, 0);

            result = new Vector3(x, y, z);

        }
        
    }
}
