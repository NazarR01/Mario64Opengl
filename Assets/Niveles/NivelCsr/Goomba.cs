using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Goomba : MonoBehaviour
{
    public float speed = 2f;
    public float detectionRadius = 6f;
    public Transform mario;
    private Rigidbody rb;

    private bool persiguiendo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Evita que rote y se incline
    }

    void Update()
    {
        float distancia = Vector3.Distance(transform.position, mario.position);

        if (distancia <= detectionRadius)
        {
            persiguiendo = true;
        }
        else
        {
            persiguiendo = false;
        }

        if (persiguiendo)
        {
            Vector3 direccion = (mario.position - transform.position).normalized;
            direccion.y = 0f;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direccion), Time.deltaTime * 5f);
            rb.velocity = new Vector3(direccion.x * speed, rb.velocity.y, direccion.z * speed);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 marioPos = collision.transform.position;
            Vector3 goombaPos = transform.position;

            Rigidbody marioRb = collision.gameObject.GetComponent<Rigidbody>();

            if (marioRb != null)
            {
                bool vieneDesdeArriba = marioRb.velocity.y < -0.1f;
                bool estaEncima = marioPos.y > goombaPos.y + 0.5f;

                if (vieneDesdeArriba && estaEncima)
                {
                    marioRb.velocity = new Vector3(marioRb.velocity.x, 8f, marioRb.velocity.z); // Rebote
                    Muerte();
                }
                else
                {
                    // Si lo tocó de lado, hacé lo que quieras (daño, etc.)
                    Debug.Log("Mario fue golpeado por el costado");
                }
            }
        }
    }

    void Muerte()
    {
        Debug.Log("Goomba eliminado");
        Destroy(gameObject);
    }
}