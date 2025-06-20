using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsKiller : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boulder"))
        {
            Destroy(other.gameObject);
        }
         Goomba goomba = other.GetComponent<Goomba>();
        if (goomba != null)
        {
            Destroy(goomba.gameObject);
        }
    }

    
}
