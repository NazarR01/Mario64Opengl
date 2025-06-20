using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LibSM64;
using UnityEngine.SceneManagement;


public class InterfaceLava : MonoBehaviour
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
        CoinLava.ResetCoinCount(); // Resetear monedas al iniciar escena
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
            textoMonedas.text = $"Coins: {CoinLava.GetCoinCount()}";
        }

        // Matar a Mario si el tiempo llega a 0
        if (tiempoRestante <= 0 && marioDetector != null)
        {
            tiempoAgotado = true;

            // FORZAMOS muerte de Mario
            Interop.sm64_mario_set_health(0, 0x0000);

            // Apagar visualmente a Mario (por si Interop no lo mata)
            if (mario != null)
                mario.SetActive(false);

            // 🔁 Reiniciar escena tras breve delay
            StartCoroutine(ReiniciarEscenaConRetraso());
        }
    }

    IEnumerator ReiniciarEscenaConRetraso()
    {
        CoinLava.ResetCoinCount(); // Asegura reinicio del contador
        yield return new WaitForSeconds(0.1f); // Delay para que se procese todo
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}