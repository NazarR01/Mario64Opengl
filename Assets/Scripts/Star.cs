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
    public float cameraDistance = 5.5f;
    public float cameraHeight = 2f;
    public float delayBeforeReturn = 3f;
    public float fadeDuration = 2f;
    [SerializeField] private GameObject Heart;
    [SerializeField] private GameObject monedas;
    [SerializeField] private GameObject textotimer;
    
    public bool starGrabbed = false;
    public bool StarGrabbed => starGrabbed;
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
            if (Heart != null) Heart.SetActive(false);
if (monedas != null) monedas.SetActive(false);
if (textotimer != null) textotimer.SetActive(false);
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
    if (marioTransform == null || victoryCamera == null || mainCamera == null) return;

    Vector3 marioPos = marioTransform.position;

    // Obtener dirección desde cámara principal hacia Mario
  

    // Evitar que la cámara quede por debajo del terreno
    float minHeight = marioPos.y + 1.3f; // Al menos medio metro sobre el suelo de Mario


   float minDistance = 3.0f;
Vector3 toMario = marioTransform.forward;
Vector3 desiredPos = marioTransform.position + toMario * cameraDistance + Vector3.up * cameraHeight;

Vector3 direction = (desiredPos - marioTransform.position).normalized;
float actualDistance = Vector3.Distance(desiredPos, marioTransform.position);

// Si está muy cerca, empújala hacia atrás
if (actualDistance < minDistance)
{
    desiredPos = marioTransform.position + direction * minDistance + Vector3.up * cameraHeight;
}

desiredPos.y = Mathf.Max(desiredPos.y, minHeight);
victoryCamera.transform.position = desiredPos;
victoryCamera.transform.LookAt(marioTransform.position + Vector3.up * 1f);

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
         
        SceneManager.LoadScene("LvlSelect");
    }
}
