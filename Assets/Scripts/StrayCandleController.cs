using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrayCandleController : MonoBehaviour
{
    private StrayCandleInteracter candleInteracter;

    private Light candleLight;
    private SphereCollider lightCollider;
    private Transform scaler;
    private ParticleSystem ps;
    private CapsuleCollider candleCollider;

    private float minHeight;
    private float minIntensity;
    private float lifeTime;

    private float maxHeight;
    private float maxIntensity;
    private float maxRadius;
    private float burnSpeed;
    private float dimSpeed;
    private float burnRadiusSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        minHeight = 0.15f;
        minIntensity = 0.25f;
        lifeTime = 120.0f;
        maxRadius = 4.5f;

        scaler = transform.GetChild(0);
        candleLight = scaler.GetChild(2).GetChild(0).GetComponent<Light>();
        ps = scaler.GetChild(2).GetChild(1).GetComponent<ParticleSystem>();

        maxHeight = scaler.localScale.y;
        maxIntensity = candleLight.intensity;
        burnSpeed = (maxHeight - minHeight) / lifeTime;
        dimSpeed = (maxIntensity - minIntensity) / lifeTime;
        burnRadiusSpeed = maxRadius / lifeTime;

        candleInteracter = GetComponent<StrayCandleInteracter>();
        candleCollider = scaler.GetChild(0).GetComponent<CapsuleCollider>();
        lightCollider = transform.GetChild(1).GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Burn())
        {
            Die();
        }
    }

    private bool Burn()
    {
        if (scaler.localScale.y < minHeight)
        {
            return true;
        }
        scaler.localScale -= new Vector3(0, burnSpeed * Time.deltaTime, 0);
        lightCollider.radius -= burnRadiusSpeed * Time.deltaTime;
        if (scaler.localScale.y < 0.5)
        {
            candleCollider.center = new Vector3(0f, 0.6f / scaler.localScale.y, 0f);
        }
        candleLight.intensity -= dimSpeed * Time.deltaTime;
        return false;
    }

    public void AdjustFuel(float time)
    {
        scaler.localScale = new Vector3(1f, Mathf.Min(maxHeight, scaler.localScale.y + burnSpeed * time), 1f);
        lightCollider.radius = Mathf.Min(maxRadius, lightCollider.radius + burnRadiusSpeed * time);
        if (scaler.localScale.y < 0.5)
        {
            candleCollider.center = new Vector3(0f, 0.5f / scaler.localScale.y, 0f);
        }
        else
        {
            candleCollider.center = new Vector3(0f, 1.0f, 0f);
        }
        candleLight.intensity = Mathf.Min(maxIntensity, candleLight.intensity + dimSpeed * time);
    }

    private void Die()
    {
        candleLight.intensity = 0;
        ps.Stop();
        candleInteracter.SwapPrompt();
        Destroy(this);
    }
}
