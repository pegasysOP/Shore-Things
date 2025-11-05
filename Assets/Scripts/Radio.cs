using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Radio : MonoBehaviour
{
    private Keyboard keyboard;
    private bool waitingForStatic = false;

    public LayerMask radioMask;
    public float range = 3f;

    void Start()
    {
        keyboard = Keyboard.current;
    }

    void Update()
    {
        HandleSongAdvancing();

        HandleRadioPrompt();

        HandleInput();
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
    }

    void HandleInput()
    {
        if (keyboard.eKey.wasPressedThisFrame)
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
