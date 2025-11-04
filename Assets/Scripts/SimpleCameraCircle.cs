using UnityEngine;

public class SimpleCameraCircle : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform target;        
    public float radius = 10f;      
    public float orbitSpeed = 10f; 

    [Header("Height Oscillation")]
    public float baseHeight = 2f;   
    public float amplitude = 1f;    
    public float frequency = 1f;   

    private float angle = 0f;

    void Start()
    {
        if (target != null)
        {
            Vector3 offset = transform.position - target.position;
            angle = Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg;
        }
    }

    void Update()
    {
        if (target == null) return;

        angle += orbitSpeed * Time.deltaTime;
        if (angle > 360f) angle -= 360f;
        float rad = angle * Mathf.Deg2Rad;

        float x = Mathf.Cos(rad) * radius;
        float z = Mathf.Sin(rad) * radius;

        float y = baseHeight + Mathf.Sin(Time.time * frequency) * amplitude;

        transform.position = new Vector3(x, y, z) + target.position;
        transform.LookAt(target);
    }
}
