using UnityEngine;
using LibSM64;

public class input : SM64InputProvider
{
    public GameObject cameraObject;


    public float mouseSensitivity = 2.0f;
    private float yaw = 0f;
    private float pitch = 35f;

    public float cameraDistance = 5f;
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Rotación de cámara con el mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        //float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        yaw += mouseX;
       // pitch -= mouseY;
       // pitch = Mathf.Clamp(pitch, -80f, 80f);

        // Zoom con scroll
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cameraDistance -= scroll * zoomSpeed;
        cameraDistance = Mathf.Clamp(cameraDistance, minZoom, maxZoom);




        cameraObject.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Posición de la cámara detrás de Mario (esto supone que el script está en el mismo GameObject que el MarioController)
        Vector3 marioPos = transform.position;
        Vector3 desiredCamPos = marioPos - cameraObject.transform.forward * cameraDistance;
        cameraObject.transform.position = desiredCamPos;
        cameraObject.transform.LookAt(marioPos + Vector3.up * 1.5f);

    }
    public override Vector3 GetCameraLookDirection()
    {
        return cameraObject.transform.forward;
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
                return Input.GetMouseButton(0); // clic izquierdo
            case Button.Stomp:
                return Input.GetKey(KeyCode.LeftShift);

        }
        return false;
    }
}