using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrayCandleInteracter : InteractController
{
    [SerializeField] private AudioClip firstContactClip;
    [SerializeField] private AudioClip secondContactClip;
    [SerializeField] private AudioClip notEnoughFuelClip;
    [SerializeField] private AudioClip offeredFuelClip;
    [SerializeField] private AudioClip offeredFailureClip;
    [SerializeField] private AudioClip failureClip;
    [SerializeField] private AudioClip neverSeenFailureClip;
    [SerializeField] private AudioClip tooLateClip;

    private StrayCandleController candleController;
    private bool offered;
    private bool alive;

    // Start is called before the first frame update
    void Awake()
    {
        promptText = "PRESS 'E' TO SHARE YOUR LIGHT";
        promptAudio = firstContactClip;
        candleController = GetComponent<StrayCandleController>();
        alive = true;
        offered = false;
    }


    public override void Interact(PlayerController playerController)
    {
        if (alive)
        {
            if (playerController.RemainingTime() > 5.0f)
            {
                playerController.AdjustFuel(-5.0f);
                candleController.AdjustFuel(20.0f);
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
        else if (promptText == "")
        {
            playerController.promptText.text = "";
            playerController.oneShotPlayer.PlayOneShot(tooLateClip);
        }
    }

    public override void Reset() 
    {
        offered = false;
        if (alive && promptAudio == null)
        {
            promptText = "PRESS 'E' TO SHARE YOUR LIGHT";
            promptAudio = secondContactClip;
        }
    }

    public void SwapPrompt()
    {
        promptText = "";
        alive = false;

        if (promptAudio == null)
        {
            promptAudio = neverSeenFailureClip;
        }

        else if (offered)
        {
            promptAudio = offeredFailureClip;
        }
        else
        {
            promptAudio = failureClip;
        }
    }
}
