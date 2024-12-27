using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicPlatform : MonoBehaviour
{
    public enum PlatformType
    {
        MaxPlayersToMove, // The platform will only move with a cap of the max players on it
        MinPlayerToMove   // The platform will only move with a cap of the min players on it
    }

    public enum IterateType
    {
        Cyclic, // The platform at the end will comeback to the first point
        Reverse // The platform will reverse iterate the points
    }

    [Header("Platform Settings")]
    public PlatformType platformType; // Type of logic used for the platform
    public IterateType iterateType; // Type of logic used for the platform
    public int numberOfPlayers;
    public List<Vector3> points; // List of points for the platform to follow
    public float speed = 2f;              // Movement speed of the platform
    public float stopTime = 2f;           // Wait time at each point

    private int numberOfPlayersOnPlatform = 0;
    private int currentTargetIndex = 0;   // Current target point index
    private bool isStopped = false;       // Indicates if the platform is stopped
    private Rigidbody rb;                 // Platform rigidbody
    private Vector3 previousPosition;     // Previous position of the platform
    private bool shouldMove = false;
    private bool isReversing = false;     // Indicates if the platform is moving backwards through the points

    private void Start()
    {
        this.rb = GetComponent<Rigidbody>();
        if (this.rb == null)
        {
            Debug.LogError("Rigidbody is missing on the platform. Please add one.");
            return;
        }

        if (points == null || points.Count < 2)
        {
            Debug.LogError("Please provide at least two points for the platform to move between.");
            return;
        }

        this.rb.isKinematic = true; // Configure rigidbody as kinematic
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
        // Move the platform towards the current target point using Rigidbody.MovePosition
        Vector3 target = points[currentTargetIndex];
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

        // Determine the next target index based on the iteration type
        if (iterateType == IterateType.Cyclic)
        {
            currentTargetIndex++;
            if (currentTargetIndex >= points.Count)
            {
                currentTargetIndex = 0; // Loop back to the start
            }
        }
        else if (iterateType == IterateType.Reverse)
        {
            if (!isReversing)
            {
                currentTargetIndex++;
                if (currentTargetIndex >= points.Count)
                {
                    currentTargetIndex = points.Count - 2; // Reverse direction
                    isReversing = true;
                }
            }
            else
            {
                currentTargetIndex--;
                if (currentTargetIndex < 0)
                {
                    currentTargetIndex = 1; // Reverse direction
                    isReversing = false;
                }
            }
        }

        isStopped = false; // Reactivate movement
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.numberOfPlayersOnPlatform++;
            other.transform.SetParent(transform); // Make the player a child of the platform
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.numberOfPlayersOnPlatform--;
            other.transform.SetParent(null); // Remove the player from the platform
        }
    }

    private void OnDrawGizmos()
    {
        if (points != null && points.Count > 1)
        {
            for (int i = 0; i < points.Count; i++)
            {
                // Draw spheres at each point
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(points[i], 0.2f);

                // Draw lines and arrows for direction
                if (i < points.Count - 1)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(points[i], points[i + 1]);
                    DrawArrow(points[i], points[i + 1]);
                }
            }

            // Draw arrow connecting last point to first for cyclic iteration
            if (iterateType == IterateType.Cyclic)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(points[points.Count - 1], points[0]);
                DrawArrow(points[points.Count - 1], points[0]);
            }
        }
    }

    // Helper method to draw an arrow
    private void DrawArrow(Vector3 from, Vector3 to)
    {
        Vector3 direction = (to - from).normalized;
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 150, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -150, 0) * Vector3.forward;

        float arrowHeadLength = 0.3f; // Size of the arrowhead
        Gizmos.DrawRay(to, right * arrowHeadLength);
        Gizmos.DrawRay(to, left * arrowHeadLength);
    }

}
