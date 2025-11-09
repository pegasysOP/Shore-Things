using UnityEngine;
using UnityEngine.InputSystem;

public class Shovel : MonoBehaviour
{
    public ParticleSystem digParticles;
    public LayerMask interactableMask;
    public LayerMask groundMask;
    public float range = 3f;

    private float digDelay = 0.3f;
    private float digTimer = 0.3f;
   
    private InputAction attackAction;
     
    void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
    }

    void Update()
    {
        digTimer -= Time.deltaTime;
        if (GameManager.Instance.LOCKED)
            return;

        if (attackAction.triggered && digTimer <= 0)
        {
            Dig();
            digTimer = digDelay;
        }
            
    }

    public void Dig()
    {
        Camera camera = GameManager.Instance.cameraController.playerCamera;
        Vector3 direction = camera.transform.forward;

        float angleFromDown = Vector3.Angle(direction, Vector3.down);
        if (angleFromDown > 60f)
            return;

        // check for item
        if (Physics.Raycast(GameManager.Instance.cameraController.playerCamera.transform.position, GameManager.Instance.cameraController.playerCamera.transform.forward, out RaycastHit hitInfo, range, interactableMask))
        {
            ItemObject itemObject = hitInfo.collider?.GetComponent<ItemObject>();
            if (itemObject != null)
            {
                itemObject.OnInteract();
                return;
            }
            else
            {
                Debug.LogWarning("Hit an interactable layer object without a ItemObject component.");
            }
        }
        
        if (Physics.Raycast(GameManager.Instance.cameraController.playerCamera.transform.position, GameManager.Instance.cameraController.playerCamera.transform.forward, out hitInfo, range, groundMask))
        {
            ParticleSystem particles = Instantiate(digParticles, hitInfo.point, Quaternion.identity);

            Destroy(particles.gameObject, particles.main.duration + particles.main.startLifetime.constantMax);
            AudioManager.Instance.PlaySfxWithPitchShifting(AudioManager.Instance.digSandClips);
        }        
    }

    private void OnDrawGizmos()
    {
        if (GameManager.Instance == null || GameManager.Instance.cameraController == null || GameManager.Instance.cameraController.playerCamera == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(GameManager.Instance.cameraController.playerCamera.transform.position, GameManager.Instance.cameraController.playerCamera.transform.position + GameManager.Instance.cameraController.playerCamera.transform.forward * range);
    }
}
