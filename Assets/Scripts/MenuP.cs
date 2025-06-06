using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuP : MonoBehaviour
{
    // Nombre de la escena del juego (la que quieres cargar cuando empieza el juego)
    [SerializeField] private string gameSceneName = "GameScene";

    public void StartGame()
    {
        // Carga la escena principal del juego
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
