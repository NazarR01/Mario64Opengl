using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderSpawner : MonoBehaviour
{
   public GameObject boulderPrefab;
    public float intervalo = 5f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnBoulder), 1f, intervalo);
    }

    void SpawnBoulder()
    {
        Instantiate(boulderPrefab, transform.position, Quaternion.identity);
    }
}
