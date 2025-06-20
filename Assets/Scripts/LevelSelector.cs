using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSelector : MonoBehaviour
{
    public RectTransform[] levelOptions;
    public RectTransform cursorDot;
    public Vector2 offset = new Vector2(0f, -30f);

    [Header("Audio")]
    public AudioClip moveSound;
    public AudioClip selectSound;
    [Range(0f, 1f)] public float volume = 1f;

    [Header("Escenas")]
    public string[] levelSceneNames;

    [Header("Fade")]
    public GameObject fadePanel;           // Panel con Image negro
    public float fadeDuration = 1.5f;      // Tiempo del fade

    private int currentIndex = 0;
    private float moveDelay = 0.2f;
    private float lastMoveTime = 0f;

    void Start()
    {
        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
            var img = fadePanel.GetComponent<Image>();
            img.color = new Color(0, 0, 0, 0); // Transparente al inicio
        }

        UpdateCursorPosition();
    }

    void Update()
    {
        if (Time.time - lastMoveTime > moveDelay)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentIndex = (currentIndex - 1 + levelOptions.Length) % levelOptions.Length;
                UpdateCursorPosition();
                PlaySound(moveSound);
                lastMoveTime = Time.time;
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentIndex = (currentIndex + 1) % levelOptions.Length;
                UpdateCursorPosition();
                PlaySound(moveSound);
                lastMoveTime = Time.time;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            PlaySound(selectSound);
            StartCoroutine(FadeAndLoadScene());
        }
    }

    void UpdateCursorPosition()
    {
        if (cursorDot != null && currentIndex < levelOptions.Length)
        {
            RectTransform selected = levelOptions[currentIndex];
            cursorDot.position = selected.position + (Vector3)offset;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
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

        if (currentIndex < levelSceneNames.Length)
        {
            string sceneName = levelSceneNames[currentIndex];
            if (!string.IsNullOrEmpty(sceneName))
                SceneManager.LoadScene(sceneName);
        }
    }
}
