using UnityEngine;
using UnityEngine.UI;
using LibSM64;

public class MarioHealt : MonoBehaviour
{
    [SerializeField] private LibSM64.SM64Mario mario = null;     // Referencia a Mario
    [SerializeField] private Image[] hearts;                     // Corazones UI
    [SerializeField] private Sprite fullHeart;                   // Sprite corazón lleno
    [SerializeField] private Sprite emptyHeart;                  // Sprite corazón vacío
    [SerializeField] private GameObject gameOverPanel = null;    // Panel Game Over
    [SerializeField] private Gameover gameoverScript;            // Script para manejar Game Over

    private const int MAX_HEALTH = 0x0880;  // Salud máxima (2176)
    private int lastHealth = -1;
    private int fullHearts = -1;
    private bool gameOverShown = false;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (mario == null)
            return;

        int rawHealth = LibSM64.Interop.MarioGetHealth(0);

        if (rawHealth != lastHealth)
        {
            lastHealth = rawHealth;
            UpdateHearts(rawHealth);

            fullHearts = GetHeartCount(rawHealth);

            if (!gameOverShown && fullHearts == 0)
            {
                gameOverShown = true;
                Debug.Log("Game Over: Vida en cero");
                gameoverScript?.TriggerGameOver();
            }
        }
    }

    // Actualiza sprites de corazones
    void UpdateHearts(int rawHealth)
    {
        fullHearts = GetHeartCount(rawHealth);

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].sprite = (i < fullHearts) ? fullHeart : emptyHeart;
        }
    }

    // Calcula cuántos corazones completos hay
    int GetHeartCount(int rawHealth)
    {
        int healthPerHeart = MAX_HEALTH / 3;
        return Mathf.Clamp(rawHealth / healthPerHeart, 0, 3);
    }

    // Método para forzar salud a cero y mostrar Game Over
    public void SetHealthZero()
    {
        Interop.sm64_mario_set_health(0, 0x0000);
        fullHearts = 0;
        lastHealth = -1;  // Forzar actualización en Update

        int rawHealth = LibSM64.Interop.MarioGetHealth(0);
        UpdateHearts(rawHealth);

        if (!gameOverShown)
        {
            gameOverShown = true;
            Debug.Log("Game Over: Salud forzada a 0 desde SetHealthZero");
            gameoverScript?.TriggerGameOver();
        }
    }
}