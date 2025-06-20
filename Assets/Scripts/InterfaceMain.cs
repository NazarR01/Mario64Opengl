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

        if (textoMonedas != null)
        {
            textoMonedas.text = $"Coins: {CoinLava.GetCoinCount()}";
        }

        if (tiempoRestante <= 0 && !tiempoAgotado)
        {
            tiempoAgotado = true;

            if (marioHealt != null)
            {
                marioHealt.SetHealthZero(); // Mata a Mario y muestra Game Over
                Debug.Log("💀 Tiempo agotado. Salud de Mario forzada a 0.");
            }
            else
            {
                Debug.LogWarning("❗ No se pudo matar a Mario: referencia a MarioHealt faltante.");
            }
        }
    }
}
