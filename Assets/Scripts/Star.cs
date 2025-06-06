using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibSM64;

public class Star : MonoBehaviour
{
    [Tooltip("ID de Mario en libsm64")]
    public uint marioId;

    // Constante para la acción agarrar estrella
    private const int ACT_STAR_DANCE_EXIT = 0x00001302;

    // El tag que debe tener el objeto estrella para detectar la colisión
    public string starTag = "Star";

    private bool starGrabbed = false;

    private void OnTriggerEnter(Collider other)
    {
        // Asegurarnos de que choca con Mario (tag “Player” o el que uses)
        if (!starGrabbed && other.CompareTag("Player"))
        {
            starGrabbed = true; // para que no se repita la acción

            Debug.Log("Mario ha agarrado la estrella!");

            // 1) Decirle a SM64 que ejecute la acción interna de “star dance”
            Interop.sm64_mario_set_cutscene_action((int)marioId, ACT_STAR_DANCE_EXIT, 0);

            // 2) Buscar el componente CameraAndInput para lanzar la cutscene
            var camScript = FindObjectOfType<CameraAndInput>();
            if (camScript != null)
            {
                camScript.PlayStarCutscene();
            }
            else
            {
                Debug.LogWarning("[Star] No se encontró ningún CameraAndInput en la escena.");
            }

            // 3) Opcional: Desactivar o destruir la estrella en el mundo
            //gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
}