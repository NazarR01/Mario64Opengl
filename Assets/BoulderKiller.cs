using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderKiller : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boulder"))
        {
            Destroy(other.gameObject);
        }
    }
}
