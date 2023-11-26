using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameManager manager;

    private float optimalDist;
    private float targetDist;
    private Transform cameraObject;
    private int mapLayer;
    private float camZoomSpeed;
    private Vector3 dir;

    public Vector3 initPos;

    private bool ended;
    private float endDist;

    void Awake()
    {
        cameraObject = transform.GetChild(0);
        optimalDist = 25f;
        targetDist = optimalDist;
        mapLayer = 1 << 3;
        camZoomSpeed = 5f;

        initPos = new Vector3(0f,0.5f,0f);

        ended = false;
        endDist = 250f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.parent != null && !ended)
        {
            RaycastHit hit;
            dir = (cameraObject.position - transform.parent.position).normalized;
            if (Physics.Raycast(transform.parent.position, dir, out hit, Mathf.Infinity,mapLayer))
            {
                targetDist = Mathf.Min(hit.distance*hit.distance,optimalDist);
            }
            else
            {
                targetDist = optimalDist;
            }

            if (targetDist > cameraObject.localPosition.sqrMagnitude)
            {
                cameraObject.position += dir * camZoomSpeed * Time.deltaTime;
            }
            if (targetDist < cameraObject.localPosition.sqrMagnitude)
            {
                cameraObject.position -= dir * camZoomSpeed * Time.deltaTime;
            }
        }

        if (ended)
        {
            if (endDist > cameraObject.localPosition.sqrMagnitude)
            {
                cameraObject.localPosition += new Vector3(0,0,-1) * camZoomSpeed * Time.deltaTime;
            }
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.y += 15f * Time.deltaTime;
            if (rotation.x * rotation.x - 900f >= 0.1f)
            {
                rotation.x -= 10f * Time.deltaTime;
            }
            if (rotation.x * rotation.x - 900f <= 0.1f)
            {
                rotation.x += 10f * Time.deltaTime;
            }
            transform.rotation = Quaternion.Euler(rotation);
            /**
            if (transform.rotation.x * transform.rotation.x - 900f >= 0.1f)
            {
                transform.rotation *= Quaternion.Euler(new Vector3((30f-transform.rotation.x)*Time.deltaTime, 0f, 0f));
            }
            **/
        }
    }

    public void ResetCam()
    {
        transform.parent = null;
        transform.position = initPos;
        transform.rotation = Quaternion.Euler(new Vector3(30,0,0));
        cameraObject.localPosition = new Vector3(0,0,-Mathf.Sqrt(optimalDist));
    }

    public void Attach(Transform parent)
    {
        transform.parent = parent;
        transform.position = parent.position;
    }

    public void Detach()
    {
        transform.parent = null;
    }

    public void VictoryRotate()
    {
        ended = true;
        manager.KillLights();
    }
}
