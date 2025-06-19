using System.Collections;
using UnityEngine;
using LibSM64;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Bobomb : MonoBehaviour
{
    [Header("Referencias")]
    public Transform marioTransform;

    [Header("Efectos de explosión")]
    public GameObject explosionEffectPrefab;
    public AudioClip explosionAudioClip;
    [Range(0f,1f)] public float explosionAudioVolume = 1f;

    public AudioClip damageAudioClip;
    [Range(0f,1f)] public float damageAudioVolume = 1f;

    [Header("Movimiento & Colisión")]
    public float patrolSpeed = 2f;
    public float chaseSpeed  = 4f;

    [Header("Ámbitos")]
    public float detectionRadius  = 8f;
    public float explosionRadius  = 1.2f;
    public float explosionDelay   = 0.5f;

    [Header("Patrulla (opcional)")]
    public bool  enablePatrol   = false;
    public float patrolRadius   = 5f;
    public float patrolInterval = 3f;

    enum State { Patrol, Chase, Exploding }
    State currentState = State.Patrol;
    Vector3 patrolTarget;
    float patrolTimer;

    Rigidbody  rb;
    Collider   col;
    Renderer[] renderers;
    bool       isExploding;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();

        // Física estándar:
        rb.useGravity = true;                   // gravedad real
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Start()
    {
        if (marioTransform == null)
            Debug.LogError("[Bobomb] Asigna marioTransform en el Inspector.");

        currentState = enablePatrol ? State.Patrol : State.Chase;
        if (enablePatrol) ChooseNewPatrolTarget();
    }

    void Update()
    {
        if (isExploding) return;

        float dist = Vector3.Distance(transform.position, marioTransform.position);
        if (currentState != State.Chase && dist <= detectionRadius)
        {
            currentState = State.Chase;
        }
        else if (currentState == State.Chase && dist > detectionRadius && enablePatrol)
        {
            currentState = State.Patrol;
            patrolTimer = 0f;
        }

        if (currentState == State.Patrol) PatrolBehavior();
        else if (currentState == State.Chase) ChaseBehavior(dist);
    }

    void PatrolBehavior()
    {
        patrolTimer += Time.deltaTime;
        Vector3 currXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targXZ = new Vector3(patrolTarget.x,0,patrolTarget.z);
        if (patrolTimer >= patrolInterval || Vector3.Distance(currXZ,targXZ)<0.5f)
        {
            patrolTimer = 0f;
            ChooseNewPatrolTarget();
        }
        MoveTowards(patrolTarget, patrolSpeed);
    }

    void ChaseBehavior(float dist)
    {
        if (dist <= explosionRadius) StartCoroutine(Explode());
        else MoveTowards(marioTransform.position, chaseSpeed);
    }

    /// <summary>
    /// Mueve horizontalmente usando Rigidbody.velocity, preservando Y para la gravedad y colisiones.
    /// </summary>
    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 curr = transform.position;
        Vector3 dir = (new Vector3(target.x, curr.y, target.z) - curr).normalized;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f)
        {
            // dejar solo la componente Y
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        // Girar el modelo (mira –Z)
        Vector3 desiredForward = -dir;
        transform.forward = Vector3.Slerp(transform.forward, desiredForward, Time.deltaTime * 10f);

        // Aplicar velocidad horizontal + conservar vertical
        Vector3 newVel = dir * speed;
        newVel.y = rb.velocity.y;
        rb.velocity = newVel;
    }

    void ChooseNewPatrolTarget()
    {
        Vector2 rnd = Random.insideUnitCircle * patrolRadius;
        Vector3 cand = transform.position + new Vector3(rnd.x, 0f, rnd.y);

        // Ajustar Y por colisión de suelo
        if (Physics.Raycast(cand + Vector3.up * 10f, Vector3.down, out var hit, 20f))
            cand.y = hit.point.y + 0.05f;

        patrolTarget = cand;
    }

    IEnumerator Explode()
    {
        isExploding = true;
        currentState = State.Exploding;

        // Ocultar visual y desactivar collider
        foreach (var r in renderers) r.enabled = false;
        col.enabled = false;
        rb.velocity = Vector3.zero;

        // Partículas
        if (explosionEffectPrefab != null)
        {
            var fx = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            var ps = fx.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(fx, ps.main.duration + ps.main.startLifetime.constantMax);
            else
                Destroy(fx, 2f);
        }

        // Audio
        if (explosionAudioClip != null)
            AudioSource.PlayClipAtPoint(explosionAudioClip, transform.position, explosionAudioVolume);
        if (damageAudioClip != null)
            AudioSource.PlayClipAtPoint(damageAudioClip, transform.position, damageAudioVolume);

        // Daño a Mario
        LibSM64.Interop.sm64_mario_apply_damage(0, 3);

        yield return new WaitForSeconds(explosionDelay);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;    Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.magenta;Gizmos.DrawWireSphere(transform.position, explosionRadius);
        if (enablePatrol)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, patrolRadius);
        }
    }
}
