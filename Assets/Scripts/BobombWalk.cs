using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BobombWalk : MonoBehaviour
{
    [Header("Referencias de partes")]
    [Tooltip("Transform raíz de la malla de Bob-omb (para el bobbing vertical).")]
    public Transform bodyTransform;
    [Tooltip("Pie izquierdo separado.")]
    public Transform leftFoot;
    [Tooltip("Pie derecho separado.")]
    public Transform rightFoot;
    [Tooltip("Cuerda (para oscilarla ligeramente al paso).")]
    public Transform fuse;

    [Header("Parámetros de paso")]
    [Tooltip("Velocidad mínima para comenzar a hacer pasos.")]
    public float minStepSpeed = 0.1f;
    [Tooltip("Frecuencia de pasos (pasos por segundo cuando velocidad = 1).")]
    public float baseStepFrequency = 2f;
    [Tooltip("Amplitud en grados del swing de los pies.")]
    public float stepAngleAmplitude = 30f;

    [Header("Bobbing & Fuse Sway")]
    [Tooltip("Altura máxima de bobbing (unidades).")]
    public float bobbingHeight = 0.1f;
    [Tooltip("Frecuencia de bobbing (veces por segundo).")]
    public float bobbingFrequency = 2f;
    [Tooltip("Ángulo máximo de oscilación de la fuse.")]
    public float fuseSwayAngle = 10f;

    private Rigidbody rb;
    private float stepTimer = 0f;
    private Vector3 bodyStartPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (bodyTransform == null) bodyTransform = transform;
        bodyStartPos = bodyTransform.localPosition;
    }

    void Update()
    {
        // Magnitud horizontal de velocidad
        Vector3 vel = rb.velocity;
        float speed = new Vector2(vel.x, vel.z).magnitude;

        if (speed > minStepSpeed)
        {
            // Avanzar el timer proporcional a la velocidad
            float freq = baseStepFrequency * speed;
            stepTimer += Time.deltaTime * freq * Mathf.PI * 2f; // en radianes

            // Swing pies
            float angle = Mathf.Sin(stepTimer) * stepAngleAmplitude;
            leftFoot.localRotation  = Quaternion.Euler(angle, 0f, 0f);
            rightFoot.localRotation = Quaternion.Euler(-angle, 0f, 0f);

            // Bobbing del cuerpo
            float bob = Mathf.Sin(stepTimer * bobbingFrequency / baseStepFrequency) * bobbingHeight;
            bodyTransform.localPosition = bodyStartPos + Vector3.up * bob;

            // Oscilación de la fuse
            float sway = Mathf.Sin(stepTimer * 2f) * fuseSwayAngle;
            if (fuse != null)
                fuse.localRotation = Quaternion.Euler(0f, 0f, sway);
        }
        else
        {
            // Reset poses suavemente
            stepTimer = 0f;
            leftFoot.localRotation  = Quaternion.Slerp(leftFoot.localRotation,  Quaternion.identity, Time.deltaTime * 5f);
            rightFoot.localRotation = Quaternion.Slerp(rightFoot.localRotation, Quaternion.identity, Time.deltaTime * 5f);
            bodyTransform.localPosition = Vector3.Lerp(bodyTransform.localPosition, bodyStartPos, Time.deltaTime * 5f);
            if (fuse != null)
                fuse.localRotation = Quaternion.Slerp(fuse.localRotation, Quaternion.identity, Time.deltaTime * 5f);
        }
    }
}
