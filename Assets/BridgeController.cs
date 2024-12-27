using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BridgeController : MonoBehaviour
{
    [Header("Bridge Settings")]
    public int maxPlayersAllowed; // Maximum number of players allowed

    private int currentPlayersOnBridge = 0; // Current players on the bridge
    private GameObject bridgeChildObject; // Reference to the child object with the BoxCollider

    void Start()
    {
        // Find the specific child object with the BoxCollider, but not the parent
        foreach (Transform child in transform)
        {
            if (child.GetComponent<BoxCollider>() != null)
            {
                bridgeChildObject = child.gameObject;
                break;
            }
        }

        if (bridgeChildObject == null)
        {
            Debug.LogError("No child object with a BoxCollider was found on the bridge.");
        }
        else
        {
            Debug.Log($"Child object with BoxCollider found: {bridgeChildObject.name}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayersOnBridge++;
            Debug.Log($"Player entered the bridge. Current players: {currentPlayersOnBridge}");
            UpdateBridgeState();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayersOnBridge--;
            Debug.Log($"Player left the bridge. Current players: {currentPlayersOnBridge}");
            UpdateBridgeState();
        }
    }

    void UpdateBridgeState()
    {
        if (bridgeChildObject == null)
            return;

        bool shouldEnable = currentPlayersOnBridge <= maxPlayersAllowed;

        // Start the coroutine to apply the change with a delay
        StartCoroutine(SetActiveWithDelay(bridgeChildObject, shouldEnable, 1f));
    }

    IEnumerator SetActiveWithDelay(GameObject targetObject, bool state, float delay)
    {
        // Wait for the specified time (1 second in this case)
        yield return new WaitForSeconds(delay);

        // Change the active state of the child object
        if (targetObject.activeSelf != state)
        {
            targetObject.SetActive(state);
            Debug.Log($"The child object is now {(state ? "enabled" : "disabled")} after {delay} seconds.");
        }
    }
}
