using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GoombaWalk : MonoBehaviour
{
    [Header("Partes del Goomba")]
    public Transform bodyTransform;
    public Transform leftFoot;
    public Transform rightFoot;

    [Header("Parámetros de movimiento")]
    public float minStepSpeed = 0.1f;
    public float baseStepFrequency = 2f;
    public float stepAngleAmplitude = 30f;
    public float bobbingHeight = 0.05f;
    public float bobbingFrequency = 2f;

    private Rigidbody rb;
    private float stepTimer = 0f;
    private Vector3 bodyStartPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (bodyTransform == null) bodyTransform = transform;
        bodyStartPos = bodyTransform.localPosition;
    }

    void Update()
    {
        Vector3 vel = rb.velocity;
        float speed = new Vector2(vel.x, vel.z).magnitude;

        if (speed > minStepSpeed)
        {
            float freq = baseStepFrequency * speed;
            stepTimer += Time.deltaTime * freq * Mathf.PI * 2f;

            float angle = Mathf.Sin(stepTimer) * stepAngleAmplitude;
            if (leftFoot != null)
                leftFoot.localRotation = Quaternion.Euler(angle, 0f, 0f);
            if (rightFoot != null)
                rightFoot.localRotation = Quaternion.Euler(-angle, 0f, 0f);

            float bob = Mathf.Sin(stepTimer * bobbingFrequency / baseStepFrequency) * bobbingHeight;
            bodyTransform.localPosition = bodyStartPos + Vector3.up * bob;
        }
        else
        {
            stepTimer = 0f;
            if (leftFoot != null)
                leftFoot.localRotation = Quaternion.Slerp(leftFoot.localRotation, Quaternion.identity, Time.deltaTime * 5f);
            if (rightFoot != null)
                rightFoot.localRotation = Quaternion.Slerp(rightFoot.localRotation, Quaternion.identity, Time.deltaTime * 5f);
            bodyTransform.localPosition = Vector3.Lerp(bodyTransform.localPosition, bodyStartPos, Time.deltaTime * 5f);
        }
    }
}
