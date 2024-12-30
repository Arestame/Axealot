using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CameraRotationController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Transform de la cámara (u objeto) que deseas rotar.")]
    public Transform cameraTransform;

    [Header("Ajustes de Rotación")]
    [Tooltip("Grados a rotar cuando se llama Next o Previous.")]
    public float rotationStep = 90f;

    [Tooltip("Tiempo que tarda en completarse la rotación.")]
    public float rotationDuration = 0.3f;

    [Tooltip("Curva de animación para el lerp (por ejemplo, un Ease In con Hard Out).")]
    public AnimationCurve rotationCurve;

    [Header("Acciones de Input (Nuevo Input System)")]
    [Tooltip("Acción que se activará para avanzar la rotación (Next).")]
    public InputActionReference nextAction;

    [Tooltip("Acción que se activará para retroceder la rotación (Previous).")]
    public InputActionReference previousAction;

    private bool isRotating = false;

    private void OnEnable()
    {
        // Suscribirse a los eventos 'performed' de cada acción
        if (nextAction != null && nextAction.action != null)
        {
            nextAction.action.performed += OnNext;
            nextAction.action.Enable();
        }

        if (previousAction != null && previousAction.action != null)
        {
            previousAction.action.performed += OnPrevious;
            previousAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        // Desuscribirse de los eventos y deshabilitar
        if (nextAction != null && nextAction.action != null)
        {
            nextAction.action.performed -= OnNext;
            nextAction.action.Disable();
        }

        if (previousAction != null && previousAction.action != null)
        {
            previousAction.action.performed -= OnPrevious;
            previousAction.action.Disable();
        }
    }

    /// <summary>
    /// Llamado cuando se recibe el input "Next".
    /// </summary>
    private void OnNext(InputAction.CallbackContext ctx)
    {
        if (!isRotating)
        {
            StartCoroutine(RotateCamera(+rotationStep));
        }
    }

    /// <summary>
    /// Llamado cuando se recibe el input "Previous".
    /// </summary>
    private void OnPrevious(InputAction.CallbackContext ctx)
    {
        if (!isRotating)
        {
            StartCoroutine(RotateCamera(-rotationStep));
        }
    }

    /// <summary>
    /// Corrutina que rota la cámara únicamente en el eje Y, 
    /// preservando los valores originales de rotación en X y Z.
    /// </summary>
    private IEnumerator RotateCamera(float deltaY)
    {
        isRotating = true;

        // Leemos la rotación inicial (en Euler) antes de empezar
        Vector3 startEuler = cameraTransform.eulerAngles;

        // Ángulo inicial y final en el eje Y
        float startY = startEuler.y;
        float endY = startY + deltaY;

        float timeElapsed = 0f;

        while (timeElapsed < rotationDuration)
        {
            timeElapsed += Time.deltaTime;

            // Progreso de 0 a 1
            float t = timeElapsed / rotationDuration;

            // Factor de interpolación según la curva
            float curveValue = rotationCurve.Evaluate(t);

            // Interpolamos el ángulo Y, pero conservamos X y Z originales
            float currentY = Mathf.Lerp(startY, endY, curveValue);

            cameraTransform.rotation = Quaternion.Euler(
                startEuler.x,  // X original
                currentY,      // Y interpolado
                startEuler.z   // Z original
            );

            yield return null;
        }

        // Ajustamos la rotación al valor final exacto
        cameraTransform.rotation = Quaternion.Euler(
            startEuler.x,
            endY,
            startEuler.z
        );

        isRotating = false;
    }
}
