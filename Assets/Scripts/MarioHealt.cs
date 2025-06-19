using UnityEngine;
using UnityEngine.UI;

public class MarioHealt : MonoBehaviour
{
    [SerializeField] private LibSM64.SM64Mario mario = null; // Referencia al objeto de Mario
    [SerializeField] private Slider healthBar = null;         // Barra de salud (UI Slider)
    [SerializeField] private GameObject gameOverPanel = null; // Panel de Game Over

    private const int MAX_HEALTH = 0x0880;  // 2176 en decimal
    private const int MIN_HEALTH = 0x0000;
    private bool gameOverShown = false;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); // Asegura que esté oculto al inicio
    }

    void Update()
    {
        if (mario == null || healthBar == null)
            return;

        int rawHealth = LibSM64.Interop.MarioGetHealth(0);
        float healthRatio = Mathf.Clamp01((float)rawHealth / MAX_HEALTH);
        healthBar.value = healthRatio;

        // El valor 0.1171875 representa 1 segmento de 8, que es el mínimo antes de "morir"
        if (!gameOverShown && healthRatio <=0.1171875)
        {
            ShowGameOver();
            FindObjectOfType<Gameover>()?.TriggerGameOver();
        }
    }

    void ShowGameOver()
    {
        gameOverShown = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}
