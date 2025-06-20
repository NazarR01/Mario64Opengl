using UnityEngine;

public class MarioController : MonoBehaviour
{
    public AudioSource audioSource;      // Asigna en Inspector el AudioSource con el clip
    public AudioClip sonidoQuemado;      // Asigna en Inspector el clip de quemadura

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lava"))
        {
            ReproducirSonidoQuemado();
        }
    }

    private void ReproducirSonidoQuemado()
    {
        if (audioSource != null && sonidoQuemado != null)
        {
            audioSource.PlayOneShot(sonidoQuemado);
        }
    }
}
