using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public ClipDepot clipDepot;
    public AudioSource burnSource;
    public AudioSource oneShotPlayer;
    public CameraController cam;
    public TextMeshProUGUI promptText;

    private GameObject canvas;
    //private CinemachineVirtualCamera vcam;
    private InteractController interactable;

    private Rigidbody rb;
    private Transform scaler;
    private CapsuleCollider[] candleColliders;
    private Light candleLight;
    private ParticleSystem ps;

    private float horizontalSens = 5.0f;
    private float verticalSens = 5.0f;
    private float verticalRotationMinimum = 0f;
    private float verticalRotationMaximum = 60f;
    private float jumpForce;
    private float moveSpeed;

    private float maxHeight;
    private float minHeight;
    private float maxIntensity;
    private float minIntensity;
    private float maxMass;
    private float minMass;
    private float gameTime;
    private float burnSpeed;
    private float dimSpeed;
    private float massLossSpeed;
    private Stack<float> burstThreshold;
    private float limitSpeed;

    private bool isGrounded;
    private float groundCheckDist;
    private float bufferCheckDist;

    private RaycastHit slopeHit;

    private bool inWind;
    private float windDimTotal;
    private Vector3 windDir;

    private Vector2 moveVector;
    private float height;

    private bool inLight;
    private float prevIntensity;

    // Start is called before the first frame update
    void Start()
    {
        limitSpeed = 5.0f;
        jumpForce = 240.0f;
        moveSpeed = 15.0f;
        minHeight = 0.15f;
        minIntensity = 0.25f;
        gameTime = 30.0f;

        inWind = false;
        windDimTotal = 0;

        inLight = false;

        isGrounded = true;
        bufferCheckDist = 0.1f;
        groundCheckDist = 1 + bufferCheckDist;

        rb = GetComponent<Rigidbody>();
        scaler = transform.GetChild(0);
        candleColliders = scaler.GetChild(0).GetComponents<CapsuleCollider>();
        candleLight = scaler.GetChild(2).GetChild(0).GetComponent<Light>();
        maxHeight = scaler.localScale.y;
        maxMass = rb.mass;
        minMass = 0.7f;
        maxIntensity = candleLight.intensity;
        burnSpeed = (maxHeight - minHeight) / gameTime;
        dimSpeed = (maxIntensity - minIntensity) / gameTime;
        massLossSpeed = (maxMass - minMass) / gameTime;
        burstThreshold = new Stack<float>();
        float section = (maxIntensity - minIntensity) / 5f;
        for (int i = 1; i < 5; i++)
        {
            burstThreshold.Push(minIntensity+section*i);
        }

        ps = scaler.GetChild(2).GetChild(1).GetComponent<ParticleSystem>();
        canvas = GameObject.Find("Canvas");
        promptText = canvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        burnSource.Play();
        //audioSource = GetComponent<AudioSource>();
        //vcam = GameObject.Find("CMVCam").GetComponent<CinemachineVirtualCamera>();
        cam.Attach(transform);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position,-transform.up, out hit, groundCheckDist);
        Vector3 moveDir = new Vector3(moveVector.x * moveSpeed, 0.0f, moveVector.y * moveSpeed);
        moveDir = transform.TransformDirection(moveDir);
        if (OnSlope(transform.localScale.y * 0.5f))
        {
            rb.AddForce(GetSlopeDirection(moveDir) * rb.mass);
        }
        else
        {
            rb.AddForce(moveDir * rb.mass);
        }
        if (Math.Abs(rb.velocity.x) > limitSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x < 0 ? -limitSpeed : limitSpeed, rb.velocity.y, rb.velocity.z);
        }
        if (Math.Abs(rb.velocity.z) > limitSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z < 0 ? -limitSpeed : limitSpeed);
        }

        if (transform.position.y<-1)
        {
            Fall();
        }
        if (!inLight)
        {
            if (Burn())
            {
                Die();
            }

            if (inWind)
            {
                if (windBurn())
                {
                    Die();
                }
            }
        }
    }

    private bool OnSlope(float checkDist)
    {
        if (Physics.Raycast(transform.position, -transform.up, out slopeHit, checkDist + 1f)){
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeDirection(Vector3 moveDir)
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal);
    }

    private bool Burn()
    {
        if (scaler.localScale.y < minHeight)
        {
            return true;
        }
        var fo = ps.forceOverLifetime;
        Vector3 blowDir = rb.velocity;
        fo.x = -blowDir.x * 5f;
        fo.z = -blowDir.z * 5f;
        if (rb.velocity != Vector3.zero)
        {
            scaler.localScale -= new Vector3(0, burnSpeed * Time.deltaTime, 0);
            if (scaler.localScale.y < 0.5)
            {
                candleColliders[0].center = new Vector3(0f, 0.65f / scaler.localScale.y, 0f);
                candleColliders[1].center = new Vector3(0f, 0.5f / scaler.localScale.y, 0f);
            }
            rb.mass -= massLossSpeed * Time.deltaTime;
            candleLight.intensity -= dimSpeed * Time.deltaTime;
            //ps.transform.rotation = Quaternion.Euler(new Vector3(-blowDir.z, 0, blowDir.x) * 30f);
        }
        else
        {
            scaler.localScale -= new Vector3(0, 0.5f * burnSpeed * Time.deltaTime, 0);
            if (scaler.localScale.y < 0.5)
            {
                candleColliders[0].center = new Vector3(0f, 0.65f / scaler.localScale.y, 0f);
                candleColliders[1].center = new Vector3(0f, 0.5f / scaler.localScale.y, 0f);
            }
            rb.mass -= 0.5f * massLossSpeed * Time.deltaTime;
            candleLight.intensity -= 0.5f * dimSpeed * Time.deltaTime;
            //ps.transform.rotation = Quaternion.identity;
        }
        if (burstThreshold.Count != 0)
        {
            if (candleLight.intensity < burstThreshold.Peek())
            {
                ps.Emit(30);
                burstThreshold.Pop();
            }
        }
        return false;
    }

    private bool windBurn()
    {
        RaycastHit hit;
        var fo = ps.forceOverLifetime;
        if (Physics.Raycast(transform.position,windDir, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag == "WindSource")
            {
                Vector3 blowDir = rb.velocity;
                //ps.transform.rotation = Quaternion.Euler(new Vector3(-windDir.z * 45f - blowDir.z * 30f, 0, windDir.x * 45f + blowDir.x * 30f));
                fo.x = -windDir.x * 50f - blowDir.x * 5f;
                fo.z = -windDir.z * 50f - blowDir.z * 5f;
                float dimAmount = 4f * dimSpeed * Time.deltaTime;
                candleLight.intensity -= dimAmount;
                windDimTotal += dimAmount;
                return candleLight.intensity <= minIntensity && !inLight;
            }
        }
        //ps.transform.rotation = Quaternion.identity;
        candleLight.intensity += windDimTotal;
        windDimTotal = 0;
        return false;
    }

    private void Die()
    {
        cam.ResetCam();
        burnSource.Stop();
        int clip = UnityEngine.Random.Range(0, clipDepot.deathClips.Length);
        oneShotPlayer.PlayOneShot(clipDepot.deathClips[clip]);
        candleLight.intensity = 0;
        ps.Stop();
        canvas.transform.GetChild(0).gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        promptText.gameObject.SetActive(false);
        GetComponent<PostMortem>().enabled = true;
        Destroy(this);
    }

    private void Fall()
    {
        cam.ResetCam();
        burnSource.Stop();
        int clip = UnityEngine.Random.Range(0, clipDepot.fallClips.Length);
        oneShotPlayer.PlayOneShot(clipDepot.fallClips[clip]);
        canvas.transform.GetChild(0).gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        promptText.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fuel")
        {
            AdjustFuel(10.0f);
            inLight = false;
            ps.transform.rotation = Quaternion.identity;
            ps.Play();
            burnSource.Play();
            candleLight.intensity = prevIntensity;
            other.gameObject.SetActive(false);
        }

        if (other.tag == "Victory")
        {
            int clip = UnityEngine.Random.Range(0, clipDepot.victoryClips.Length);
            oneShotPlayer.PlayOneShot(clipDepot.victoryClips[clip]);
            candleLight.range = 1000f;
            GetComponent<PostMortem>().enabled = true;
            cam.VictoryRotate();
            canvas.transform.GetChild(2).gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Destroy(this);
        }

        if (other.tag == "Wind")
        {
            inWind = true;
            windDimTotal = 0;
            windDir = other.GetComponent<WindSourceInfo>().GetDir();
        }

        if (other.tag == "Narration")
        {
            //AudioClip line = other.GetComponent<Narration>().narration;
            oneShotPlayer.PlayOneShot(other.GetComponent<Narration>().narration, 1.5f);
            Destroy(other.gameObject);
            /**
            if (line != null)
            {
                oneShotPlayer.PlayOneShot(line);
                line = null;
            }
            **/
        }

        if (other.tag == "Interactable")
        {
            interactable = other.GetComponent<InteractController>();
            if (interactable.promptAudio != null)
            {
                oneShotPlayer.PlayOneShot(interactable.promptAudio);
                interactable.promptAudio = null;
            }
            if (interactable.promptText != "")
            {
                promptText.gameObject.SetActive(true);
                promptText.text = interactable.promptText;
            }
        }

        if (other.tag == "Light")
        {
            if (!inLight)
            {
                inLight = true;
                prevIntensity = candleLight.intensity;
                candleLight.intensity = 0;
                ps.Stop();
                burnSource.Stop();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Interactable")
        {
            promptText.gameObject.SetActive(false);
        }

        if (other.tag == "Wind")
        {
            inWind = false;
            if (inLight)
            {
                prevIntensity += windDimTotal;
            }
            else
            {
                candleLight.intensity += windDimTotal;
            }
            windDimTotal = 0;
        }

        if (other.tag == "Light")
        {
            LeaveLight();
        }
    }

    public void RotateX(InputAction.CallbackContext context)
    {
        float rotationY = horizontalSens * context.ReadValue<float>() * Time.deltaTime;
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.y += rotationY;
        transform.rotation = Quaternion.Euler(rotation);
    }
    
    //normal camera
    public void RotateY(InputAction.CallbackContext context)
    {
        float rotationX = verticalSens * context.ReadValue<float>() * Time.deltaTime;
        Vector3 cameraRotation = Camera.main.transform.parent.rotation.eulerAngles;
        cameraRotation.x += rotationX;
        if ((cameraRotation.x > verticalRotationMinimum && rotationX<0) || (cameraRotation.x < verticalRotationMaximum && rotationX>0))
        {
            Camera.main.transform.parent.rotation = Quaternion.Euler(cameraRotation);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded && context.performed)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0));
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed && interactable != null)
        {
            interactable.Interact(this);
        }
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void AdjustFuel(float time)
    {
        scaler.localScale = new Vector3(1f, Mathf.Min(maxHeight, scaler.localScale.y + burnSpeed * time), 1f);
        if (scaler.localScale.y < 0.5)
        {
            candleColliders[0].center = new Vector3(0f, 0.65f / scaler.localScale.y, 0f);
            candleColliders[1].center = new Vector3(0f, 0.5f / scaler.localScale.y, 0f);
        }
        else
        {
            candleColliders[0].center = new Vector3(0f, 1.1f, 0f);
            candleColliders[1].center = new Vector3(0f, 1.0f, 0f);
        }
        rb.mass = Mathf.Min(maxMass, rb.mass + massLossSpeed * time);
        if (inLight)
        {
            prevIntensity = Mathf.Min(maxIntensity, prevIntensity + dimSpeed * time);
        }
        else
        {
            candleLight.intensity = Mathf.Min(maxIntensity, candleLight.intensity + dimSpeed * time);
        }
        //reset the bursts
        burstThreshold.Clear();
        float section = (maxIntensity - minIntensity) / 5f;
        for (int i = 1; i < 5; i++)
        {
            if (candleLight.intensity > minIntensity + section * i)
            {
                burstThreshold.Push(minIntensity + section * i);
            }
            else
            {
                break;
            }
        }
    }

    public float RemainingTime()
    {
        return (scaler.localScale.y - minHeight) / burnSpeed;
    }

    public void LeaveLight()
    {
        inLight = false;
        candleLight.intensity = prevIntensity;
        ps.transform.rotation = Quaternion.identity;
        ps.Play();
        burnSource.Play();
    }

    public bool IsInLight()
    {
        return inLight;
    }
}
