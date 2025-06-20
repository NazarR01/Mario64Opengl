using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibSM64;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Goomba : MonoBehaviour
{
    [Header("Referencias")]
    public Transform marioTransform;

    [Header("Velocidades")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3f;
    public float detectionRadius = 6f;

    [Header("Patrulla")]
    public bool enablePatrol = true;
    public float patrolRadius = 4f;
    public float patrolInterval = 3f;

    [Header("Audio")]
    public AudioClip damageAudioClip;
    [Range(0f, 1f)] public float damageAudioVolume = 1f;

    public AudioClip alertAudioClip;
    [Range(0f, 1f)] public float alertAudioVolume = 0.7f;

    public AudioClip chaseLoopClip;
    [Range(0f, 1f)] public float chaseLoopVolume = 0.5f;

    [Tooltip("Tiempo (en segundos) que espera después del sonido de alerta antes de reproducir el sonido de persecución.")]
    public float chaseDelay = 1f;

    public AudioClip stompAudioClip;
    [Range(0f, 1f)] public float stompAudioVolume = 1f;

    [Header("Aplastar")]
    public float stompHeightThreshold = 0.3f;
    public float squashDuration = 0.2f;

    private AudioSource chaseLoopSource;
    private bool hasPlayedAlert = false;
    private bool isChaseLoopDelayed = false;

    private Rigidbody rb;
    private Vector3 patrolTarget;
    private float patrolTimer = 0f;

    private enum State { Patrol, Chase, Dead }
    private State currentState = State.Patrol;

    private bool isDead = false;
    private bool canChase = true; // Si puede perseguir a Mario o está bloqueado (cuando Mario es invulnerable)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (marioTransform == null)
            Debug.LogError("⚠️ marioTransform no asignado.");

        if (enablePatrol)
            ChooseNewPatrolTarget();
        else
            currentState = State.Chase;

        chaseLoopSource = gameObject.AddComponent<AudioSource>();
        chaseLoopSource.clip = chaseLoopClip;
        chaseLoopSource.loop = true;
        chaseLoopSource.volume = chaseLoopVolume;
        chaseLoopSource.playOnAwake = false;
    }

    void Update()
    {
        if (isDead) return;

        float distToMario = Vector3.Distance(transform.position, marioTransform.position);

        if (!canChase)
        {
            // Goomba no persigue mientras canChase es falso
            StopChaseLoop();
            return;
        }

        if (currentState != State.Chase && distToMario <= detectionRadius)
        {
            currentState = State.Chase;
            PlayAlertOnce();
            if (!isChaseLoopDelayed)
                StartCoroutine(PlayChaseLoopAfterDelay(chaseDelay));
        }
        else if (currentState == State.Chase && distToMario > detectionRadius && enablePatrol)
        {
            currentState = State.Patrol;
            patrolTimer = 0f;
            StopChaseLoop();
            hasPlayedAlert = false;
            isChaseLoopDelayed = false;
        }

        switch (currentState)
        {
            case State.Patrol: PatrolBehavior(); break;
            case State.Chase: ChaseBehavior(); break;
        }
    }

    void PatrolBehavior()
    {
        patrolTimer += Time.deltaTime;

        if (patrolTimer >= patrolInterval || Vector3.Distance(transform.position, patrolTarget) < 0.5f)
        {
            patrolTimer = 0f;
            ChooseNewPatrolTarget();
        }

        MoveTowards(patrolTarget, patrolSpeed);
    }

    void ChaseBehavior()
    {
        MoveTowards(marioTransform.position, chaseSpeed);
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 current = transform.position;
        Vector3 dir = (target - current);
        dir.y = 0f;
        dir.Normalize();

        if (dir.sqrMagnitude > 0.01f)
        {
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

    void ChooseNewPatrolTarget()
    {
        Vector2 rnd = Random.insideUnitCircle * patrolRadius;
        Vector3 candidate = transform.position + new Vector3(rnd.x, 0f, rnd.y);

        if (Physics.Raycast(candidate + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f))
            candidate.y = hit.point.y + 0.05f;

        patrolTarget = candidate;
    }

    void OnCollisionEnter(Collision collision)
    {
       if (isDead) return;

    if (collision.transform == marioTransform)
    {
        ContactPoint contact = collision.GetContact(0);
        float yDiff = marioTransform.position.y - contact.point.y;

        MarioInvulnerability marioInvul = marioTransform.GetComponent<MarioInvulnerability>();

        if (yDiff > stompHeightThreshold)
        {
            Rigidbody marioRb = marioTransform.GetComponent<Rigidbody>();
            if (marioRb != null)
                marioRb.velocity = new Vector3(marioRb.velocity.x, 6f, marioRb.velocity.z);

            StartCoroutine(SquashAndDie());
        }
        else
        {
            if (marioInvul != null && !marioInvul.IsInvulnerable())
            {
                // ✅ Aplica daño + sonido con MarioHealt
                MarioHealt marioHealt = marioTransform.GetComponent<MarioHealt>();
                if (marioHealt != null)
                    Interop.sm64_mario_apply_damage(0, 4);

                // ✅ Invulnerabilidad
                marioInvul.StartInvulnerability(3f);

                // ✅ Detener persecución temporal
                canChase = false;
                StartCoroutine(EnableChaseAfterDelay(3f));
            }
        }
    }
    }

    IEnumerator SquashAndDie()
    {
        isDead = true;
        currentState = State.Dead;

        StopChaseLoop();

        if (stompAudioClip != null)
            AudioSource.PlayClipAtPoint(stompAudioClip, transform.position, stompAudioVolume);

        Vector3 originalScale = transform.localScale;
        Vector3 squashedScale = new Vector3(originalScale.x, 0.2f, originalScale.z);

        float t = 0f;
        while (t < squashDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, squashedScale, t / squashDuration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = squashedScale;

        rb.velocity = Vector3.zero; // Para que se quede quieto

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    IEnumerator EnableChaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canChase = true;
    }

    void PlayAlertOnce()
    {
        if (!hasPlayedAlert && alertAudioClip != null)
        {
            AudioSource.PlayClipAtPoint(alertAudioClip, transform.position, alertAudioVolume);
            hasPlayedAlert = true;
        }
    }

    IEnumerator PlayChaseLoopAfterDelay(float delay)
    {
        isChaseLoopDelayed = true;
        yield return new WaitForSeconds(delay);
        if (!chaseLoopSource.isPlaying && !isDead)
            chaseLoopSource.Play();
    }

    void StopChaseLoop()
    {
        if (chaseLoopSource != null && chaseLoopSource.isPlaying)
            chaseLoopSource.Stop();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        if (enablePatrol)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, patrolRadius);
        }
    }
}