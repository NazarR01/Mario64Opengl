using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderDamage : MonoBehaviour
{
   [SerializeField] private int damage = 1;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Asegúrate de que Mario tenga el tag "Player"
        {
            MarioHealt marioHealth = collision.gameObject.GetComponent<MarioHealt>();
            MarioInvulnerability marioInvul = collision.gameObject.GetComponent<MarioInvulnerability>();

            if (marioHealth != null && marioInvul != null && !marioInvul.IsInvulnerable())
            {
                marioHealth.ApplyDamage(damage);
                marioInvul.StartInvulnerability(3f);
            }
        }
    }
}
