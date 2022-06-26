using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager : MonoBehaviour
{
    [SerializeField]
    int index;
    [SerializeField]
    DropDown drop;
    
    public void OnClick()
    {
        drop.HandleInputData(index);
    }
}
