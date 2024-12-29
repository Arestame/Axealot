using UnityEngine;

public class DirectionalSlab : MonoBehaviour
{
    [Header("Directional Settings")]
    public Vector3 direction; // Direction of the platform
    public ControlledPlatform controlledPlatform; // Platform's reference

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Just players
        {
            this.controlledPlatform.StartMoving(this.direction); // Send info to the platform
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.controlledPlatform.StopMoving(); // Stop Platform's movement
        }
    }
}

