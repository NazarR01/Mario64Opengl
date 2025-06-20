using UnityEngine;

public class MovimientoEstrella : MonoBehaviour
{
    [Tooltip("Velocidad de rotación de la estrella")]
    public float velocidadRotacion = 90f;

    void Update()
    {
        // Hace rotar la estrella constantemente en el eje Y
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime, Space.World);
    }
}
