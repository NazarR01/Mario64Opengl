using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LibSM64;

public class interfacesurvival : MonoBehaviour
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
            Debug.LogWarning("TextoCronometro no está asignado en el Inspector.");
        }

        // Mostrar monedas recogidas
        if (textoMonedas != null)
        {
            textoMonedas.text = $"Coins: {Coin.GetCoinCount()}";
        }

        // Matar a Mario si el tiempo llega a 0
        if (tiempoRestante <= 0)
        {
            tiempoAgotado = true;
            //aqui se agregara pantalla de victoria
            




        
        }
    }
}
