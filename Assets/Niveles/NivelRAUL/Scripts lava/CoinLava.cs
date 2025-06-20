using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibSM64;

[RequireComponent(typeof(Collider))]
public class CoinLava : MonoBehaviour
{
    [Tooltip("ID de Mario en libsm64")]
    public uint marioId;

    [Tooltip("Tag que debe tener el jugador (Mario)")]
    public string playerTag = "Player";

    [Header("Animación de rotación")]
    public float velocidadRotacion = 90f; // grados por segundo

    [Header("Audio")]
    public AudioClip coinSound; // sonido al recoger moneda
    [Range(0f, 1f)] public float coinSoundVolume = 1f;

    [Header("Estrella")]
    public GameObject estrellaExistente;  // Objeto de la estrella en la escena
    public float alturaExtra = 1.5f;

    private static int coinCount = 0; // contador global por escena
    private static bool estrellaAparecida = false;

    void Update()
    {
        // Gira la moneda en el eje Y
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // Reproduce el sonido si está asignado
            if (coinSound != null)
                AudioSource.PlayClipAtPoint(coinSound, transform.position, coinSoundVolume);

            coinCount++;
            Debug.Log($"¡Moneda recogida! Total de monedas: {coinCount}");

            if (coinCount >= 14 && !estrellaAparecida && estrellaExistente != null)
            {
                estrellaAparecida = true;
                Vector3 posicionBase = transform.position;
                Vector3 posicionFinal = posicionBase + Vector3.up * alturaExtra;

                estrellaExistente.transform.position = posicionBase;
                estrellaExistente.SetActive(true);

                Debug.Log("🌟 ¡Estrella desbloqueada con 14 monedas!");
                // Opcional: iniciar animación
                estrellaExistente.GetComponent<MonoBehaviour>().StartCoroutine(AnimarEstrella(estrellaExistente.transform, posicionFinal));
            }

            Destroy(gameObject); // eliminar la moneda recogida
        }
    }

    IEnumerator AnimarEstrella(Transform estrella, Vector3 destino)
    {
        float duracion = 3.0f;
        float tiempo = 0f;
        Vector3 inicio = estrella.position;

        while (tiempo < duracion)
        {
            estrella.position = Vector3.Lerp(inicio, destino, tiempo / duracion);
            estrella.Rotate(Vector3.up * 180f * Time.deltaTime, Space.World);
            tiempo += Time.deltaTime;
            yield return null;
        }

        estrella.position = destino;
    }

    public static int GetCoinCount()
{
    return coinCount;
}

public static void ResetCoinCount()
{
    coinCount = 0;
    estrellaAparecida = false;
    Debug.Log("🔁 Monedas reseteadas por muerte de Mario.");
}

}
