using UnityEngine;
using UnityEngine.UI;

public class MenuC : MonoBehaviour
{
    public RectTransform[] menuOptions;   // Botones u opciones del menú
    public RectTransform cursorDot;       // Punto como cursor
    public Vector2 offset = new Vector2(-20f, 0f); // Menor desplazamiento que la mano

    public AudioClip moveSound;
    public AudioClip selectSound;
    [Range(0f, 1f)] public float volume = 1f;

    private int currentIndex = 0;
    private float moveDelay = 0.2f;
    private float lastMoveTime = 0f;

    void Start()
    {
        UpdateCursorPosition();
    }

    void Update()
    {
        if (Time.time - lastMoveTime > moveDelay)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex = (currentIndex - 1 + menuOptions.Length) % menuOptions.Length;
                UpdateCursorPosition();
                PlaySound(moveSound);
                lastMoveTime = Time.time;
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex = (currentIndex + 1) % menuOptions.Length;
                UpdateCursorPosition();
                PlaySound(moveSound);
                lastMoveTime = Time.time;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            Button btn = menuOptions[currentIndex].GetComponent<Button>();
            if (btn != null)
            PlaySound(selectSound);
                btn.onClick.Invoke();
        }
    }
 void PlaySound(AudioClip clip)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
    }
    void UpdateCursorPosition()
    {
        if (cursorDot != null && currentIndex < menuOptions.Length)
        {
            RectTransform selected = menuOptions[currentIndex];
            cursorDot.position = selected.position + (Vector3)offset;
        }
    }
}
