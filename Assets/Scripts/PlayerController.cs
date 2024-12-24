using GameCreator.Runtime.VisualScripting;
using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Initial air of the player")]
    public float air = FULL_HEALTH; // Initial air's bar

    [Tooltip("Speed of air decreasing")]
    public float airReductionSpeed = 2f; // Speed of air decreasing

    [Tooltip("Multiplier for size reduction when splitting")]
    public float sizeReductionMultiplier = 0.8f; // Size reduction multiplier

    [Tooltip("Delay of the split in time")]
    public float splitDelay = 0.5f; // Delay of the split in time

    [Tooltip("Cooldown of the split in time")]
    public float splitCooldown = 2f; // Cooldown of the split in time

    [Tooltip("Position offset when creating the clone")]
    public Vector3 splitOffset = new Vector3(1, 0, 0); // Relative position of the clone

    [Header("Respawn Settings")]
    [Tooltip("Respawn initial position")]
    public Vector3 initialPosition = new Vector3(0, 1, 0); // Respawn initial position

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Register player in the player list
        players.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        this.air -= Time.deltaTime * this.airReductionSpeed;
        Debug.Log("Player's air = " + this.air);
        if (this.air < 0)
        {
            this.Respawn();
        }
    }

    // OnTriggerEnter function
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Splitter") && !isCoolingDownSplit) // Check if this player is not in cooldown
        {
            if(activeSplits < this.maxNumOfSplits) // Allow a max num of copies active at the same time
            {
                activeSplits++;
                StartCoroutine(HandleSplitWithDelay()); // Start coroutine to handle split with delay
            }
        }
    }

    IEnumerator HandleSplitWithDelay()
    {
        Debug.Log($"{gameObject.name} is preparing to split...");
        isCoolingDownSplit = true; // Prevent further splits for this instance

        yield return new WaitForSeconds(this.splitDelay); // Wait for half a second

        Split(); // Perform the split

        yield return new WaitForSeconds(this.splitCooldown); // Cooldown of 2 seconds
        isCoolingDownSplit = false; // Re-enable splitting for this instance
    }

    void Split()
    {
        // Decrease by half the air's value
        this.air /= 2;

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

        // Apply cooldown to the new player
        StartCoroutine(ApplyCooldownToNewPlayer(newPlayerController));

        Debug.Log($"{gameObject.name} divided!");
    }

    // Method to calculate circular offset for the new clone
    Vector3 CalculateCircularOffset()
    {
        int cloneIndex = players.Count; // Use the current number of players to determine the position
        float angle = cloneIndex * Mathf.PI * 2f / maxNumOfSplits; // Calculate angle in radians
        float radius = 2f; // Distance from the center (adjust for spacing)

        // Calculate position in a circle
        float offsetX = Mathf.Cos(angle) * radius;
        float offsetZ = Mathf.Sin(angle) * radius;

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
        Debug.Log("Player died!");

        // Delete all players except the first one
        for (int i = players.Count - 1; i > 0; i--)
        {
            Destroy(players[i].gameObject);
            players.RemoveAt(i);
        }

        // Restore values for main player
        PlayerController mainPlayer = players[0];
        mainPlayer.air = FULL_HEALTH;
        mainPlayer.transform.position = initialPosition;
        mainPlayer.transform.localScale = initialScale;
    }

    // When we destroy this instance, delete it from the list
    private void OnDestroy()
    {
        players.Remove(this);
    }
}
