using UnityEngine;

public class Slab : MonoBehaviour
{
    [Tooltip("Door that this slab controls")]
    public DoorController doorController; // The door this slab is associated with

    private bool isActive = false; // The current state of the slab (active/inactive)

    void OnTriggerEnter(Collider other)
    {
        // Check if the player steps on the slab
        if (other.CompareTag("Player") && doorController != null)
        {
            isActive = true; // Mark the slab as active
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the player leaves the slab
        if (other.CompareTag("Player") && doorController != null)
        {
            isActive = false; // Mark the slab as inactive
        }
    }

    // Returns whether the slab is currently active
    public bool IsActive()
    {
        return isActive;
    }
}
