using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibSM64;

[RequireComponent(typeof(SM64DynamicTerrain))]
[DisallowMultipleComponent]
public class MovimientoObj : MonoBehaviour
{
     [Header("Ejes de Movimiento")]
    public bool moverEnX = false;
    public bool moverEnY = false;
    public bool moverEnZ = true;

    [Header("Dirección Inicial")]
    public bool xIniciaIzquierda = true;
    public bool yIniciaArriba = true;

    [Header("Parámetros del Movimiento")]
    public float amplitudMovimiento = 5.0f;
    public float velocidad = 1.0f;

    private SM64DynamicTerrain platform;
    private Vector3 ogPos;
    private Quaternion initialRotation;

    void Start()
    {
        ogPos = transform.position;
        platform = GetComponent<SM64DynamicTerrain>();
        initialRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        Vector3 offset = Vector3.zero;
        float tiempo = Time.fixedTime * velocidad;

        // Movimiento en X
        if (moverEnX)
        {
            float directionX = xIniciaIzquierda ? -1f : 1f;
            offset += Vector3.right * directionX * Mathf.Cos(tiempo) * amplitudMovimiento;
        }

        // Movimiento en Y
        if (moverEnY)
        {
            float directionY = yIniciaArriba ? 1f : -1f;
            offset += Vector3.up * directionY * Mathf.Cos(tiempo) * amplitudMovimiento;
        }

        // Movimiento en Z (normal, sin dirección inicial)
        if (moverEnZ)
        {
            offset += Vector3.forward * Mathf.Cos(tiempo) * amplitudMovimiento;
        }

        platform.SetPosition(ogPos + offset);
        platform.SetRotation(initialRotation); // sin rotación
    }
}
