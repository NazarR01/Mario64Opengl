using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using LibSM64;

public class MovimientoEstrella : MonoBehaviour
{
    [Tooltip("ID de Mario en libsm64")]
    public uint marioId;

    private const int ACT_STAR_DANCE_EXIT = 0x00001302;

    public float velocidadRotacion = 90f;

    private bool starGrabbed = false;

    private void Update()
    {
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!starGrabbed && other.CompareTag("Player"))
        {
            starGrabbed = true;
            Debug.Log("Mario ha agarrado la estrella!");

            Interop.sm64_mario_set_cutscene_action((int)marioId, ACT_STAR_DANCE_EXIT, 0);

            var camScript = FindObjectOfType<CameraAndInput>();
            if (camScript != null)
            {
                camScript.PlayStarCutscene();
            }
            else
            {
                Debug.LogWarning("[Star] No se encontró ningún CameraAndInput en la escena.");
            }

            Destroy(gameObject);

            StartCoroutine(TerminarNivelDespuesDeDelay(3f));
        }
    }

    private IEnumerator TerminarNivelDespuesDeDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("VictoryScene"); // Cambia "VictoryScene" por el nombre de tu escena final
    }
}
