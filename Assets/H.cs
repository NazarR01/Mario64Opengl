using UnityEngine;

public class H : MonoBehaviour
{
    public Transform mario;
    public Vector3 offset = new Vector3(0, 2f, 0);

    void LateUpdate()
    {
        if (mario != null)
        {
            transform.position = mario.position + offset;
            transform.LookAt(Camera.main.transform); // Siempre hacia la cámara
        }
    }
}
