using UnityEngine;
using LibSM64;

public class CameraAndInput : SM64InputProvider
{
    [Header("Referencia a la cámara")]
    [Tooltip("Arrastra aquí tu Camera principal (el GameObject que contiene Camera).")]
    public GameObject cameraObject;

    [Header("Rotación con Mouse")]
    [Tooltip("Sensibilidad en eje X del mouse.")]
    public float mouseSensitivity = 2.0f;

    [Tooltip("Pitch fijo en grados (ángulo de inclinación hacia abajo).")]
    public float fixedPitch = 35f;

    private float yaw = 0f; // rotación acumulada en Y

    [Header("Zoom (ScrollWheel)")]
    [Tooltip("Distancia inicial de la cámara respecto a Mario.")]
    public float cameraDistance = 5f;

    [Tooltip("Velocidad de zoom (ScrollWheel).")]
    public float zoomSpeed = 2f;

    [Tooltip("Distancia mínima (zoom in).")]
    public float minZoom = 2f;

    [Tooltip("Distancia máxima (zoom out).")]
    public float maxZoom = 10f;

    [Header("Offset vertical para LookAt")]
    [Tooltip("Altura a la que la cámara mirará la cabeza de Mario y desde donde se lanza el raycast.")]
    public float lookAtHeight = 1.5f;

    [Header("Evitar paredes")]
    [Tooltip("Radio del spherecast usado para detectar colisión de la cámara contra obstáculos.")]
    public float collisionSphereRadius = 0.2f;

    [Tooltip("Pequeño offset para que la cámara no se quede pegada justo en el punto de impacto.")]
    public float wallOffset = 0.1f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (cameraObject == null)
            Debug.LogError("[CameraAndInput] Debes arrastrar tu cámara principal al campo cameraObject.");
    }

    void Update()
    {
        if (cameraObject == null) return;

        // 1) Rotación horizontal con el mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        yaw += mouseX;

        // 2) Zoom con ScrollWheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cameraDistance -= scroll * zoomSpeed;
        cameraDistance = Mathf.Clamp(cameraDistance, minZoom, maxZoom);

        // 3) Aplicamos rotación: pitch fijo en X, yaw variable en Y
        cameraObject.transform.rotation = Quaternion.Euler(fixedPitch, yaw, 0f);

        // 4) Calculamos posición deseada DETRÁS de Mario (sin colisión)
        Vector3 marioPos = transform.position;
        Vector3 forward = cameraObject.transform.forward;
        Vector3 idealCamPos = marioPos - forward * cameraDistance;
        idealCamPos.y = marioPos.y + lookAtHeight;

        // 5) Raycast/SphereCast para evitar que la cámara quede tras paredes
        Vector3 rayOrigin = new Vector3(marioPos.x, marioPos.y + lookAtHeight, marioPos.z);
        Vector3 dir = (idealCamPos - rayOrigin).normalized;
        float dist = Vector3.Distance(rayOrigin, idealCamPos);

        RaycastHit hit;
        // Usamos SphereCast para darle un poco de grosor a la cámara
        if (Physics.SphereCast(rayOrigin, collisionSphereRadius, dir, out hit, dist))
        {
            // Hay un muro entre Mario y la posición deseada de la cámara
            // Ajustamos la posición para que quede un poco antes del muro
            Vector3 hitPoint = hit.point;
            Vector3 adjustedPos = hitPoint - dir * wallOffset;
            cameraObject.transform.position = adjustedPos;
        }
        else
        {
            // No hay muro en el camino, la cámara va a la posición ideal
            cameraObject.transform.position = idealCamPos;
        }

        // 6) La cámara ya está ubicada. No necesitamos LookAt(),
        //    porque rotación ya viene de Quaternion.Euler(fixedPitch, yaw, 0)
        //    y el forward apunta hacia la posición de Mario (aprox).
    }

    public override Vector3 GetCameraLookDirection()
    {
        return cameraObject != null
            ? cameraObject.transform.forward
            : Vector3.forward;
    }

    public override Vector2 GetJoystickAxes()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public override bool GetButtonHeld(Button button)
    {
        switch (button)
        {
            case Button.Jump:
                return Input.GetKey(KeyCode.Space);
            case Button.Kick:
                return Input.GetMouseButton(0);
            case Button.Stomp:
                return Input.GetKey(KeyCode.LeftShift);
        }
        return false;
    }
}
