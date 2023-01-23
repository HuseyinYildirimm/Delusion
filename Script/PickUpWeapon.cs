using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;


public class PickUpWeapon : MonoBehaviour
{
    
    public WeaponName weaponGun;
     WeaponDataHolder weaponDataHolder;
    
    //Collider'a girdi�inde silah� als�n


    public void OnTriggerEnter(Collider other)
    {
        //weaponDataHolder = GetComponentInChildren<WeaponDataHolder>();
        weaponDataHolder = GameObject.Find("Man").transform.GetComponent<WeaponDataHolder>();
        if (other.tag == "Player")
        {
            if (weaponDataHolder)
            {
                WeaponName newWeapon = weaponGun;
                weaponDataHolder.Equip(newWeapon);

                Destroy(gameObject);
            }
        }
    }
}