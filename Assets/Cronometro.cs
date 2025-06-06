using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LibSM64;

public class Cronometro : MonoBehaviour
{
    public float tiempoInicial = 120f; // Tiempo en segundos
    private float tiempoRestante;

    public Text textoCronometro;
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

        tiempoRestante -= Time.deltaTime;
        tiempoRestante = Mathf.Max(0, tiempoRestante);

        // Mostrar el tiempo en formato MM:SS
        if (textoCronometro != null)
        {
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            textoCronometro.text = Mathf.CeilToInt(tiempoRestante).ToString();
        }
        else
        {
            Debug.LogWarning("TextoCronometro no está asignado en el Inspector.");
        }

        // Matar a Mario si el tiempo llega a 0
        if (tiempoRestante <= 0 && marioDetector != null)
        {
            tiempoAgotado = true;
            Interop.sm64_mario_set_health(0,0x0000);
            //marioDetector.MatarInstantaneamente();
        }
    }
}
