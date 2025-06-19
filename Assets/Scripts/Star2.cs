using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using LibSM64;

public class Star2 : MonoBehaviour
{
    [Tooltip("ID de Mario en libsm64")]
    public uint marioId;

    private const int ACT_STAR_DANCE_EXIT = 0x00001302; // Acción de animación al recoger estrella
    private const int ACT_IDLE = 0x0C000201; // Acción de estado IDLE de Mario

    [Tooltip("Tag que debe tener el jugador (Mario)")]
    public string playerTag = "Player";

    private bool starGrabbed = false;

    // Contador global de estrellas
    private static int estrellasRecogidas = 0;

    // Total de estrellas necesarias para finalizar el nivel
    public int estrellasNecesarias = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (!starGrabbed && other.CompareTag(playerTag))
        {
            starGrabbed = true;
            estrellasRecogidas++;

            Debug.Log($"Estrellas recogidas: {estrellasRecogidas}/{estrellasNecesarias}");

            // Ejecutar animación de estrella
            Interop.sm64_mario_set_cutscene_action((int)marioId, ACT_STAR_DANCE_EXIT, 0);

            // Destruir la estrella visualmente
            Destroy(gameObject);

            if (estrellasRecogidas >= estrellasNecesarias)
            {
                Debug.Log("¡Nivel completado!");
                StartCoroutine(FinalizarNivelTrasEspera(3f));
            }
            else
            {
                // Después de animación, volver a IDLE para permitir movimiento
                StartCoroutine(ReactivarControlTrasEspera(3f));
            }
        }
    }

    private IEnumerator ReactivarControlTrasEspera(float segundos)
    {
        yield return new WaitForSeconds(segundos);

        // Volver al estado IDLE para que Mario pueda moverse otra vez
        Interop.sm64_mario_set_action((int)marioId, ACT_IDLE, 0);
    }

    private IEnumerator FinalizarNivelTrasEspera(float segundos)
    {
        yield return new WaitForSeconds(segundos);

        // Cargar la siguiente escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
