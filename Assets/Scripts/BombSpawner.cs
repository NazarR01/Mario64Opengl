using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BombSpawner : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Prefab del Bob-omb (debe tener BobombEnhanced).")]
    public GameObject bobombPrefab;

    [Tooltip("Transform de Mario (GameObject con SM64Mario).")]
    public Transform marioTransform;

    [Tooltip("Objeto plano que define el área de spawn: debe estar colocado justo encima del mapa, sin rotación, con un Renderer o Collider.")]
    public GameObject referencePlane;

    [Header("Spawning")]
    [Tooltip("Cantidad total de Bob-ombs a spawnear.")]
    public int totalBombCount = 10;

    [Tooltip("Intervalo (en segundos) entre cada spawn.")]
    public float spawnInterval = 1.0f;

    [Tooltip("Elevación extra sobre el plano desde donde caen los Bob-ombs.")]
    public float spawnHeightOffset = 5f;

    [Tooltip("Radio mínimo (en XZ) para que dos Bob-ombs no aparezcan demasiado cerca.")]
    public float minSeparation = 2f;

    [Tooltip("Máscara de capa(s) que identifican el suelo (para raycast hacia abajo).")]
    public LayerMask groundLayerMask;

    [Tooltip("Máscara de capa(s) que identifican obstáculos (paredes) donde no queremos que aterricen).")]
    public LayerMask obstacleLayerMask;

    [Header("Supervivencia")]
    [Tooltip("Tiempo (en segundos) que el jugador debe sobrevivir DESPUÉS de que se ha spawneado el último Bob-omb.")]
    public float postSpawnSurviveTime = 10f;

    // --- Estado interno ---
    private int spawnedCount = 0;
    private bool spawningFinished = false;
    private float postSpawnTimer = 0f;
    private bool levelEnded = false;

    // Para llevar las posiciones XZ ocupadas por Bob-ombs ya spawneados
    private List<Vector2> existingPositions = new List<Vector2>();
    private Bounds planeBounds;

    void Start()
    {
        if (bobombPrefab == null || marioTransform == null || referencePlane == null)
        {
            Debug.LogError("[BobombSpawner] Asigna bobombPrefab, marioTransform y referencePlane en el Inspector.");
            enabled = false;
            return;
        }

        // Calcular los bounds XZ del referencePlane
        var rend = referencePlane.GetComponent<Renderer>();
        if (rend != null)
        {
            planeBounds = rend.bounds;
        }
        else
        {
            var col = referencePlane.GetComponent<Collider>();
            if (col != null)
                planeBounds = col.bounds;
            else
            {
                Debug.LogError("[BobombSpawner] referencePlane debe tener un Renderer o Collider para definir sus bounds.");
                enabled = false;
                return;
            }
        }

        // Iniciar corrutina de spawn
        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        if (levelEnded) return;

        if (spawningFinished)
        {
            postSpawnTimer += Time.deltaTime;
            if (postSpawnTimer >= postSpawnSurviveTime)
            {
                levelEnded = true;
                OnWin();
            }
        }
    }

    /// <summary>
    /// Corrutina que spawnea un Bob-omb cada spawnInterval hasta llegar a totalBombCount.
    /// </summary>
    private IEnumerator SpawnRoutine()
    {
        while (spawnedCount < totalBombCount)
        {
            SpawnOneBomb();
            spawnedCount++;
            yield return new WaitForSeconds(spawnInterval);
        }

        spawningFinished = true;
    }

    /// <summary>
    /// Instancia un solo Bob-omb en una posición XZ aleatoria dentro de los bounds del plane,
    /// a la altura del plane más spawnHeightOffset, luego ajustará su Y automáticamente al caer.
    /// </summary>
    private void SpawnOneBomb()
    {
        Vector3 spawnPos = Vector3.zero;
        int attempts = 0;
        bool found = false;

        while (attempts < 30)
        {
            attempts++;

            // 1) Elegir XZ al azar dentro de los bounds del plane
            float x = Random.Range(planeBounds.min.x, planeBounds.max.x);
            float z = Random.Range(planeBounds.min.z, planeBounds.max.z);
            float y = planeBounds.max.y + spawnHeightOffset;

            Vector3 candidate = new Vector3(x, y, z);

            // 2) Comprobar separación mínima en XZ
            bool tooClose = false;
            foreach (var pos in existingPositions)
            {
                Vector2 a = pos;
                Vector2 b = new Vector2(x, z);
                if (Vector2.Distance(a, b) < minSeparation)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue;

            // 3) Verificar que, al caer, no quedará dentro de un obstáculo
            //    Raycast hacia abajo desde (x, y, z) contra groundLayerMask para encontrar suelo
            if (Physics.Raycast(candidate, Vector3.down, out RaycastHit hit, spawnHeightOffset + 100f, groundLayerMask))
            {
                Vector3 groundPoint = hit.point; // punto en suelo
                // Hacemos un pequeño CheckSphere justo encima del suelo para ver si hay pared
                Vector3 aboveGround = groundPoint + Vector3.up * 0.5f;
                if (Physics.CheckSphere(aboveGround, 0.3f, obstacleLayerMask))
                {
                    // Si hay colisión con “obstacleLayerMask” justo sobre el suelo, descartamos
                    continue;
                }

                // Si llegamos aquí, candidate es válido
                spawnPos = candidate;
                found = true;
                break;
            }
        }

        if (!found)
        {
            Debug.LogWarning("[BobombSpawner] No se encontró posición válida tras 30 intentos. Se salta este spawn.");
            return;
        }

        // 4) Instanciar Bob-omb en spawnPos
        GameObject bomb = Instantiate(bobombPrefab, spawnPos, Quaternion.identity);

        var be = bomb.GetComponent<Bobomb>();
        if (be != null)
        {
            be.marioTransform = marioTransform;
        }
      

        existingPositions.Add(new Vector2(spawnPos.x, spawnPos.z));
    }

    /// <summary>
    /// Llamar cuando el jugador muere para terminar en derrota.
    /// </summary>
    public void OnPlayerDeath()
    {
        if (levelEnded) return;
        levelEnded = true;
        Debug.Log("[BobombSpawner] Derrota: Mario murió.");

        // Cargar escena de derrota o mostrar UI aquí:
        // SceneManager.LoadScene("GameOverScene");
    }

    /// <summary>
    /// Llamar cuando el jugador sobrevive el tiempo completo (postSpawnSurviveTime).
    /// </summary>
    private void OnWin()
    {
        Debug.Log("[BobombSpawner] ¡Victoria! Sobreviviste el tiempo establecido.");

        // Cargar escena de victoria o mostrar UI aquí:
        // SceneManager.LoadScene("VictoryScene");
    }

    private void OnDrawGizmosSelected()
    {
        if (referencePlane == null) return;

        // Dibujar los bounds del referencePlane
        var rend = referencePlane.GetComponent<Renderer>();
        Bounds b;
        if (rend != null)
            b = rend.bounds;
        else
        {
            var col = referencePlane.GetComponent<Collider>();
            if (col != null) b = col.bounds;
            else return;
        }

        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawCube(b.center, b.size);
    }
}
