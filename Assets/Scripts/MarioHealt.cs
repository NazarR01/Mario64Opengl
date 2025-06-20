using UnityEngine;
using UnityEngine.UI;

public class MarioHealt : MonoBehaviour
{
   [SerializeField] private LibSM64.SM64Mario mario = null;
    [SerializeField] private Slider healthBar = null;
    [SerializeField] private GameObject gameOverPanel = null;
    [SerializeField] private Gameover gameoverScript;

    [Header("Audio")]
    [SerializeField] private AudioClip damageClip;      // 🔊 Sonido al recibir daño
    [SerializeField] private float damageVolume = 1f;

    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private float gameOverVolume = 1f;

    private AudioSource audioSource;

    private const int MAX_HEALTH = 0x0880;  // 2176 en decimal
    private const int MIN_HEALTH = 0x0000;
    private bool gameOverShown = false;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (mario == null || healthBar == null)
            return;

        int rawHealth = LibSM64.Interop.MarioGetHealth(0);
        float healthRatio = Mathf.Clamp01((float)rawHealth / MAX_HEALTH);
        healthBar.value = healthRatio;

        if (!gameOverShown && healthRatio <= 0.1171875f)
        {
            gameOverShown = true;

            if (gameOverClip != null)
                audioSource.PlayOneShot(gameOverClip, gameOverVolume);

            gameoverScript?.TriggerGameOver();
        }
    }

    // ✅ Método público para aplicar daño y reproducir sonido
    public void ApplyDamage(int amount)
    {
        LibSM64.Interop.sm64_mario_apply_damage(0, amount);

        if (damageClip != null && audioSource != null)
            audioSource.PlayOneShot(damageClip, damageVolume);
    }
}
