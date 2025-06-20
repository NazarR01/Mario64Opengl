using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI; //
public class MenuP : MonoBehaviour
{
    // Nombre de la escena del juego (la que quieres cargar cuando empieza el juego)
    [SerializeField] private string gameSceneName = "GameScene";
 public float fadeDuration = 1.5f; 
     public GameObject fadePanel;   
     
    public void StartGame()
    {
        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
            var img = fadePanel.GetComponent<Image>();
            img.color = new Color(0, 0, 0, 0); // Transparente al inicio
        }
        // Carga la escena principal del juego
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

        // En el editor no cierra, por eso esto:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
