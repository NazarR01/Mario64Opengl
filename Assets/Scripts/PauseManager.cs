using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public RectTransform[] menuOptions;         // Opciones de menú (botones)
    public RectTransform cursorDot;             // Punto como cursor
    public Vector2 offset = new Vector2(-60f, -5f); // Posición relativa del cursor

     public AudioClip pauseAudioClip;
    [Range(0f,1f)] public float pauseAudioVolume = 1f;



    private CameraAndInput cameraControl;
    private int selectedIndex = 0;
    private bool isPaused = false;
    private float moveDelay = 0.2f;
    private float lastMoveTime = 0f;
    [SerializeField] private Gameover gameoverScript;
     [SerializeField] private Star Starscript;
    [SerializeField] private GameObject Heart;
[SerializeField] private GameObject monedas;
[SerializeField] private GameObject textotimer;
    void Start()
    {
        pauseMenuPanel.SetActive(false);
        MoveCursorToSelected();

        cameraControl = FindObjectOfType<CameraAndInput>();
        if (cameraControl == null)
            Debug.LogWarning("No se encontró el script CameraAndInput en la escena.");
    }

    void Update()
    {
           if (gameoverScript != null && gameoverScript.IsGameOver)
        return;
          if (Starscript.StarGrabbed) return; // Si ganaste, no permitas pausar

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (!isPaused) return;

        if (Time.unscaledTime - lastMoveTime > moveDelay)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedIndex = (selectedIndex - 1 + menuOptions.Length) % menuOptions.Length;
                MoveCursorToSelected();
                lastMoveTime = Time.unscaledTime;
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedIndex = (selectedIndex + 1) % menuOptions.Length;
                MoveCursorToSelected();
                lastMoveTime = Time.unscaledTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            Button btn = menuOptions[selectedIndex].GetComponent<Button>();
            if (btn != null)
                btn.onClick.Invoke();
        }
    }

    void PauseGame()
    {

        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
        selectedIndex = 0;
        if (Heart != null) Heart.SetActive(false);
        if (monedas != null) monedas.SetActive(false);
        if (textotimer != null) textotimer.SetActive(false);
        MoveCursorToSelected();
        if (pauseAudioClip != null)
            AudioSource.PlayClipAtPoint(pauseAudioClip, transform.position, pauseAudioVolume);


        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (cameraControl != null)
            cameraControl.cameraPaused = true;
    }



    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);


    if (Heart != null) Heart.SetActive(true);
    if (monedas != null) monedas.SetActive(true);
    if (textotimer != null) textotimer.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (cameraControl != null)
            cameraControl.cameraPaused = false;
    }

    void MoveCursorToSelected()
    {
        if (cursorDot != null && selectedIndex < menuOptions.Length)
        {
            RectTransform selected = menuOptions[selectedIndex];
            cursorDot.position = selected.position + (Vector3)offset;
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
