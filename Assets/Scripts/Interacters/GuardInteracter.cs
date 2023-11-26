using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardInteracter : InteractController
{
    [SerializeField] private AudioClip orderedClip;

    [SerializeField] private ControlledPanelPatrol paneControl;

    private bool open;

    // Start is called before the first frame update
    void Awake()
    {
        promptText = "PRESS 'E' TO OPEN THE GATE";
        open = false;
    }

    public override void Interact(PlayerController playerController)
    {
        if (!open) {
            paneControl.Control();
            //playerController.oneShotPlayer.PlayOneShot(orderedClip);
            promptText = "";
            playerController.promptText.text = "";
            open = true;
        }
    }

    public override void Reset()
    {
        promptText = "PRESS 'E' TO OPEN THE GATE";
        open = false;
        paneControl.Release();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            promptText = "PRESS 'E' TO OPEN THE GATE";
            open = false;
            paneControl.Release();
        }
    }
}
