using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxCollectorInteracter : InteractController
{
    [SerializeField] private AudioClip notEnoughFuelClip;
    [SerializeField] private AudioClip offeredFuelClip;

    [SerializeField] private ControlledPanelPatrol paneControl;

    private bool offered;

    // Start is called before the first frame update
    void Awake()
    {
        promptText = "PRESS 'E' TO PAY TAX";
        offered = false;
    }

    public override void Interact(PlayerController playerController)
    {
        if (!offered)
        {
            if (playerController.RemainingTime() > 5.0f)
            {
                playerController.AdjustFuel(-5.0f);
                paneControl.Control();
                playerController.oneShotPlayer.PlayOneShot(offeredFuelClip);
                promptText = "";
                playerController.promptText.text = "";
                offered = true;
            }
            else
            {
                playerController.oneShotPlayer.PlayOneShot(notEnoughFuelClip);
            }
        }
    }

    public override void Reset()
    {
        promptText = "PRESS 'E' TO PAY TAX";
        paneControl.Release();
        offered = false;
    }
}
