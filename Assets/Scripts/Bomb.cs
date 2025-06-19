using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float velocidad = 1.5f;
    public float distanciaDeDeteccion = 10f;
    public float distanciaDeExplosion = 2f; // distancia para explotar
    public float radioMovimiento = 3f; // rango para movimiento random
    public string tagJugador = "Player";

    public GameObject prefabExplosion; // Prefab de la explosión 

    private Transform objetivo;
    private Vector3 posicionInicial;
    private Vector3 destinoRandom;

    void Start()
    {
        GameObject mario = GameObject.FindGameObjectWithTag(tagJugador);
        if (mario != null)
        {
            objetivo = mario.transform;
        }

        posicionInicial = transform.position;
        destinoRandom = GetNuevoDestinoRandom();
    }

    void Update()
    {
        if (objetivo == null) return;

        float distancia = Vector3.Distance(transform.position, objetivo.position);

        // Explota si está muy cerca
        if (distancia <= distanciaDeExplosion)
        {
            Explote();
            return;
        }

        if (distancia <= distanciaDeDeteccion)
        {
            // Seguir a Mario
            Vector3 direccion = (objetivo.position - transform.position).normalized;
            direccion.y = 0;

            transform.position += direccion * velocidad * Time.deltaTime;

            if (direccion != Vector3.zero)
            {
                Vector3 direccionPlano = new Vector3(direccion.x, 0, direccion.z);
                transform.rotation = Quaternion.LookRotation(direccionPlano);
            }
        }
        else
        {
            // Movimiento random dentro del radio limitado
            float distanciaDestino = Vector3.Distance(transform.position, destinoRandom);

            if (distanciaDestino < 0.1f)
            {
                destinoRandom = GetNuevoDestinoRandom();
            }

            Vector3 direccionRandom = (destinoRandom - transform.position).normalized;
            direccionRandom.y = 0;

            transform.position += direccionRandom * velocidad * Time.deltaTime;

            if (direccionRandom != Vector3.zero)
            {
                Vector3 direccionPlano = new Vector3(direccionRandom.x, 0, direccionRandom.z);
                transform.rotation = Quaternion.LookRotation(direccionPlano);
            }
        }
    }

    Vector3 GetNuevoDestinoRandom()
    {
        Vector2 randomPos = Random.insideUnitCircle * radioMovimiento;
        Vector3 nuevoDestino = posicionInicial + new Vector3(randomPos.x, 0, randomPos.y);
        return nuevoDestino;
    }

    void Explote()
    {
        if (prefabExplosion != null)
        {
            Instantiate(prefabExplosion, transform.position, Quaternion.identity);
        }

        // Destruye la bomba
        Destroy(gameObject);
    }
}
