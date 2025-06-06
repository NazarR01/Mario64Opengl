using UnityEngine;

public class PLemerg : MonoBehaviour
{
    public float velocidad = 1.5f;
    public float distanciaDeDeteccion = 10f;
    public float distanciaExplosion = 2f;
    public float cambioDireccionIntervalo = 3f;
    public string tagJugador = "Player";

    private Transform objetivo;
    private Vector3 direccionAleatoria;
    private float tiempoProximoCambio;

    void Start()
    {
        GameObject mario = GameObject.FindGameObjectWithTag(tagJugador);
        if (mario != null)
        {
            objetivo = mario.transform;
        }

        GenerarNuevaDireccion();
        tiempoProximoCambio = Time.time + cambioDireccionIntervalo;
    }

    void Update()
    {
        if (objetivo == null) return;

        float distancia = Vector3.Distance(transform.position, objetivo.position);

        if (distancia <= distanciaExplosion)
        {
            // "Explota"
            Debug.Log("¡La bomba explotó cerca de Mario!");
            Destroy(gameObject);
        }
        else if (distancia <= distanciaDeDeteccion)
        {
            // Persigue a Mario
            Vector3 direccion = (objetivo.position - transform.position).normalized;
            direccion.y = 0; // Mantener en plano horizontal

            transform.position += direccion * velocidad * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direccion);
        }
        else
        {
            // Movimiento aleatorio
            if (Time.time >= tiempoProximoCambio)
            {
                GenerarNuevaDireccion();
                tiempoProximoCambio = Time.time + cambioDireccionIntervalo;
            }

            Vector3 nuevaPosicion = transform.position + direccionAleatoria * velocidad * Time.deltaTime;
            nuevaPosicion.y = transform.position.y; // Mantener altura constante

            transform.position = nuevaPosicion;
            transform.rotation = Quaternion.LookRotation(direccionAleatoria);
        }
    }

    void GenerarNuevaDireccion()
    {
        direccionAleatoria = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }
}
