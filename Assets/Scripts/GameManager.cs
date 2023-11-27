using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject fuelPrefab;
    [SerializeField] private Vector3[] fuelPositions;
    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private AudioSource burnSource;
    [SerializeField] private AudioSource oneShotPlayer;
    //[SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private CameraController cam;
    [SerializeField] private Transform NPCs;

    [SerializeField] private Transform previousCandle;
    [SerializeField] private GameObject StartMenu;
    private bool opening;

    private Transform playerHolder;
    private Transform fuelHolder;
    private ClipDepot clipDepot;
    private int latestLamp;
    private float openingTimer;

    private float camSpeed;

    // Start is called before the first frame update

    void Awake()
    {
        latestLamp = -1;
        playerHolder = transform.GetChild(0);
        fuelHolder = transform.GetChild(1);

        //vcam.LookAt = player.transform;
        //vcam.Follow = player.transform;
        foreach (Vector3 pos in fuelPositions)
        {
            GameObject fuel = Instantiate(fuelPrefab, fuelHolder);
            fuel.transform.position = pos;
        }
        Cursor.lockState = CursorLockMode.None;
        burnSource.Play();
    }

    void Start()
    {
        camSpeed = (cam.initPos - cam.transform.position).magnitude / 5f;
    }

    void FixedUpdate()
    {
        if (opening)
        {
            if (openingTimer  < 1.0f)
            {
                openingTimer += Time.deltaTime;
            }
            else
            {
                if ((cam.transform.position - cam.initPos).sqrMagnitude > 0.1f)
                {
                    Vector3 dir = (cam.initPos - cam.transform.position).normalized;
                    cam.transform.position += camSpeed * dir * Time.deltaTime;
                }
                else
                {
                    opening = false;
                    Destroy(previousCandle.gameObject);
                    cam.ResetCam();
                    SpawnPlayer();
                }
            }
        }
    }

    public void Restart()
    {
        SpawnPlayer();
        //vcam.LookAt = player.transform;
        //vcam.Follow = player.transform;
        foreach (Transform fuel in fuelHolder)
        {
            fuel.gameObject.SetActive(true);
        }
        foreach (Transform t in NPCs)
        {
            t.GetComponent<InteractController>().Reset();
        }
    }

    private void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, playerHolder);
        player.transform.position = spawnPos;
        clipDepot = GetComponent<ClipDepot>();
        player.GetComponent<PlayerController>().clipDepot = clipDepot;
        player.GetComponent<PlayerController>().burnSource = burnSource;
        player.GetComponent<PlayerController>().oneShotPlayer = oneShotPlayer;
        player.GetComponent<PlayerController>().cam = cam;
        int clip = UnityEngine.Random.Range(0, clipDepot.beginClips.Length);
        oneShotPlayer.PlayOneShot(clipDepot.beginClips[clip]);
    }

    public void UpdateSpawnPoint(int lampNum, Vector3 spawnPos)
    {
        if (lampNum > latestLamp)
        {
            this.spawnPos = spawnPos;
        }
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void StartLevel()
    {
        opening = true;
        StartMenu.SetActive(false);
        previousCandle.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
        previousCandle.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        burnSource.Stop();
    }

    public void KillLights()
    {
        foreach (Transform fuel in fuelHolder)
        {
            fuel.gameObject.SetActive(false);
        }

    }
}
