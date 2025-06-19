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

    [Header("Pies")]
    public Transform footLeft;
    public Transform footRight;
    public float footSwingAmount = 15f;
    public float footSwingSpeed = 8f;

    [Header("Aplastar")]
    public float stompHeightThreshold = 0.3f;
    public float squashDuration = 0.2f;

    private Rigidbody rb;
    private Vector3 patrolTarget;
    private float patrolTimer = 0f;
    private enum State { Patrol, Chase, Dead }
    private State currentState = State.Patrol;

    private float footAnimTimer = 0f;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Evitar que se incline al moverse
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (marioTransform == null)
            Debug.LogError("⚠️ marioTransform no asignado.");

        if (enablePatrol)
            ChooseNewPatrolTarget();
        else
            currentState = State.Chase;
    }

    void Update()
    {
        if (isDead) return;

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
            case State.Chase:  ChaseBehavior(); break;
        }

        AnimateFeet();
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
            transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * 10f);
            Vector3 vel = dir * speed;
            vel.y = rb.velocity.y;
            rb.velocity = vel;
        }
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
    }

    void AnimateFeet()
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            footAnimTimer += Time.deltaTime * footSwingSpeed;
            float angle = Mathf.Sin(footAnimTimer) * footSwingAmount;

            if (footLeft != null) footLeft.localRotation = Quaternion.Euler(angle, 0, 0);
            if (footRight != null) footRight.localRotation = Quaternion.Euler(-angle, 0, 0);
        }
        else
        {
            if (footLeft != null) footLeft.localRotation = Quaternion.identity;
            if (footRight != null) footRight.localRotation = Quaternion.identity;
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

            if (yDiff > stompHeightThreshold)
            {
                // Rebota Mario al pisarlo
                Rigidbody marioRb = marioTransform.GetComponent<Rigidbody>();
                if (marioRb != null)
                    marioRb.velocity = new Vector3(marioRb.velocity.x, 6f, marioRb.velocity.z);

                StartCoroutine(SquashAndDie());
            }
            else
            {
                // Aplica daño a Mario
                LibSM64.Interop.sm64_mario_apply_damage(0, 1);
            }
        }
    }

    IEnumerator SquashAndDie()
    {
        isDead = true;
        currentState = State.Dead;

        Vector3 originalScale = transform.localScale;
        Vector3 squashedScale = new Vector3(originalScale.x, originalScale.y * 0.2f, originalScale.z);

        float t = 0f;
        while (t < squashDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, squashedScale, t / squashDuration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = squashedScale;
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
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