using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlledPanelPatrol : Controllable
{
    [SerializeField] private Vector3[] wayPoints;//0 is idle and 1 is target
    [SerializeField] private float speed;

    private int wayPointIndex;
    private float dist;
    private Vector3 dir;

    void Awake()
    {
        SetTarget(0);
    }

    // Update is called once per frame
    void Update()
    {
        dist = Vector3.Distance(transform.position, wayPoints[wayPointIndex]);
        if (dist > 0.01f)
        {
            Move();
        }
    }

    private void Move()
    {
        transform.Translate(dir * speed * Time.deltaTime);
    }

    private void SetTarget(int target)
    {
        wayPointIndex = target;
        dir = (wayPoints[target] - transform.position).normalized;
    }

    public override void Control()
    {
        SetTarget(1);
    }

    public override void Release()
    {
        SetTarget(0);
    }
}
