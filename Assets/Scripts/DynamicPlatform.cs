using UnityEngine;

public class DynamicPlatform : MonoBehaviour
{
    public enum PlatformType
    {
        MaxPlayersToMove, // The platform will only move with a cap of the max players on it
        MinPlayerToMove   // The platform will only move with a cap of the min players on it
    }

    [Header("Platform Settings")]
    public PlatformType platformType; // Type of logic used for the platform
    public int numberOfPlayers;
    public Vector3 pointA = Vector3.zero; // Point A
    public Vector3 pointB = Vector3.right; // Point B
    public float speed = 2f;              // Movement speed of the platform
    public float stopTime = 2f;           // Wait time at each point

    private int numberOfPlayersOnPlatform = 0;
    private Vector3 target;               // Current target
    private bool isStopped = false;       // Indicates if the platform is stopped
    private Rigidbody rb;                 // Platform rigidbody
    private Vector3 previousPosition;     // Previous position of the platform
    private bool shouldMove = false;

    private void Start()
    {
        this.rb = GetComponent<Rigidbody>();
        if (this.rb == null)
        {
            Debug.LogError("Rigidbody is missing on the platform. Please add one.");
            return;
        }

        this.rb.isKinematic = true; // Configure rigidbody as kinematic
        this.target = this.pointB;       // Start moving to point B
        this.previousPosition = this.transform.position; // Initialize previous position
    }

    private void FixedUpdate()
    {
        if (!this.isStopped)
        {
            if (this.platformType == PlatformType.MaxPlayersToMove)
            {
                this.shouldMove = this.numberOfPlayersOnPlatform <= this.numberOfPlayers;
            }
            else if (this.platformType == PlatformType.MinPlayerToMove)
            {
                this.shouldMove = this.numberOfPlayersOnPlatform >= this.numberOfPlayers;
            }

            if (this.shouldMove)
            {
                MovePlatform();
            }
        }

        // Calculate the displacement of the platform
        Vector3 displacement = transform.position - previousPosition;

        // Apply the displacement to all child objects (including the player)
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Player"))
            {
                child.position += displacement; // Move the player along with the platform
            }
        }

        previousPosition = transform.position; // Update the previous position
    }

    private void MovePlatform()
    {
        // Move the platform towards the target using Rigidbody.MovePosition
        Vector3 newPosition = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition); // Use MovePosition for smooth movement

        // Change the target if the platform reaches the destination
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            StartCoroutine(StopPlatform());
        }
    }

    private System.Collections.IEnumerator StopPlatform()
    {
        isStopped = true; // Stop the platform
        yield return new WaitForSeconds(stopTime); // Wait
        target = target == pointA ? pointB : pointA; // Switch the target
        isStopped = false; // Reactivate movement
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the platform trigger.");
            this.numberOfPlayersOnPlatform++;
            other.transform.SetParent(transform); // Make the player a child of the platform
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the platform trigger.");
            this.numberOfPlayersOnPlatform--;
            other.transform.SetParent(null); // Remove the player from the platform
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize the destination points in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pointA, pointB);
        Gizmos.DrawSphere(pointA, 0.2f);
        Gizmos.DrawSphere(pointB, 0.2f);
    }
}
