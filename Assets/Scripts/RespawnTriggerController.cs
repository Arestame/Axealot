using UnityEngine;

public class RespawnTriggerController : MonoBehaviour
{

    [Tooltip("Player Respawn Point Position")]
    public Vector3 playerRespawnPointPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(playerRespawnPointPosition, 0.5f);
    }
}
