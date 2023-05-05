using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField] //Get the mouse position
    private Vector3 screenPosition;

    [SerializeField]
    private GameObject target;


    [SerializeField]
    private Vector3 worldPosition;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            screenPosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            if (Physics.Raycast(ray, out RaycastHit hitData))
            {
                Arrow arrow = hitData.collider.gameObject.GetComponent<Arrow>();
                if (arrow != null)
                {
                    worldPosition = hitData.point;
                    //transform.position = worldPosition;
                    // target.transform.position = worldPosition + new Vector3(1, 1, 1);
                    Vector3 DeltaPosition = hitData.point - worldPosition;
                    arrow.Process(DeltaPosition);
                }

            }
        }
    }
}
