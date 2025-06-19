using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibSM64;

[RequireComponent(typeof(Collider))]
public class Coin : MonoBehaviour
{
   [Tooltip("ID de Mario en libsm64")]
    public uint marioId;

    [Tooltip("Tag que debe tener el jugador (Mario)")]
    public string playerTag = "Player";

    [Header("Animación de rotación")]
    public float velocidadRotacion = 90f; // grados por segundo

    private static int coinCount = 0; // contador global por escena

    void Update()
    {
        // Gira la moneda en el eje Y
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            coinCount++;
            Debug.Log($"¡Moneda recogida! Total de monedas: {coinCount}");

            Destroy(gameObject); // eliminar la moneda recogida
        }
    }

    public static int GetCoinCount()
    {
    return coinCount;
    }

}
