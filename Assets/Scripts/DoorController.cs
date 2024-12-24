using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Tooltip("Final door's position")]
    public Vector3 doorTargetPosition;

    [Tooltip("Door's speed when moving")]
    public float doorMoveSpeed = 0.8f;

    private Vector3 doorInitialPosition; // Initial door's pos
    private int activeSlabs = 0; // Active slab's counter
    private float lerpProgress = 0f; // Interp's progress

    void Start()
    {
        doorInitialPosition = transform.position;

    }

    void Update()
    {
        // Detects if a slab is activated to open the door
        if (activeSlabs > 0)
        {
            lerpProgress += doorMoveSpeed * Time.deltaTime;
        }
        else // Or close it
        {
            lerpProgress -= doorMoveSpeed * Time.deltaTime;
        }

        lerpProgress = Mathf.Clamp01(lerpProgress);

        // Interp door's pos
        transform.position = Vector3.Lerp(doorInitialPosition, doorTargetPosition, lerpProgress);
    }

    public void ActivateSlab()
    {
        activeSlabs++;
    }

    public void DeactivateSlab()
    {
        activeSlabs = Mathf.Max(0, activeSlabs - 1);
    }
}
