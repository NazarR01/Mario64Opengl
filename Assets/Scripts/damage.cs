using LibSM64;
using UnityEngine;


public class damage : MonoBehaviour
{
    public int marioId = 0;
    public int damageType = 0;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by {other.name}, tag: {other.tag}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Mario detected! Applying damage.");
            Interop.sm64_mario_apply_damage(marioId, damageType);
        }
    }
}





