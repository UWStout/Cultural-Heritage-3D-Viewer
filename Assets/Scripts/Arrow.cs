using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Arrow : MonoBehaviour
{
    [SerializeField]
    private GameObject LightSource;

    public abstract void Process(Vector3 DeltaPosition);
}


