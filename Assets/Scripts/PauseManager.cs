using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public RectTransform cursor;
    public Button[] buttons;

    private int selectedIndex = 0;
    private bool isPaused = false;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        MoveCursorToSelected();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else 
                PauseGame();
             

        }

        if (!isPaused) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + buttons.Length) % buttons.Length;
            MoveCursorToSelected();
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % buttons.Length;
            MoveCursorToSelected();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            buttons[selectedIndex].onClick.Invoke();
        }
    }

  void PauseGame()
{
    isPaused = true;
    Time.timeScale = 0f;
    pauseMenuPanel.SetActive(true);
    selectedIndex = 0;
    MoveCursorToSelected();

    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;
}
void ResumeGame()
{
    isPaused = false;
    Time.timeScale = 1f;
    pauseMenuPanel.SetActive(false);

    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
}
    void MoveCursorToSelected()
    {
        if (cursor != null && buttons[selectedIndex] != null)
        {
            RectTransform buttonTransform = buttons[selectedIndex].GetComponent<RectTransform>();
            Vector3 newPos = buttonTransform.position;
            newPos.x -= 30f; 
            cursor.position = newPos;
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
