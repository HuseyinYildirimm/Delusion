using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{

    public int selectedWeapon = 2;
    public bool isOne;
    public bool isTwo;
    private float nextSwitch;
    public float switchDelay = 0.1f;

    [HideInInspector] public WeaponDataHolder weaponDataHolder;
    [HideInInspector] public WeaponName pickUp;
    [HideInInspector] public AxWeapon.Ax axThrowCase;
  
    public void Start()
    {
        weaponDataHolder = GameObject.Find("Man").GetComponent<WeaponDataHolder>();
        pickUp = GetComponent<WeaponName>();
        axThrowCase = GetComponent<AxWeapon.Ax>();
      
    }


    void Update()
    {

        int previousSelectedWeapon = selectedWeapon;
        if (!axThrowCase.isResetAx)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (selectedWeapon >= transform.childCount - 1)
                    selectedWeapon = 2;
                else
                    selectedWeapon++;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (selectedWeapon <= 2)
                    selectedWeapon = transform.childCount - 1;
                else
                    selectedWeapon--;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectedWeapon = 2;

            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
            {
                selectedWeapon = 3;

            }

            if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
            {
                selectedWeapon = 4;

            }

            if (previousSelectedWeapon != selectedWeapon && Time.time > nextSwitch)
            {
                nextSwitch = Time.time + switchDelay;
                selectWeapon();
            }
        }
    }
    void selectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {

                weapon.gameObject.SetActive(true);

                if (weapon.gameObject.name == ("Gun Variant"))
                {
                   
                    weaponDataHolder.rigController.Play("equip_pistol");
                    weaponDataHolder.isAx = false;
                    weaponDataHolder.isGun = true;
                    weaponDataHolder.rigController.SetBool("Ax", false);
                    weaponDataHolder.rigController.SetBool("Gun", true);
                    weaponDataHolder.rigController.SetBool("Rifle", false);
                }
                if (weapon.gameObject.name == ("Axe"))
                {
                   
                    weaponDataHolder.rigController.Play("equip_ax");
                    weaponDataHolder.isGun = false;
                    weaponDataHolder.isAx = true;
                    weaponDataHolder.rigController.SetBool("Ax", true);
                    weaponDataHolder.rigController.SetBool("Gun", false);
                    weaponDataHolder.rigController.SetBool("Rifle", false);
                }
                if (weapon.gameObject.name == ("VintageRifle"))
                {
                  
                    weaponDataHolder.rigController.Play("equip_rifle");
                    weaponDataHolder.isRifle = true;
                    weaponDataHolder.isAx = false;
                    weaponDataHolder.isGun = false;
                    weaponDataHolder.rigController.SetBool("Rifle", true);
                    weaponDataHolder.rigController.SetBool("Gun", false);
                    weaponDataHolder.rigController.SetBool("Ax", false);

                }
                weaponDataHolder.rigController.SetBool("PistolRun", false);
                weaponDataHolder.rigController.SetBool("AxRun", false);
                weaponDataHolder.rigController.SetBool("RifleRun", false);

            }
            else if (i != 0 && i != 1)
            {
                weapon.gameObject.SetActive(false);
                
            }

            i++;
        }
    }

    
}