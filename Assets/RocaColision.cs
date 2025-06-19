using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocaColision : MonoBehaviour
{
   void OnCollisionEnter(Collision collision)
{
    Debug.Log("La esfera chocó con: " + collision.gameObject.name);
}
}
