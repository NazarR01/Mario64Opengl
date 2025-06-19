using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LibSM64;

public class Interface : MonoBehaviour
{
    public float tiempoInicial = 120f;
    private float tiempoRestante;

    public Text textoCronometro;
    public Text textoMonedas; 
    public GameObject mario;

    private MarioCollisionDetector marioDetector;
    private bool tiempoAgotado = false;

    void Start()
    {
        tiempoRestante = tiempoInicial;

        if (mario != null)
            marioDetector = mario.GetComponent<MarioCollisionDetector>();
        else
            Debug.LogWarning("Mario no está asignado en el Inspector.");
    }

    void Update()
    {
        if (tiempoAgotado) return;

        // Cronómetro
        tiempoRestante -= Time.deltaTime;
        tiempoRestante = Mathf.Max(0, tiempoRestante);

        if (textoCronometro != null)
        {
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            textoCronometro.text = $"Time: {segundos:00}";
        }
        else
        {
            Debug.LogWarning("TextoCronometro no está asignado en el Inspector.");
        }

        // Mostrar monedas recogidas
        if (textoMonedas != null)
        {
            textoMonedas.text = $"Coins: {Coin.GetCoinCount()}";
        }

        // Matar a Mario si el tiempo llega a 0
        if (tiempoRestante <= 0 && marioDetector != null)
        {
            tiempoAgotado = true;
            Interop.sm64_mario_set_health(0, 0x0000);
        }
    }
}
