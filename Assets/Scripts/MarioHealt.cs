using UnityEngine;
using UnityEngine.UI;

public class MarioHealt : MonoBehaviour
{
    [SerializeField] private LibSM64.SM64Mario mario = null;     // Referencia a Mario
    [SerializeField] private Image[] hearts;                     // 3 imágenes de corazones
    [SerializeField] private Sprite fullHeart;                   // Sprite de corazón lleno
    [SerializeField] private Sprite emptyHeart;                  // Sprite de corazón vacío
    [SerializeField] private GameObject gameOverPanel = null;    // Panel de Game Over
    [SerializeField] private Gameover gameoverScript;            // Script de Game Over

    private const int MAX_HEALTH = 0x0880;  // 2176
    private int lastHealth = -1;
    private bool gameOverShown = false;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); // Asegura que esté oculto al inicio
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

            int fullHearts = GetHeartCount(rawHealth);
            if (!gameOverShown && fullHearts == 0)
            {
                gameOverShown = true;
                Debug.Log("Game Over: Vida en cero");
                gameoverScript?.TriggerGameOver();
            }
        }
    }

    void UpdateHearts(int rawHealth)
    {
        int fullHearts = GetHeartCount(rawHealth);

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].sprite = i < fullHearts ? fullHeart : emptyHeart;
        }
    }

    int GetHeartCount(int rawHealth)
    {
        int healthPerHeart = MAX_HEALTH / 3;
        return Mathf.Clamp(rawHealth / healthPerHeart, 0, 3);
    }
}
