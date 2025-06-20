using System.Collections;
using UnityEngine;
using LibSM64;

public class CameraR : SM64InputProvider
{
    public bool cameraPaused = false;

    [Header("Referencia a la cámara")]
    [Tooltip("Arrastra aquí tu cámara principal (el GameObject que contiene Camera).")]
    public GameObject cameraObject;

    [Header("Rotación con Mouse")]
    [Tooltip("Sensibilidad en eje X del mouse.")]
    public float mouseSensitivity = 2.0f;

    [Tooltip("Pitch fijo en grados (ángulo de inclinación hacia abajo).")]
    public float fixedPitch = 20f;

    private float yaw = 0f; // rotación acumulada en Y

    [Header("Zoom (ScrollWheel)")]
    [Tooltip("Distancia inicial de la cámara respecto a Mario.")]
    public float cameraDistance = 10f;

    [Tooltip("Velocidad de zoom (ScrollWheel).")]
    public float zoomSpeed = 2f;

    [Tooltip("Distancia mínima (zoom in).")]
    public float minZoom = 2f;

    [Tooltip("Distancia máxima (zoom out).")]
    public float maxZoom = 10f;

    [Header("Offset vertical para LookAt")]
    [Tooltip("Altura a la que la cámara mirará la cabeza de Mario.")]
    public float lookAtHeight = 7.5f;

    // --- Para el Star Cutscene ---
    private bool inStarCutscene = false;
    private Vector3 savedPosition;
    private Quaternion savedRotation;
    private float savedYaw;
    private float savedDistance;

    [Header("Star Cutscene Settings")]
    [Tooltip("Duración total de la cinemática (en segundos).")]
    public float starCutsceneDuration = 3.0f;

    [Tooltip("Radio extra alrededor de Mario para la cinemática.")]
    public float starCutsceneRadiusOffset = 2.5f;

    [Tooltip("Altura extra a la que subirá la cámara durante la cinemática.")]
    public float starCutsceneHeightOffset = 1.5f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraObject == null)
            Debug.LogError("[CameraAndInput] Debes arrastrar tu cámara principal al campo cameraObject.");
    }

    void Update()
{
    if (cameraObject == null || inStarCutscene || cameraPaused)
        return;

    // 1) Rotación horizontal con el mouse
    float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
    yaw += mouseX;

    // 2) Zoom con ScrollWheel
    float scroll = Input.GetAxis("Mouse ScrollWheel");
    cameraDistance = Mathf.Clamp(cameraDistance - scroll * zoomSpeed, minZoom, maxZoom);

    // 3) Aplicamos rotación
    cameraObject.transform.rotation = Quaternion.Euler(fixedPitch, yaw, 0f);

    // 4) Posición ideal
    Vector3 marioPos = transform.position;
    Vector3 forward  = cameraObject.transform.forward;
    Vector3 idealCamPos = marioPos - forward * cameraDistance;
    idealCamPos.y = marioPos.y + lookAtHeight;

    // 5) Sin detección de colisiones
    cameraObject.transform.position = idealCamPos;
}


    // Métodos de SM64InputProvider
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

    /// <summary>
    /// Llama esto desde otro script (ej. Star.cs) para lanzar la cinemática de Star.
    /// </summary>
    public void PlayStarCutscene()
    {
        if (!inStarCutscene)
        {
            StartCoroutine(StarCutsceneRoutine());
        }
    }

    /// <summary>
    /// Corrutina que desplaza la cámara en un arco circular/elevado alrededor de Mario
    /// durante starCutsceneDuration segundos y luego restaura pos/rotación original.
    /// </summary>
    private IEnumerator StarCutsceneRoutine()
    {
        inStarCutscene = true;

        // 1) Guardar valores actuales
        savedPosition = cameraObject.transform.position;
        savedRotation = cameraObject.transform.rotation;
        savedYaw      = yaw;
        savedDistance = cameraDistance;

        Vector3 marioStart = transform.position; // posición de Mario al inicio

        float halfTime = starCutsceneDuration / 2f;
        float elapsed = 0f;

        // 2) Durante los primeros halfTime segundos, elevamos la cámara y damos media vuelta
        while (elapsed < halfTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfTime; // de 0 a 1

            // Calcular un ángulo de rotación: 0 → 180° sobre Y
            float angle = Mathf.Lerp(0f, 180f, t);
            float pitch = Mathf.Lerp(fixedPitch, fixedPitch + starCutsceneHeightOffset, t);
            float radius = savedDistance + starCutsceneRadiusOffset;

            // Posición de la cámara en coordenadas polares respecto a Mario
            float rad = Mathf.Deg2Rad * (savedYaw + angle);
            Vector3 offset = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)) * radius;
            Vector3 pos = marioStart + offset;
            pos.y = Mathf.Lerp(marioStart.y + lookAtHeight, marioStart.y + lookAtHeight + starCutsceneHeightOffset, t);

            // Rotación: apuntar hacia Mario
            Quaternion rot = Quaternion.LookRotation((marioStart + Vector3.up * lookAtHeight) - pos, Vector3.up);

            cameraObject.transform.position = pos;
            cameraObject.transform.rotation = rot;

            yield return null;
        }

        // 3) Durante los segundos restantes (halfTime), volvemos a la posición original
        float afterTime = 0f;
        while (afterTime < halfTime)
        {
            afterTime += Time.deltaTime;
            float t2 = afterTime / halfTime; // de 0 a 1

            // Interpolamos de la posición actual → savedPosition
            cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position, savedPosition, t2);
            cameraObject.transform.rotation = Quaternion.Slerp(cameraObject.transform.rotation, savedRotation, t2);

            yield return null;
        }

        // 4) Restaurar valores exactos
        cameraObject.transform.position = savedPosition;
        cameraObject.transform.rotation = savedRotation;
        yaw = savedYaw;
        cameraDistance = savedDistance;

        inStarCutscene = false;
    }
}
