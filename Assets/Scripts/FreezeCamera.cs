using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeCamera : MonoBehaviour
{
    [SerializeField]
    CameraController con;

    public void OnClick()
    {
        con.rotateCamera = false;
        con.dragCamera = false;
    }
}
