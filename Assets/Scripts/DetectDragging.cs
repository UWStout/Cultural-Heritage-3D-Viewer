using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectDragging : MonoBehaviour
{
    Vector3 MousePositionOffset;

    private Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }
    private void OnMouseDown()
    {
        MousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
        Debug.Log("on Mouse Down");
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + MousePositionOffset;
        Debug.Log("on Mouse Drag");

    }
}
