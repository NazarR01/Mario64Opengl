using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Gameover : MonoBehaviour
{
    public GameObject panel;                         // Panel de Game Over
    public RectTransform[] menuOptions;              // Opciones del menú
    public RectTransform cursorDot;                  // Punto como cursor
    public Vector2 offset = new Vector2(-60f, -5f);   // Desplazamiento visual del cursor
    public Transform marioTransform;                 // Referencia al transform de Mario
    public GameObject marioObject;                   // Objeto principal de Mario (asignar en Inspector)
    public float delayBeforeMenu = 2.5f;

    private CameraAndInput cameraControl;
    private int currentIndex = 0;
    private bool isGameOver = false;
    private float moveDelay = 0.2f;
    private float lastMoveTime = 0f;

    void Start()
    {
        
        cameraControl = FindObjectOfType<CameraAndInput>();
    }

    public void TriggerGameOver()
    {
          Debug.Log("Método TriggerGameOver ejecutado");

        if (isGameOver) return;
        isGameOver = true;

        panel.SetActive(true); // Mostrar panel inmediatamente

        if (cameraControl != null)
            cameraControl.cameraPaused = true;

        // Inicia seguimiento de cámara y espera
        StartCoroutine(FollowMarioFace(delayBeforeMenu));
        StartCoroutine(ShowGameOverMenuAfterDelay());
    }

    IEnumerator FollowMarioFace(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            if (marioTransform != null)
            {
                Vector3 marioPos = marioTransform.position;
                Vector3 marioForward = marioTransform.forward;
                float distance = 3f;
                float height = 2f;

                Vector3 cameraPos = marioPos - marioForward * distance + Vector3.up * height;
                Camera.main.transform.position = cameraPos;
                Camera.main.transform.LookAt(marioPos + Vector3.up * 1f);
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

 IEnumerator ShowGameOverMenuAfterDelay()
{
    yield return new WaitForSeconds(delayBeforeMenu);

    // Pausar todo menos Mario, el panel y este script
    PauseAllExcept(marioObject); // <- Asegura que no se incluya la cámara

    // ¡NO desbloqueamos la cámara aquí!
    if (cameraControl != null)
        cameraControl.cameraPaused = true;

    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;

    currentIndex = 0;
    UpdateCursorPosition();
}

 void PauseAllExcept(GameObject exceptObject)
{
    var allBehaviours = FindObjectsOfType<MonoBehaviour>();

    foreach (var script in allBehaviours)
    {
        if (script == this) continue; // Este script sí debe seguir
        if (script is CameraAndInput) continue; // NO pausar manualmente (ya está pausado con `cameraPaused`)
        if (script.gameObject == panel || script.transform.IsChildOf(panel.transform)) continue;
        if (exceptObject != null && (script.gameObject == exceptObject || script.transform.IsChildOf(exceptObject.transform)))
            continue;

        script.enabled = false;
    }
}


    void Update()
    {
        if (!isGameOver || !panel.activeSelf) return;

        if (Time.unscaledTime - lastMoveTime > moveDelay)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex = (currentIndex - 1 + menuOptions.Length) % menuOptions.Length;
                UpdateCursorPosition();
                lastMoveTime = Time.unscaledTime;
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex = (currentIndex + 1) % menuOptions.Length;
                UpdateCursorPosition();
                lastMoveTime = Time.unscaledTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            Button btn = menuOptions[currentIndex].GetComponent<Button>();
            if (btn != null)
                btn.onClick.Invoke();
        }
    }

    void UpdateCursorPosition()
    {
        if (cursorDot != null && currentIndex < menuOptions.Length)
        {
            RectTransform selected = menuOptions[currentIndex];
            cursorDot.position = selected.position + (Vector3)offset;
        }
    }

    public void RestartLevel()
    {
        if (cameraControl != null)
            cameraControl.cameraPaused = false;

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        if (cameraControl != null)
            cameraControl.cameraPaused = false;

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
