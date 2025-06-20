using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LibSM64;

public class InterfaceMain : MonoBehaviour
{
    public float tiempoInicial = 120f;
    private float tiempoRestante;

    public Text textoCronometro;
    public Text textoMonedas;
    public GameObject mario;
    public MarioHealt marioHealt;

    private MarioCollisionDetector marioDetector;
    private bool tiempoAgotado = false;

    void Start()
    {
        Coin.ResetCoinCount(); // 🔁 Reinicia monedas al cargar la escena
        tiempoRestante = tiempoInicial;

        if (mario != null)
            marioDetector = mario.GetComponent<MarioCollisionDetector>();
        else
            Debug.LogWarning("❗ Mario no está asignado en el Inspector.");

        if (marioHealt == null)
            Debug.LogWarning("❗ MarioHealt no está asignado en el Inspector.");
    }

    void Update()
    {
        if (tiempoAgotado) return;

        // Cronómetro
        tiempoRestante -= Time.deltaTime;
        tiempoRestante = Mathf.Max(0, tiempoRestante);

        if (textoCronometro != null)
        {
            int segundos = Mathf.CeilToInt(tiempoRestante);
            textoCronometro.text = $"Time: {segundos:00}";
        }
        else
        {
            Debug.LogWarning("❗ textoCronometro no está asignado.");
        }

        // Mostrar monedas recogidas
        if (textoMonedas != null)
        {
            textoMonedas.text = $"Coins: {Coin.GetCoinCount()}";
        }

        // Matar a Mario si el tiempo llega a 0
        if (tiempoRestante <= 0 && !tiempoAgotado)
        {
            tiempoAgotado = true;

            if (marioHealt != null)
            {
                marioHealt.SetHealthZero(); // ✅ Matar a Mario
                Debug.Log("💀 Tiempo agotado. Salud de Mario forzada a 0.");
            }
            else
            {
                Debug.LogWarning("❗ No se pudo matar a Mario: referencia a MarioHealt faltante.");
            }
        }
    }

    // 🔁 Permite reiniciar el cronómetro desde Game Over si se desea
    public void ResetTimer()
    {
        tiempoRestante = tiempoInicial;
        tiempoAgotado = false;
    }
}
