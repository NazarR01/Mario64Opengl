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

    [Header("Efectos de explosión")]
    [Tooltip("Prefab de partículas para la explosión.")]
    public GameObject explosionEffectPrefab;
    [Tooltip("Clip de audio a reproducir al explotar.")]
    public AudioClip explosionAudioClip;
    [Range(0f,1f)]
    [Tooltip("Volumen del SFX de explosión.")]
    public float explosionAudioVolume = 1f;

    [Header("Movimiento")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Ámbitos")]
    public float detectionRadius = 8f;
    public float explosionRadius = 1.2f;
    public float explosionDelay = 0.5f;

    [Header("Patrulla (opcional)")]
    public bool enablePatrol = false;
    public float patrolRadius = 5f;
    public float patrolInterval = 3f;

    private enum State { Patrol, Chase, Exploding }
    private State currentState = State.Patrol;

    private Vector3 patrolTarget;
    private float patrolTimer = 0f;

    private Rigidbody rb;
    private Collider col;
    private Renderer[] renderers;
    private bool isExploding = false;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();

        rb.useGravity = true;
        rb.isKinematic = false;

        // Reemplazar MeshCollider no-convex si existe
        if (col is MeshCollider mc && !mc.convex)
        {

            DestroyImmediate(mc);
            var sc = gameObject.AddComponent<SphereCollider>();
            sc.radius = 0.5f;
        }

    }

    void Start()
    {
        if (marioTransform == null)
            Debug.LogError("[Bobomb] Asigna marioTransform en el Inspector.");

        if (enablePatrol)
            ChooseNewPatrolTarget();
        else
            currentState = State.Chase;

    }

    void Update()
    {
        if (isExploding) return;

        float distToMario = Vector3.Distance(transform.position, marioTransform.position);

        if (currentState != State.Chase && distToMario <= detectionRadius)
            currentState = State.Chase;

        else if (currentState == State.Chase && distToMario > detectionRadius && enablePatrol)
        {
            currentState = State.Patrol;
            patrolTimer = 0f;
        }

        switch (currentState)
        {
            case State.Patrol: PatrolBehavior(); break;
            case State.Chase:  ChaseBehavior(distToMario); break;

        }
    }

    private void PatrolBehavior()
    {
        patrolTimer += Time.deltaTime;

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
            StartCoroutine(Explode());
        else
            MoveTowards(marioTransform.position, chaseSpeed);

    }

    private void MoveTowards(Vector3 target, float speed)
    {
        Vector3 current = transform.position;
        Vector3 dir = (new Vector3(target.x, current.y, target.z) - current).normalized;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            // El modelo mira hacia −Z, así que front = −dir
            Vector3 desiredForward = -dir;
            transform.forward = Vector3.Slerp(transform.forward, desiredForward, Time.deltaTime * 10f);


            Vector3 vel = dir * speed;
            vel.y = rb.velocity.y;
            rb.velocity = vel;
        }
        else
        {

            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
    }

    private void ChooseNewPatrolTarget()
    {
        Vector2 rnd = Random.insideUnitCircle * patrolRadius;
        Vector3 candidate = transform.position + new Vector3(rnd.x, 0f, rnd.y);

        if (Physics.Raycast(candidate + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f))
            candidate.y = hit.point.y + 0.05f;


        patrolTarget = candidate;
    }

    private IEnumerator Explode()
    {
        isExploding = true;
        currentState = State.Exploding;

        // Oculta el Bob-omb
        foreach (var rend in renderers) rend.enabled = false;
        col.enabled = false;
        rb.velocity = Vector3.zero;

        // 1) Partículas
        if (explosionEffectPrefab != null)
        {
            GameObject fx = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            var ps = fx.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(fx, ps.main.duration + ps.main.startLifetime.constantMax);
            else
                Destroy(fx, 2f);
        }

        // 2) SFX
        if (explosionAudioClip != null)
            AudioSource.PlayClipAtPoint(explosionAudioClip, transform.position, explosionAudioVolume);
        else
            Debug.LogWarning("[Bobomb] explosionAudioClip no asignado.");

        // 3) Daño a Mario (opcional)
        Interop.sm64_mario_apply_damage(0, 3);

        yield return new WaitForSeconds(explosionDelay);


        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
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
