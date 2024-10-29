using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadDamage : MonoBehaviour, IDamageable
{
    [SerializeField] float damageMultiplier = 2f;

    [SerializeField] ARPlayer player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        player.TakeDamage(damage * damageMultiplier);
    }


}
