using System;
using UnityEngine;
using LibSM64;

public class Camara : MonoBehaviour
{
    [SerializeField] GameObject target = null;
    [SerializeField] float radius = 15;
    [SerializeField] float elevation = 5;

    void LateUpdate()
    {

        Vector3 m = target.transform.position;

        Vector3 dir = transform.forward;

        Vector3 desiredPos = m - dir * radius;
        desiredPos.y = m.y + elevation;

        transform.position = desiredPos;

    }
}
