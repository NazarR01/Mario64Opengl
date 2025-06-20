using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LibSM64;
using System.Collections;

public class Star : MonoBehaviour
{
    [Tooltip("ID de Mario en libsm64")]
    public uint marioId;

    public string starTag = "Star";
    public GameObject winPanel;              // Panel de "¡Ganaste!"
    public Transform marioTransform;         // Mario para posicionar la cámara
    public GameObject fadePanel;             // Panel negro para fade
    public Camera mainCamera;                // Cámara principal (opcional si ya está asignada)
    public float cameraDistance = 3f;
    public float cameraHeight = 2f;
    public float delayBeforeReturn = 3f;
    public float fadeDuration = 2f;

    private bool starGrabbed = false;
    private const int ACT_STAR_DANCE_EXIT = 0x00001302;
    private Camera victoryCamera;

    private void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);

        if (fadePanel != null)
            fadePanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!starGrabbed && other.CompareTag("Player"))
        {
            starGrabbed = true;

            Debug.Log("Mario ha agarrado la estrella!");

            Interop.sm64_mario_set_cutscene_action((int)marioId, ACT_STAR_DANCE_EXIT, 0);

            var camScript = FindObjectOfType<CameraAndInput>();
            if (camScript != null)
                camScript.cameraPaused = true;

            if (mainCamera != null)
                mainCamera.gameObject.SetActive(false);

            CreateVictoryCamera();
            PositionVictoryCamera();

            if (winPanel != null)
                winPanel.SetActive(true);

            StartCoroutine(ReturnToMenuAfterDelay());
        }
    }
public void TriggerVictory()
{
    if (starGrabbed) return;
    starGrabbed = true;

    Debug.Log("Victoria manual activada");

    Interop.sm64_mario_set_cutscene_action((int)marioId, ACT_STAR_DANCE_EXIT, 0);

    var camScript = FindObjectOfType<CameraAndInput>();
    if (camScript != null)
        camScript.cameraPaused = true;

    if (mainCamera != null)
        mainCamera.gameObject.SetActive(false);

    CreateVictoryCamera();
    PositionVictoryCamera();

    if (winPanel != null)
        winPanel.SetActive(true);

    StartCoroutine(ReturnToMenuAfterDelay());
}//llamar asi en el otro script   starScript?.TriggerVictory();

    void CreateVictoryCamera()
    {
        GameObject camObj = new GameObject("VictoryCamera");
        victoryCamera = camObj.AddComponent<Camera>();
        victoryCamera.backgroundColor = mainCamera.backgroundColor;
        // Opcional: configurar valores básicos
        victoryCamera.clearFlags = CameraClearFlags.Skybox;
        victoryCamera.fieldOfView = 60f;
        victoryCamera.nearClipPlane = 0.1f;
        victoryCamera.farClipPlane = 100f;
        victoryCamera.depth = 100f; // Asegura que esté al frente
            Skybox mainSkybox = mainCamera.GetComponent<Skybox>();
        if (mainSkybox != null)
        {
            Skybox newSkybox = camObj.AddComponent<Skybox>();
            newSkybox.material = mainSkybox.material;
        }
    }

    void PositionVictoryCamera()
    {
        if (marioTransform == null || victoryCamera == null) return;

        Vector3 marioPos = marioTransform.position;
        Vector3 marioForward = marioTransform.forward;

        // Mirar a Mario desde el frente
        Vector3 cameraPos = marioPos - marioForward * cameraDistance + Vector3.up * cameraHeight;
        victoryCamera.transform.position = cameraPos;
        victoryCamera.transform.LookAt(marioPos + Vector3.up * 1f);
    }

    IEnumerator ReturnToMenuAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeReturn);

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

        SceneManager.LoadScene("MainMenu");
    }
}
