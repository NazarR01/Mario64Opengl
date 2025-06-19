using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameover : MonoBehaviour
{
    public GameObject panel;            // Panel de Game Over
    public float restartDelay = 3f;     // Tiempo antes de reiniciar

    private bool isGameOver = false;

    void Start()
    {
        panel.SetActive(false); // Ocultar al comenzar
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        panel.SetActive(true);  // Mostrar el panel
        Invoke("RestartLevel", restartDelay); // Esperar y reiniciar
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
