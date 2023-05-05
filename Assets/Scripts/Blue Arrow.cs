using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueArrow : Arrow
{
    public override void Process(Vector3 DeltaPosition)
    {
        Debug.Log("This is a blue arrow");
    }
}
