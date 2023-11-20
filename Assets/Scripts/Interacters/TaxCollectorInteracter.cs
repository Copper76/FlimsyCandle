using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxCollectorInteracter : InteractController
{
    [SerializeField] private AudioClip notEnoughFuelClip;
    [SerializeField] private AudioClip offeredFuelClip;
    [SerializeField] private AudioClip payAgainClip;

    [SerializeField] private ControlledPanelPatrol paneControl;

    // Start is called before the first frame update
    void Awake()
    {
        promptText = "PRESS 'E' TO OFFER TAX";
    }

    public override void Interact(PlayerController playerController)
    {
        if (playerController.RemainingTime() > 5.0f)
        {
            playerController.AdjustFuel(-5.0f);
            paneControl.Release();
            playerController.oneShotPlayer.PlayOneShot(offeredFuelClip);
            promptText = "";
            playerController.promptText.text = "";
        }
        else
        {
            playerController.oneShotPlayer.PlayOneShot(notEnoughFuelClip);
        }
    }

    public override void Reset()
    {
        promptText = "PRESS 'E' TO OFFER TAX";
        paneControl.Release();
    }
}
