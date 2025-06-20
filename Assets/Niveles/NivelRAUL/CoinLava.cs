using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinLava : MonoBehaviour
{
    public string playerTag = "Player";
    public float velocidadRotacion = 90f;

    private static int coinCount = 0;
    private static bool estrellaAparecida = false;

    [Header("Estrella")]
    public GameObject estrellaExistente; // Estrella ya colocada en la escena
    public float alturaExtra = 1.5f;

    [Header("Detección manual")]
    public float radioDeteccion = 1.0f;

    private Transform mario;

    void Start()
    {
        GameObject marioObj = GameObject.FindGameObjectWithTag(playerTag);
        if (marioObj != null)
            mario = marioObj.transform;
        else
            Debug.LogWarning("❗ Mario no encontrado. Asegúrate de que tenga el tag 'Player'");
    }

    void Update()
    {
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime, Space.World);

        if (mario != null && Vector3.Distance(transform.position, mario.position) < radioDeteccion)
        {
            coinCount++;
            Debug.Log($"✅ Moneda recogida. Total: {coinCount}");

            // Posición de esta moneda
            Vector3 posicionMoneda = transform.position;

            if (coinCount >= 14 && !estrellaAparecida)
            {
                estrellaAparecida = true;
                MoverYAnimarEstrella(posicionMoneda);
            }

            Destroy(gameObject);
        }
    }

    void MoverYAnimarEstrella(Vector3 posicionBase)
    {
        if (estrellaExistente != null)
        {
            Vector3 posicionFinal = posicionBase + Vector3.up * alturaExtra;

            estrellaExistente.transform.position = posicionBase;
            estrellaExistente.SetActive(true);

            StartCoroutine(AnimarEstrella(estrellaExistente.transform, posicionFinal));
            Debug.Log("🌟 ¡Estrella desbloqueada en la última moneda!");
        }
        else
        {
            Debug.LogWarning("⚠️ No se ha asignado el objeto de la estrella.");
        }
    }

    IEnumerator AnimarEstrella(Transform estrella, Vector3 destino)
    {
        float duracion = 3.0f; // ahora la estrella sube más lentamente
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
}
