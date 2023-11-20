using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampController : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private int lampNum;
    [SerializeField] private GameManager manager;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            manager.UpdateSpawnPoint(lampNum, spawnPos);
        }
    }
}
