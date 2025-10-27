using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MetalDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public float range = 5f;
    public float ignoreAngleRange = 1.0f;
    public float maxAngle = 90f;
    public LayerMask metalDetectionMask;

    [Header("Battery Settings")]
    public float maxBattery = 100f;
    public float dischargeRate = 0f;
    public float rechargeRate = 0f;
    private float battery;

    [Header("Audio Settings")]
    public AudioClip metalDetectorBeep;
    public float minPitch = 0.6f;
    public float maxPitch = 1.4f;
    public float minBeepInterval = 0.08f;
    public float maxBeepInterval = 0.6f;
    private float beepInterval = 0.6f;

    [Header("Animation")]
    public Animator animator;

    private float beepTimer = 0f;
    private Mouse mouse;
    private AudioSource audioSource;

    private void Start()
    {
        mouse = Mouse.current;
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = metalDetectorBeep;
        battery = maxBattery;
    }

    private void Update()
    {
        beepTimer += Time.deltaTime;

        if (GameManager.Instance.LOCKED)
        {
            RechargeBatteryOverTime();
            return;
        }
            

        if (mouse.rightButton.isPressed)
        {
            Detect();
        }
        else
        {
            animator.SetBool("isDetecting", false);
            beepTimer = 0f;
            audioSource.Stop();
            RechargeBatteryOverTime();
        }
    }

    public void Detect()
    {
        DischargeBattery();

        if(battery <= 0f)
        {
            animator.SetBool("isDetecting", false);
            return;
        }

        // don't detect if looking above horizontal
        if (Camera.main.transform.forward.y > 0)
        {
            beepTimer = 0f;
            animator.SetBool("isDetecting", false);
            return;
        }

        animator.SetBool("isDetecting", true);
        Vector3 detectionPoint = transform.position + transform.forward * 0.4f; // just in front of player

        Collider[] hitColliders = Physics.OverlapSphere(detectionPoint, range, metalDetectionMask);
        if (hitColliders.Length < 1)
        {
            beepTimer = 0f;
            return;
        }

        Vector2 lookDir = new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z).normalized;

        bool foundTarget = false;
        float closestDistance = float.MaxValue;
        float closestAngle = float.MaxValue;

        foreach (Collider hit in hitColliders)
        {
            Vector2 targetVector = new Vector2(hit.transform.position.x - detectionPoint.x,
                                               hit.transform.position.z - detectionPoint.z);

            float distance = targetVector.magnitude;
            float horizontalAngle = Vector2.Angle(lookDir, targetVector.normalized);

            bool isValid = horizontalAngle <= maxAngle || distance < ignoreAngleRange;
            if (!isValid)
                continue;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestAngle = Vector2.Angle(lookDir, targetVector.normalized);
                foundTarget = true;
            }
        }

        if (!foundTarget)
        {
            beepTimer = 0f;
            return;
        }

        float proximity = Mathf.Clamp01(1f - (closestDistance / (closestAngle < maxAngle ? range : ignoreAngleRange)));
        beepInterval = Mathf.Lerp(maxBeepInterval, minBeepInterval, proximity);

        float angleProximity = 1f - Mathf.Clamp01(closestAngle / maxAngle);
        float curvedAngle = Mathf.Pow(angleProximity, 2f);
        audioSource.pitch = closestDistance < ignoreAngleRange ? maxPitch : Mathf.Lerp(minPitch, maxPitch, curvedAngle);

        Debug.Log($"Pitch: {audioSource.pitch:F2} | Horizontal Angle: {closestAngle:F1}� | Horizontal Distance: {closestDistance:F2}");

        if (beepTimer >= beepInterval)
        {
            PlayBeep(proximity);
            beepTimer = 0f;
        }
    }

    private void PlayBeep(float proximity)
    {
        if (audioSource == null || metalDetectorBeep == null)
            return;

        audioSource.Stop();
        audioSource.clip = metalDetectorBeep;
        audioSource.Play();

        AudioManager.Instance.StartDuckAudio(AudioManager.Instance.musicSource, 0.1f, 0.5f, 0.5f);

    }

    private void DischargeBattery()
    {
        battery -= dischargeRate * Time.deltaTime;
        GameManager.Instance.hudController.UpdateBatteryText(battery);
        if (battery <= 0f)
        {
            battery = 0f;
            return;
        }
    }

    public void RechargeBatteryOverTime()
    {
        battery += rechargeRate * Time.deltaTime;

        if (battery >= maxBattery)
        {
            battery = maxBattery;
        }

        GameManager.Instance.hudController.UpdateBatteryText(battery);
    }

    public void RechargeBattery(float amount)
    {
        battery += amount;
        
        if(battery >= maxBattery)
        {
            battery = maxBattery;

        }

        GameManager.Instance.hudController.UpdateBatteryText(battery);
    }
}