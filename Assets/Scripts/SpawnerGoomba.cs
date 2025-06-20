using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerGoomba : MonoBehaviour
{
    public GameObject goombaPrefab;
    public Transform marioTransform;
    public float intervalo = 5f;
    public int maxGoombas = 5;

    private List<GameObject> goombasVivos = new List<GameObject>();

    void Start()
    {
        InvokeRepeating(nameof(SpawnGoomba), 1f, intervalo);
    }

    void SpawnGoomba()
    {
        // Eliminar goombas destruidos
        goombasVivos.RemoveAll(g => g == null);

        if (goombasVivos.Count >= maxGoombas)
            return;

        GameObject goomba = Instantiate(goombaPrefab, transform.position, Quaternion.identity);

        // Asignar Mario
        Goomba goombaScript = goomba.GetComponent<Goomba>();
        if (goombaScript != null)
            goombaScript.marioTransform = marioTransform;

        goombasVivos.Add(goomba);
    }
}
