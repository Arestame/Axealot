using UnityEngine;

public class Slab : MonoBehaviour
{
    [Tooltip("Door that this slab controls")]
    public DoorController doorController;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && doorController != null)
        {
            doorController.ActivateSlab();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && doorController != null)
        {
            doorController.DeactivateSlab();
        }
    }
}