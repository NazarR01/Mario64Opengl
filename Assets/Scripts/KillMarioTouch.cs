using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillMarioTouch : MonoBehaviour
{
    [SerializeField] private MarioHealt marioHealt;

    private void OnTriggerEnter(Collider other)
    {
        // Solo mata a Mario si toca un objeto con tag "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("Mario tocó zona peligrosa, Game Over!");
            if (marioHealt != null)
            {
                marioHealt.SetHealthZero();
            }
        }
    }
}
