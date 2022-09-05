using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField]
    Camera cam;

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = cam.transform.rotation;
    }
}