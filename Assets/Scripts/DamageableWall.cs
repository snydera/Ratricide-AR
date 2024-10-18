using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableWall : MonoBehaviour, IDamageable
{
    public void TakeDamage(float damage)
    {
        Debug.Log("Wall took damage: " + damage);
        // Implement your destruction logic here, for example:
        Destroy(gameObject); // Destroy the wall on damage
    }
}
