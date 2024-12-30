using UnityEngine;
using System.Collections.Generic;

public class DoorController : MonoBehaviour
{
    public enum DoorLogic
    {
        AnySlabActive, // The door opens if any slab is active
        AllSlabsActive // The door opens only if all slabs are active
    }

    [Header("Door Settings")]
    [Tooltip("Select the logic for opening the door")]
    public DoorLogic doorLogic; // Type of logic used for the door

    [Tooltip("Speed of the door's movement")]
    public float doorMoveSpeed = 0.8f; // Speed at which the door moves

    [Tooltip("List of slabs that control this door")]
    public List<Slab> slabs = new List<Slab>(); // List of slabs associated with the door

    private Vector3 doorInitialPosition; // Initial position of the door
    private float lerpProgress = 0f;     // Progress of the door's movement (0 to 1)

    // We define how far the door moves in the negative Y direction when opening
    private Vector3 doorOpenOffset = new Vector3(0f, -3f, 0f);

    void Start()
    {
        // Store the initial position of the door
        doorInitialPosition = transform.position;
    }

    void Update()
    {
        // Determine whether the door should open or close based on the selected logic
        bool shouldOpen = doorLogic switch
        {
            DoorLogic.AnySlabActive => IsAnySlabActive(),
            DoorLogic.AllSlabsActive => AreAllSlabsActive(),
            _ => false
        };

        // Open or close the door smoothly based on the calculated condition
        if (shouldOpen)
        {
            lerpProgress += doorMoveSpeed * Time.deltaTime;
        }
        else
        {
            lerpProgress -= doorMoveSpeed * Time.deltaTime;
        }

        // Clamp the lerp progress to ensure it stays between 0 and 1
        lerpProgress = Mathf.Clamp01(lerpProgress);

        // Smoothly interpolate the door's position from its initial position
        // to 2 units below in the -Y direction
        Vector3 targetPos = doorInitialPosition + doorOpenOffset;
        transform.position = Vector3.Lerp(doorInitialPosition, targetPos, lerpProgress);
    }

    // Check if any slab is active
    private bool IsAnySlabActive()
    {
        foreach (Slab slab in slabs)
        {
            if (slab.IsActive()) return true; // If at least one slab is active, return true
        }
        return false; // No active slabs
    }

    // Check if all slabs are active
    private bool AreAllSlabsActive()
    {
        foreach (Slab slab in slabs)
        {
            if (!slab.IsActive()) return false; // If any slab is inactive, return false
        }
        return true; // All slabs are active
    }
}
