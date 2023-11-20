using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlledSHieldDropControl : Controllable
{
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private Vector3 spawnPosition;
    private GameObject shield;

    public override void Control()
    {
        if (shield != null)
        {
            shield.transform.position = spawnPosition;
            shield.transform.rotation = Quaternion.identity;
        }
        else
        {
            shield = Instantiate(shieldPrefab, transform.parent);
            shield.transform.position = spawnPosition;
        }
    }

    public override void Release()
    {
    }
}
