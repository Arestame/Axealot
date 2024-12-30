using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CameraRotationController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Transform de la c�mara (u objeto) que deseas rotar.")]
    public Transform cameraTransform;

    [Header("Ajustes de Rotaci�n")]
    [Tooltip("Grados a rotar cuando se llama Next o Previous.")]
    public float rotationStep = 90f;

    [Tooltip("Tiempo que tarda en completarse la rotaci�n.")]
    public float rotationDuration = 0.3f;

    [Tooltip("Curva de animaci�n para el lerp (por ejemplo, un Ease In con Hard Out).")]
    public AnimationCurve rotationCurve;

    [Header("Acciones de Input (Nuevo Input System)")]
    [Tooltip("Acci�n que se activar� para avanzar la rotaci�n (Next).")]
    public InputActionReference nextAction;

    [Tooltip("Acci�n que se activar� para retroceder la rotaci�n (Previous).")]
    public InputActionReference previousAction;

    private bool isRotating = false;

    private void OnEnable()
    {
        // Suscribirse a los eventos 'performed' de cada acci�n
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
    /// Corrutina que rota la c�mara �nicamente en el eje Y, 
    /// preservando los valores originales de rotaci�n en X y Z.
    /// </summary>
    private IEnumerator RotateCamera(float deltaY)
    {
        isRotating = true;

        // Leemos la rotaci�n inicial (en Euler) antes de empezar
        Vector3 startEuler = cameraTransform.eulerAngles;

        // �ngulo inicial y final en el eje Y
        float startY = startEuler.y;
        float endY = startY + deltaY;

        float timeElapsed = 0f;

        while (timeElapsed < rotationDuration)
        {
            timeElapsed += Time.deltaTime;

            // Progreso de 0 a 1
            float t = timeElapsed / rotationDuration;

            // Factor de interpolaci�n seg�n la curva
            float curveValue = rotationCurve.Evaluate(t);

            // Interpolamos el �ngulo Y, pero conservamos X y Z originales
            float currentY = Mathf.Lerp(startY, endY, curveValue);

            cameraTransform.rotation = Quaternion.Euler(
                startEuler.x,  // X original
                currentY,      // Y interpolado
                startEuler.z   // Z original
            );

            yield return null;
        }

        // Ajustamos la rotaci�n al valor final exacto
        cameraTransform.rotation = Quaternion.Euler(
            startEuler.x,
            endY,
            startEuler.z
        );

        isRotating = false;
    }
}
