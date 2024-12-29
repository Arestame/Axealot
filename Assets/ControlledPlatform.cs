using UnityEngine;

public class ControlledPlatform : MonoBehaviour
{
    [Header("Controlled Platform Settings")]
    public float speed = 2f; // Platform's speed when moving

    private Vector3 currentDirection = Vector3.zero; // Direction of the platform
    private Rigidbody rb; // Platform's rigidbody
    private Vector3 previousPosition; // Previous pos of the platforn
    private bool isMoving = false; // Is platform moving
    private float marginBoxCastDistance = 0.05f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        this.rb = GetComponent<Rigidbody>();
        if (this.rb == null)
        {
            Debug.LogError("Rigidbody is missing on the platform. Please add one.");
            return;
        }

        this.rb.isKinematic = true; // PLatform in kinematic
        this.previousPosition = this.transform.position;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isMoving)
        {
            if (!IsPathBlocked())
            {

                // Moves platform in a certain direction
                Vector3 newPosition = this.transform.position + this.currentDirection * speed * Time.fixedDeltaTime;
                this.rb.MovePosition(newPosition);

                // Calcula el desplazamiento y lo aplica a los jugadores en la plataforma
                Vector3 displacement = this.transform.position - this.previousPosition;
                foreach (Transform child in transform)
                {
                    if (child.CompareTag("Player"))
                    {
                        child.position += displacement; // Mueve al jugador con la plataforma
                    }
                }

                this.previousPosition = this.transform.position; // Update previous pos
            }
            else
            {
                StopMoving(); // Detener el movimiento si el camino está bloqueado
            }   
        }
    }

    public void StartMoving(Vector3 direction)
    {
        this.currentDirection = direction.normalized; // Establecer la dirección normalizada
        this.isMoving = true; // Activar el movimiento
    }

    public void StopMoving()
    {
        this.currentDirection = Vector3.zero; // Resetea la dirección
        this.isMoving = false; // Detener el movimiento
    }

    private bool IsPathBlocked()
    {
        // Calculate BoxCast Size based on its scale
        Vector3 boxSize = new Vector3(
            transform.localScale.x / 2, // Mitad del tamaño en X
            transform.localScale.y / 2, // Mitad del tamaño en Y
            transform.localScale.z / 2  // Mitad del tamaño en Z
        );

        // Throw a raycast from our current position to the current direction
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, boxSize, currentDirection, out hit, Quaternion.identity, this.marginBoxCastDistance))
        {
            return true; // If detects an object, path is blocked
        }

        return false; // If there's not, path free
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(this.transform); // Make the player a child of the platform
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null); // Remove the player from the platform
        }
    }
}
