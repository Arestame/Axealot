using UnityEngine;

public class DynamicPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    public Vector3 pointA = Vector3.zero; // Point A
    public Vector3 pointB = Vector3.right; // Point B
    public float speed = 2f;              // Movement speed of the platform
    public float stopTime = 2f;           // Wait time on each point

    private Vector3 target;               // Target at the moment
    private bool isStopped = false;       // Indicates if Platform stopped
    private Rigidbody rb;                 // Platform rigidbody
    private Vector3 previousPosition;     // Previous position of the platform

    private void Start()
    {
        this.rb = GetComponent<Rigidbody>();
        if (this.rb == null)
        {
            Debug.LogError("Rigidbody is missing on the platform. Please add one.");
            return;
        }

        this.rb.isKinematic = true; // Configure rigidbody as kinematic
        this.target = this.pointB;       // At start it moves to point b
        this.previousPosition = this.transform.position; // Initialize previous position
    }

    private void FixedUpdate()
    {
        if (!this.isStopped)
        {
            MovePlatform(); // Mover la plataforma si no está detenida
        }

        // Calcula el desplazamiento de la plataforma
        Vector3 displacement = transform.position - previousPosition;

        // Aplica el desplazamiento a todos los objetos hijos (jugador incluido)
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Player"))
            {
                child.position += displacement; // Mueve al jugador junto con la plataforma
            }
        }

        previousPosition = transform.position; // Actualiza la posición anterior
    }

    private void MovePlatform()
    {
        // Mueve la plataforma hacia el objetivo usando Rigidbody.MovePosition
        Vector3 newPosition = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition); // Usa MovePosition para un movimiento suave

        // Cambia el destino si alcanza el objetivo
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            StartCoroutine(StopPlatform());
        }
    }

    private System.Collections.IEnumerator StopPlatform()
    {
        isStopped = true; // Detener la plataforma
        yield return new WaitForSeconds(stopTime); // Esperar
        target = target == pointA ? pointB : pointA; // Cambiar el objetivo
        isStopped = false; // Reactivar el movimiento
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the platform trigger.");
            other.transform.SetParent(transform); // Hacer al jugador hijo de la plataforma
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the platform trigger.");
            other.transform.SetParent(null); // Quitar al jugador de la plataforma
        }
    }

    private void OnDrawGizmos()
    {
        // Visualización en el editor de los puntos de destino
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pointA, pointB);
        Gizmos.DrawSphere(pointA, 0.2f);
        Gizmos.DrawSphere(pointB, 0.2f);
    }
}
