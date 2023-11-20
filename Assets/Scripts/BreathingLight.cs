using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class BreathingLight : MonoBehaviour
{
    private float timer;
    private Light myLight;
    private Vector3 initialPos;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        myLight = GetComponent<Light>();
        initialPos = transform.position;
    }

    float oscillate(float timer, float speed, float scale)
    {
        return Mathf.Cos(timer * speed / Mathf.PI) * scale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;
        myLight.intensity = 5.0f + oscillate(timer, 5f, 1f);
        transform.position = initialPos + new Vector3(0,oscillate(timer, 5, 0.5f),0);
    }
}
