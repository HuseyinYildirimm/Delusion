using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public float zombieHealth = 100f;

    ZombieAI zombieAI;
    [HideInInspector] AxWeapon.Ax aX;

    [HideInInspector] AudioManager audioManager;
 


    public bool isZombieDead;
    // public Collider[] colliders;

    public GameObject soulEffect;

    private void Start()
    {
        aX = FindObjectOfType<AxWeapon.Ax>();
        zombieAI = GetComponent<ZombieAI>();
      
        audioManager =GameObject.Find("AudioManager"). GetComponent<AudioManager>();

    }
    public void BulletDamage(float bulletDamage)
    {
        if (!isZombieDead)
        {
            zombieHealth -= bulletDamage;

            if (zombieHealth <= 0)
            {
                zombieHealth = 0;
                ZombieDeath();
            }
        }
    }
    public void ZombieDeath()
    {
        isZombieDead = true;
        zombieAI.ZombieDeathAnim();
        zombieAI.agent.speed = 0f;

        audioManager.Play("Soul");

        var cloneSoul = Instantiate(soulEffect, transform.position, transform.rotation);
        Destroy(cloneSoul , 2f);
        Destroy(gameObject, 5);

        
    }

   
}
