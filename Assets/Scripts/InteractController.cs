using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractController : MonoBehaviour
{
    public string promptText;
    public AudioClip promptAudio;

    public abstract void Interact(PlayerController playerController);
    public abstract void Reset();
}
