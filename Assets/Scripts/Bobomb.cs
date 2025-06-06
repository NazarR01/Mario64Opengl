using System.Collections;
using UnityEngine;
using LibSM64;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Bobomb : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Transform de Mario (debe tener SM64Mario).")]
    public Transform marioTransform;

    [Tooltip("Prefab o GameObject que maneje la animación de explosión (opcional).")]
    public GameObject explosionEffectPrefab;

    [Header("Movimiento")]
    [Tooltip("Velocidad de patrulla (m/s) cuando aún no detecta a Mario.")]
    public float patrolSpeed = 2f;

    [Tooltip("Velocidad de persecución (m/s) cuando persigue a Mario.")]
    public float chaseSpeed = 4f;

    [Header("Ámbitos")]
    [Tooltip("Radio (en unidades) de detección de Mario para comenzar persecución.")]
    public float detectionRadius = 8f;

    [Tooltip("Distancia (en unidades) a Mario para explotar y matar.")]
    public float explosionRadius = 1.2f;

    [Tooltip("Duración en segundos antes de destruir al explotar.")]
    public float explosionDelay = 0.5f;

    [Header("Patrulla (si se quiere)")]
    [Tooltip("Si es true, este Bob-omb patrullará aleatoriamente hasta detectar a Mario.")]
    public bool enablePatrol = false;

    [Tooltip("Radio de patrulla en XZ (solo si enablePatrol=true).")]
    public float patrolRadius = 5f;

    [Tooltip("Tiempo en segundos que tarda en elegir un nuevo punto de patrulla.")]
    public float patrolInterval = 3f;

    // Estado interno
    private enum State { Patrol, Chase, Exploding }
    private State currentState = State.Patrol;

    private Vector3 patrolTarget;
    private float patrolTimer = 0f;

    private Rigidbody rb;
    private Collider col;
    private Renderer[] renderers;

    private bool isExploding = false;

    // Para disparar muerte de Mario
    private SM64Mario marioScript;
    private int marioId = 0;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();

        // Asegurarse de que el Rigidbody tenga gravedad real
        rb.useGravity = true;
        rb.isKinematic = false;

        // Comprobar que el Collider no sea MeshCollider no-convex
        if (col is MeshCollider mc && !mc.convex)
        {
            Debug.LogWarning("[BobombEnhanced] Se detectó un MeshCollider no-convex. Cambiando a SphereCollider.");

            // Remplazamos con un SphereCollider con radio aproximado
            DestroyImmediate(mc);
            var sc = gameObject.AddComponent<SphereCollider>();
            sc.radius = 0.5f;
        }

        if (marioTransform != null)
        {
            marioScript = marioTransform.GetComponent<SM64Mario>();
            if (marioScript != null)
                marioId = 0;
            else
                Debug.LogWarning("[BobombEnhanced] No se encontró SM64Mario en marioTransform.");
        }
        else
        {
            Debug.LogError("[BobombEnhanced] marioTransform no está asignado.");
        }
        
    }

    void Start()
    {
        // Si está habilitada patrulla, elegir primero
        if (enablePatrol)
            ChooseNewPatrolTarget();
        else
            currentState = State.Chase; // si no patrulla, va directo a perseguir
    }

    void Update()
    {
        if (isExploding) return;

        // 1) Comprobar distancia a Mario
        float distToMario = Vector3.Distance(transform.position, marioTransform.position);

        if (currentState != State.Chase && distToMario <= detectionRadius)
        {
            currentState = State.Chase;
        }
        else if (currentState == State.Chase && distToMario > detectionRadius && enablePatrol)
        {
            currentState = State.Patrol;
            patrolTimer = 0f;
        }

        // 2) Ejecutar comportamiento según estado
        switch (currentState)
        {
            case State.Patrol:
                PatrolBehavior();
                break;
            case State.Chase:
                ChaseBehavior(distToMario);
                break;
        }
    }

    private void PatrolBehavior()
    {
        patrolTimer += Time.deltaTime;
        // Si llegó al punto o se acabó el tiempo, elegir otro
        Vector3 currXZ = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 targXZ = new Vector3(patrolTarget.x, 0f, patrolTarget.z);

        if (patrolTimer >= patrolInterval || Vector3.Distance(currXZ, targXZ) < 0.5f)
        {
            patrolTimer = 0f;
            ChooseNewPatrolTarget();
        }

        MoveTowards(patrolTarget, patrolSpeed);
    }

    private void ChaseBehavior(float distToMario)
    {
        if (distToMario <= explosionRadius)
        {
            StartCoroutine(Explode());
        }
        else
        {
            MoveTowards(marioTransform.position, chaseSpeed);
        }
    }

    private void MoveTowards(Vector3 target, float speed)
    {
        Vector3 current = transform.position;
        Vector3 dir = (new Vector3(target.x, current.y, target.z) - current).normalized;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            // Rotar frontalmente
            transform.forward = Vector3.Slerp(transform.forward, -dir, Time.deltaTime * 10f);

            // Aplicar velocidad XZ + mantener gravedad
            Vector3 vel = dir * speed;
            vel.y = rb.velocity.y;
            rb.velocity = vel;
        }
        else
        {
            // Si no se mueve en XZ, mantener solo componente Y
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
    }

    private void ChooseNewPatrolTarget()
    {
        Vector2 rnd = Random.insideUnitCircle * patrolRadius;
        Vector3 candidate = transform.position + new Vector3(rnd.x, 0f, rnd.y);

        RaycastHit hit;
        float rayFrom = transform.position.y + 5f;
        if (Physics.Raycast(new Vector3(candidate.x, rayFrom, candidate.z), Vector3.down, out hit, 10f))
        {
            candidate.y = hit.point.y + 0.05f; // pequeño offset para no quedar cortado
        }
        else
        {
            candidate.y = transform.position.y;
        }

        patrolTarget = candidate;
    }

    private IEnumerator Explode()
    {
        isExploding = true;
        currentState = State.Exploding;

        foreach (var rend in renderers)
            rend.enabled = false;

        col.enabled = false;

        rb.velocity = Vector3.zero;

        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        // Asignar muerte a Mario y terminar nivel
        if (marioScript != null)
        Interop.sm64_mario_apply_damage(0, 3);

        yield return new WaitForSeconds(explosionDelay);
        // Ejemplo: SceneManager.LoadScene("GameOverScene");

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar radios
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        if (enablePatrol)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, patrolRadius);
        }
    }
}
