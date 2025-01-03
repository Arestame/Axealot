using GameCreator.Runtime.VisualScripting;
using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static UnityEngine.UI.Image;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Initial air of the player")]
    public float air = FULL_HEALTH; // Initial air's bar

    [Tooltip("Speed of air decreasing")]
    public static float airReductionSpeed = 2f; // Speed of air decreasing

    [Tooltip("Multiplier of speed reduction when cloning")]
    public float airReductionMultiplier = 2f;

    [Tooltip("Multiplier for size reduction when splitting")]
    public float sizeReductionMultiplier = 0.8f; // Size reduction multiplier

    [Tooltip("Delay of the split in time")]
    public float splitDelay = 0.5f; // Delay of the split in time

    [Tooltip("Cooldown of the split in time")]
    public float splitCooldown = 2f; // Cooldown of the split in time

    [Tooltip("Position offset when creating the clone")]
    public float splitOffset = 1f; // Relative position of the clone

    [Header("Respawn Settings")]
    [Tooltip("Respawn initial position")]
    public Vector3 currentRespawnPosition = new Vector3(0, 1, 0); // Respawn initial position

    [Tooltip("Respawn initial scale")]
    public Vector3 initialScale = new Vector3(1, 1, 1); // Respawn initial scale

    [Tooltip("Respawn initial scale")]
    public int maxNumOfSplits = 4; // Max num of splits

    // Private vars
    private const float FULL_HEALTH = 100f;
    private bool isCoolingDownSplit = false; // Local cooldown to prevent immediate re-division

    // Static vars
    private static List<PlayerController> players = new List<PlayerController>(); // Static List to handle active instances of the Player
    private static int activeSplits = 1;
    private static PlayerController mainPlayer;

    // 2) Reference to your Cinemachine TargetGroup
    [Header("Camera Settings")]
    [Tooltip("Drag your Cinemachine TargetGroup here")]
    public CinemachineTargetGroup targetGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Register player in the player list
        players.Add(this);

        if (mainPlayer == null)
        {
            mainPlayer = this; // First player in the main one
            mainPlayer.transform.position = this.currentRespawnPosition;
        }

        // 3) Add this player (main or clone) to the Cinemachine TargetGroup
        AddToTargetGroup(this.transform);

        this.air = Mathf.Clamp(this.air, 0, FULL_HEALTH); // Limitar a rango [0,100]
        UIManager.Instance.UpdateAirSlider(this.air);
    }

    // Update is called once per frame
    void Update()
    {
        this.air -= Time.deltaTime * airReductionSpeed;
        this.air = Mathf.Max(0, this.air); // We ensure the value is no less than 0

        // Update UI value
        this.air = Mathf.Clamp(this.air, 0, FULL_HEALTH); // Limitar a rango [0,100
        UIManager.Instance.UpdateAirSlider(this.air);

        if (this.air <= 0)
        {
            Respawn();
        }
    }

    // OnTriggerEnter function
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Splitter") && !isCoolingDownSplit) // Check if this player is not in cooldown
        {
            if (activeSplits < this.maxNumOfSplits) // Allow a max num of copies active at the same time
            {
                activeSplits++;
                airReductionSpeed *= this.airReductionMultiplier;
                StartCoroutine(HandleSplitWithDelay()); // Start coroutine to handle split with delay
            }
        }

        if (other.CompareTag("Respawn"))
        {
            // Get RespawnController from the trigger
            RespawnTriggerController respawnTrigger = other.GetComponent<RespawnTriggerController>();
            if (respawnTrigger != null)
            {
                // Update the current respawn position
                this.currentRespawnPosition = respawnTrigger.playerRespawnPointPosition;
            }
        }
    }

    IEnumerator HandleSplitWithDelay()
    {
        isCoolingDownSplit = true; // Prevent further splits for this instance

        yield return new WaitForSeconds(this.splitDelay); // Wait for half a second

        Split(); // Perform the split

        yield return new WaitForSeconds(this.splitCooldown); // Cooldown of 2 seconds
        isCoolingDownSplit = false; // Re-enable splitting for this instance
    }

    void Split()
    {
        // Calculate the position for the new player in a circular pattern
        Vector3 newPosition = this.CalculateCircularOffset();

        // Create a new player in the calculated position
        GameObject newPlayer = Instantiate(this.gameObject, this.transform.position + newPosition, Quaternion.identity);

        // New Player values
        PlayerController newPlayerController = newPlayer.GetComponent<PlayerController>();
        newPlayerController.air = this.air;

        // Reduce size of both players
        this.transform.localScale *= this.sizeReductionMultiplier;
        newPlayerController.transform.localScale = this.transform.localScale;

        // 4) Since this is a new clone, it will run its own Start() 
        //    and call AddToTargetGroup in that method automatically.
        //    (Alternatively, you could call `AddToTargetGroup` directly here.)

        // Apply cooldown to the new player
        StartCoroutine(ApplyCooldownToNewPlayer(newPlayerController));
    }

    // Method to calculate circular offset for the new clone
    Vector3 CalculateCircularOffset()
    {
        int cloneIndex = players.Count; // Use the current number of players to determine the position
        float angle = cloneIndex * Mathf.PI * 2f / maxNumOfSplits; // Calculate angle in radians

        // Calculate position in a circle
        float offsetX = Mathf.Cos(angle) * this.splitOffset;
        float offsetZ = Mathf.Sin(angle) * this.splitOffset;

        return new Vector3(offsetX, 0, offsetZ);
    }

    // Coroutine to apply cooldown to the new player
    IEnumerator ApplyCooldownToNewPlayer(PlayerController newPlayerController)
    {
        newPlayerController.isCoolingDownSplit = true; // Start cooldown for the new player
        yield return new WaitForSeconds(this.splitCooldown); // Cooldown duration
        newPlayerController.isCoolingDownSplit = false; // Allow splitting again for the new player
    }

    void Respawn()
    {
        // Only the main player triggers the full Respawn process
        if (this != mainPlayer) return;

        StartCoroutine(HandleRespawn());
    }

    IEnumerator HandleRespawn()
    {
        // Destroy clones
        yield return DestroyClonesWithDelay();

        // Reset value of current splits
        activeSplits = 1;

        // Restore values for main player
        mainPlayer.air = FULL_HEALTH;
        mainPlayer.currentRespawnPosition = this.currentRespawnPosition;

        // Detach the player from any parent object
        mainPlayer.transform.SetParent(null);

        mainPlayer.transform.position = this.currentRespawnPosition;
        mainPlayer.transform.localScale = initialScale;
        airReductionSpeed = 1.0f;

        // Reset UI value when respawning
        UIManager.Instance.UpdateAirSlider(mainPlayer.air);
    }

    IEnumerator DestroyClonesWithDelay()
    {
        // We create a temporary list to allocate the clones to destroy
        List<PlayerController> clonesToDestroy = new List<PlayerController>();

        // Add clones to new list
        for (int i = players.Count - 1; i > 0; i--)
        {
            PlayerController clone = players[i];
            if (clone != null && clone.gameObject != null)
            {
                clonesToDestroy.Add(clone);
            }
        }

        // Delete clones from the original list and destroy them
        foreach (PlayerController clone in clonesToDestroy)
        {
            if (clone != null && clone.gameObject != null)
            {
                players.Remove(clone); // Delete clone from main list

                clone.transform.position = new Vector3(0, 0, 0); // Move clones to force exiting if dying on a trigger

                yield return new WaitForSeconds(0.2f);

                // Verify before destroying
                if (clone != null && clone.gameObject != null)
                {
                    Destroy(clone.gameObject);
                }
            }
        }
    }

    // 5) When we destroy this instance, remove it from the Cinemachine TargetGroup
    private void OnDestroy()
    {
        players.Remove(this);
        RemoveFromTargetGroup(this.transform);
    }

    // ------------------ HELPER METHODS FOR CINEMACHINE TARGETGROUP ------------------

    /// <summary>
    /// Adds a transform to the CinemachineTargetGroup if it's assigned
    /// </summary>
    private void AddToTargetGroup(Transform playerTransform)
    {
        if (targetGroup == null) return;

        // (A) EASIER WAY: Just call the built-in method
        targetGroup.AddMember(playerTransform, 1f, 1f);

        // (B) OR MANUAL WAY: 
        // var newTargets = new List<CinemachineTargetGroup.Target>(targetGroup.Targets);
        // var t = new CinemachineTargetGroup.Target
        // {
        //     Object = playerTransform,
        //     Weight = 1f,
        //     Radius = 2f
        // };
        // newTargets.Add(t);
        // targetGroup.Targets = newTargets;
    }

    private void RemoveFromTargetGroup(Transform playerTransform)
    {
        if (targetGroup == null) return;

        // (A) EASIER WAY: 
        targetGroup.RemoveMember(playerTransform);

        // (B) OR MANUAL WAY:
        // var newTargets = new List<CinemachineTargetGroup.Target>(targetGroup.Targets);
        // newTargets.RemoveAll(x => x.Object == playerTransform);
        // targetGroup.Targets = newTargets;
    }
}

