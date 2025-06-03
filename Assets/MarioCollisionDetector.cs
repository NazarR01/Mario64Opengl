using UnityEngine;
using LibSM64;

[RequireComponent(typeof(SM64Mario))]
[DisallowMultipleComponent]
public class MarioCollisionDetector : MonoBehaviour
{
    [Header("Configuración del Collider de Mario")]
    public float colliderRadius = 0.5f;
    public float colliderHeight = 1.8f;
    public float colliderYOffset = 0.9f;

    [Header("Filtro de colisiones")]
    public string enemyTag = "Enemy";

    [Header("Vidas de Mario")]
    public int maxVidas = 3;

    private Collider marioTrigger;
    private int vidasRestantes;
    private bool estaMuerto = false;
    private SM64Mario sm64Mario;

    void Awake()
    {
        Collider existing = GetComponent<Collider>();
        if (existing == null)
        {
            CapsuleCollider cc = gameObject.AddComponent<CapsuleCollider>();
            cc.isTrigger = true;
            cc.radius = colliderRadius;
            cc.height = colliderHeight;
            cc.center = new Vector3(0f, colliderYOffset, 0f);
            cc.direction = 1;
            marioTrigger = cc;
        }
        else
        {
            existing.isTrigger = true;
            if (existing is CapsuleCollider cc)
            {
                cc.radius = colliderRadius;
                cc.height = colliderHeight;
                cc.center = new Vector3(0f, colliderYOffset, 0f);
                cc.direction = 1;
            }
            else if (existing is BoxCollider bc)
            {
                bc.size = new Vector3(colliderRadius * 2f, colliderHeight, colliderRadius * 2f);
                bc.center = new Vector3(0f, colliderYOffset, 0f);
            }
            else if (existing is SphereCollider sc)
            {
                sc.radius = colliderRadius;
                sc.center = new Vector3(0f, colliderYOffset, 0f);
            }
            marioTrigger = existing;
        }
    }

    void Start()
    {
        vidasRestantes = maxVidas;
        sm64Mario = GetComponent<SM64Mario>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (estaMuerto) return;
        if (other.CompareTag(enemyTag))
        {
            OnMarioHitEnemy(other);
        }
    }

    protected virtual void OnMarioHitEnemy(Collider enemyCollider)
    {
        if (estaMuerto) return;
        vidasRestantes--;
        Debug.Log($"Mario ha sido golpeado por {enemyCollider.name}. Vidas restantes: {vidasRestantes}");
        if (vidasRestantes <= 0)
        {
            estaMuerto = true;



            Debug.Log("Mario ha muerto. Reinicia la escena para volver a jugar.");
        }
    }
}
