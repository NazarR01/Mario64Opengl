using UnityEngine;
using UnityEngine.UI;

public class MarioHealt : MonoBehaviour
{
    [SerializeField] private LibSM64.SM64Mario mario=null; // Referencia a Mario
    [SerializeField] private Slider healthBar=null;        // Slider de vida
    [SerializeField] private GameObject gameOverPanel=null; // Panel de Game Over

    private const float MAX_HEALTH = 0x0880;// Salud máxima (hexadecimal 0x0880 = decimal 2176)
    private bool gameOverShown = false;
private const float MIN_HEALTH = 0x000;
    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); // Asegura que el panel esté oculto al inicio
    }

    void Update()
    {
        if (mario == null || healthBar == null)
            return;

        float currentHealth = mario.CurrentState.health;
        float healthRatio = Mathf.Clamp01(currentHealth / MAX_HEALTH);
        healthBar.value = healthRatio;

        if (healthRatio == 0.1171875)
        {   
            ShowGameOver();
	   
FindObjectOfType<Gameover>().TriggerGameOver();

        }
    }

    void ShowGameOver()
    {
        gameOverShown = true;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}
