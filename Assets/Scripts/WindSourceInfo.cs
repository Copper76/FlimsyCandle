using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSourceInfo : MonoBehaviour
{
    private Vector3 dir;

    void Awake()
    {
        dir = transform.rotation * new Vector3(1.0f, 0.0f, 0.0f);
    }

    public Vector3 GetDir() { return dir; }
}
