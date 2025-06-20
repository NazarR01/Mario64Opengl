using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MenuP : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";
    public float fadeDuration = 1.5f;
    public GameObject fadePanel;

    [Header("Panel de controles")]
    [SerializeField] private GameObject controlesPanel; // Panel de controles
    [SerializeField] private GameObject menuPrincipal;  // Panel u objeto con los botones
    [SerializeField] private MenuC menuControlScript;   // Script del control del menú (si es un componente aparte)

    void Start()
    {
        if (controlesPanel != null)
            controlesPanel.SetActive(false); // Ocultar al inicio
    }

    void Update()
    {
        // Si el panel está activo y se presiona Enter
        if (controlesPanel != null && controlesPanel.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            OcultarControles();
        }
    }

    public void StartGame()
    {
        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
            var img = fadePanel.GetComponent<Image>();
            img.color = new Color(0, 0, 0, 0); // Transparente al inicio
        }

        StartCoroutine(FadeAndLoadScene());
    }

    IEnumerator FadeAndLoadScene()
    {
        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
            Image fadeImage = fadePanel.GetComponent<Image>();
            Color color = fadeImage.color;

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                fadeImage.color = new Color(color.r, color.g, color.b, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }

            fadeImage.color = new Color(color.r, color.g, color.b, 1f);
        }

        if (!string.IsNullOrEmpty(gameSceneName))
            SceneManager.LoadScene(gameSceneName);
    }

    public void ExitGame()
    {
        Debug.Log("Salir del juego");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Mostrar el panel de controles
    public void MostrarControles()
    {
        if (controlesPanel != null)
            controlesPanel.SetActive(true);

        if (menuPrincipal != null)
            menuPrincipal.SetActive(false);

        if (menuControlScript != null)
            menuControlScript.enabled = false;
    }

    // Ocultar el panel de controles
    public void OcultarControles()
    {
        if (controlesPanel != null)
            controlesPanel.SetActive(false);

        if (menuPrincipal != null)
            menuPrincipal.SetActive(true);

        if (menuControlScript != null)
            menuControlScript.enabled = true;
    }
}
