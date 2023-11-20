using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePadController : MonoBehaviour
{
    [SerializeField] Controllable controlledObj;

    void OnTriggerEnter(Collider other)
    {
        controlledObj.Control();
        transform.GetChild(0).localScale = new Vector3(1f, 0.5f, 1f);
    }

    void OnTriggerExit(Collider other)
    {
        controlledObj.Release();
        transform.GetChild(0).localScale = new Vector3(1f, 1f, 1f);
    }
}
