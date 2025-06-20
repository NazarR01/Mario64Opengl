using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibSM64;

[RequireComponent(typeof(SM64DynamicTerrain))]
[DisallowMultipleComponent]
public class Movimientos : MonoBehaviour
{
    [Header("Movimiento vertical")]
    public float amplitudMovimiento = 1.5f;     // Altura de subida y bajada
    public float velocidad = 1.0f;              // Velocidad del movimiento

    private SM64DynamicTerrain platform;
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    void Start()
    {
        posicionInicial = transform.position;
        platform = GetComponent<SM64DynamicTerrain>();
        rotacionInicial = transform.rotation;
    }

    void FixedUpdate()
    {
        float desplazamientoY = Mathf.Sin(Time.fixedTime * velocidad) * amplitudMovimiento;
        Vector3 nuevaPos = posicionInicial + Vector3.up * desplazamientoY;

        platform.SetPosition(nuevaPos);
        platform.SetRotation(rotacionInicial); // mantiene la rotación original
    }
}
