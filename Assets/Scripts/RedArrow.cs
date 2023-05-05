using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedArrow : MonoBehaviour // : Arrow
{
    //public override void Process(Vector3 DeltaPosition)
    //{
    //    Debug.Log("This is a red Arrow");
    //}
    private void OnMouseDown()
    {
        //MousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
        Debug.Log("on Mouse Down");
    }

    private void OnMouseDrag()
    {
        //transform.position = GetMouseWorldPosition() + MousePositionOffset;
        Debug.Log("on Mouse Drag");

    }

}
