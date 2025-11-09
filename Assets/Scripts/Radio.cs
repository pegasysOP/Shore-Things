using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Radio : MonoBehaviour
{
    private InputAction interactAction;
    private bool waitingForStatic = false;

    public LayerMask radioMask;
    public float range = 3f;

    void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {
        HandleSongAdvancing();

        HandleRadioPrompt();
    }

    void HandleSongAdvancing()
    {
        AudioSource musicSource = AudioManager.Instance.musicSource;

        if (waitingForStatic && !musicSource.isPlaying)
        {
            waitingForStatic = false;
            AudioManager.Instance.AdvanceSong();
        }
    }

    void HandleRadioPrompt()
    {
        if (!Physics.Raycast(
            GameManager.Instance.cameraController.playerCamera.transform.position,
            GameManager.Instance.cameraController.playerCamera.transform.forward,
            out RaycastHit hit, range, radioMask)
        )
        {
            GameManager.Instance.hudController.ShowRadioPrompt(false);
            return;
        }

        GameManager.Instance.hudController.ShowRadioPrompt(true);
        HandleInput();
    }

    void HandleInput()
    {
        if (interactAction.triggered)
        {
            PlayStaticThenNextSong();
        }
    }

    void PlayStaticThenNextSong()
    {
        AudioClip staticClip = AudioManager.Instance.staticClip;

        if (AudioManager.Instance.staticClip == null)
        {
            AudioManager.Instance.AdvanceSong();
            return;
        }

        waitingForStatic = true;
        AudioManager.Instance.PlayMusic(staticClip, AudioManager.FadeType.None);
    }
}
