using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [SerializeField] private Vector3[] wayPoints;
    [SerializeField] private float speed;

    private int wayPointIndex;
    private float dist;
    private Vector3 dir;

    void Awake()
    {
        wayPointIndex = -1;
        IncreaseIndex();
    }

    // Update is called once per frame
    void Update()
    {
        dist = Vector3.Distance(transform.position, wayPoints[wayPointIndex]);
        if (dist < 0.1f)
        {
            IncreaseIndex();
        }
        Move();
    }

    void Move()
    {
        transform.Translate(dir * speed * Time.deltaTime);
    }

    void IncreaseIndex()
    {
        wayPointIndex = (wayPointIndex + 1) % wayPoints.Length;
        dir = (wayPoints[wayPointIndex] - transform.position).normalized;
    }
}
