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
        // Verificamos que la colisión sea con el objeto estrella
        if (!starGrabbed && other.CompareTag("Player"))
        {
            starGrabbed = true; // para que no se repita la acción varias veces

            Debug.Log("Mario ha agarrado la estrella!");

            // Llama a la función para activar la animación de agarrar estrella
            Interop.sm64_mario_set_cutscene_action((int)marioId, ACT_STAR_DANCE_EXIT, 0);

            // Opcional: Desactiva o destruye la estrella
            //other.gameObject.SetActive(false);
            //Destroy(other.gameObject);
        }
    }
}

