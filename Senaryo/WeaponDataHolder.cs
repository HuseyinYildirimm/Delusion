using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponDataHolder : MonoBehaviour
{
    public UnityEngine.Animations.Rigging.Rig handIK;
    

    public Transform weaponParent;
    public Transform weaponLeftGrip;
    public Transform weaponRightGrip;

    public Animator rigController;

    [SerializeField] Transform ShootPoint;
    [HideInInspector] public WeaponName pickUp;
    [HideInInspector] public MovePlayer movePlayer;
    [HideInInspector] public Pistol.Gun gun;
    [HideInInspector] public AxWeapon.AxeMechanic axMechanic;
    [HideInInspector] public AxWeapon.Ax ax;
    [HideInInspector] public WeaponSwitcher weaponSwitch;
    [HideInInspector] public StaminaController stamina;
    [HideInInspector] public Rifle rifle;

    public bool isGun = false;
    public bool isAx = false;
    public bool isJump = false;
    public bool isThrow = false;
    public bool isRifle = false;


    public void Start()
    {
        RigBuilder rigBuilder = GetComponent<RigBuilder>();
        rigBuilder.Build();

        weaponSwitch = GameObject.Find("WeaponPivot").GetComponent<WeaponSwitcher>();
        ax = GameObject.Find("WeaponPivot").GetComponent<AxWeapon.Ax>();
        movePlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<MovePlayer>();
        gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Pistol.Gun>();
        rifle = GameObject.FindGameObjectWithTag("Rifle").GetComponent<Rifle>(); 
        stamina = GameObject.FindGameObjectWithTag("Player").GetComponent<StaminaController>();

        rifle.enabled = false;
        gun.enabled = false;
        ax.enabled = false;

        WeaponName existingWeapon = GameObject.Find("Man").GetComponent<WeaponName>();
        if (existingWeapon)
            Equip(existingWeapon);

    }


    public void Equip(WeaponName newWeapon)
    {
        if (pickUp)
        {
            pickUp.gameObject.SetActive(false);
          

        }
        pickUp = newWeapon;
        pickUp.transform.parent = weaponParent;
        pickUp.transform.localPosition = Vector3.zero;


        if (pickUp.gameObject.name == ("Gun Variant"))
        {
            gun.enabled = true;
            Debug.Log("Pistol");
            isGun = true;
            isAx = false;
            isRifle = false;
            pickUp.transform.localRotation = Quaternion.Euler(0, 180, 0);
            pickUp.transform.localScale = new Vector3(0.045f, 0.045f, 0.045f);
            rigController.SetBool("Gun", true);
            rigController.SetBool("Ax", false);
            rigController.SetBool("Rifle", false);
            

        }
        if (pickUp.gameObject.name == ("Axe"))
        {
            Debug.Log("Axe");
            isAx = true;
            isGun = false;
            isRifle = false;
            pickUp.transform.localRotation = Quaternion.Euler(-126, 0, -25);
            pickUp.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);

            rigController.SetBool("Ax", true);
            rigController.SetBool("Gun", false);
            rigController.SetBool("Rifle", false);
            ax.AddRigidBody();
            ax.enabled = true;

            gun.crossHair.SetActive(false);

        }
         if (pickUp.gameObject.name == ("VintageRifle"))
        {
            Debug.Log("rifle");
            isRifle = true;
            isAx = false;
            isGun = false;
            pickUp.transform.localRotation = Quaternion.Euler(-1.6f, -68.86f, 16.88f);

            rigController.SetBool("Rifle", true);
            rigController.SetBool("Ax", false);
            rigController.SetBool("Gun", false);
            rifle.enabled = true;
        }

       

        rigController.Play("equip_" + pickUp.weaponName);
    }

    public void Update()
    {


        #region Pistol
        if (isGun)
        {

            if (movePlayer.isMoving && Input.GetKeyDown(KeyCode.LeftShift))
            {
                rigController.SetBool("PistolRun", true);

            }
            if (Input.GetKeyUp(KeyCode.LeftShift) || !movePlayer.isMoving || stamina.playerstamina <= 0)
            {
                rigController.SetBool("PistolRun", false);
            }

        }

        #endregion

        #region Rifle

        if (isRifle)
        {
            if (movePlayer.isMoving && Input.GetKeyDown(KeyCode.LeftShift))
            {
                rigController.SetBool("RifleRun", true);

            }
            if (Input.GetKeyUp(KeyCode.LeftShift) || !movePlayer.isMoving || stamina.playerstamina <= 0)
            {
                rigController.SetBool("RifleRun", false);
            }
        }


        #endregion

        #region Ax
        if (isAx)
        {
            if (movePlayer.isMoving && Input.GetKeyDown(KeyCode.LeftShift))
            {
                rigController.SetBool("AxRun", true);
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) || !movePlayer.isMoving || stamina.playerstamina <= 0)
            {
                rigController.SetBool("AxRun", false);
            }

        }
        #endregion

        #region Jump
        if (Input.GetKeyDown(KeyCode.Space) || !movePlayer.isGrounded)
        {

            isJump = true;
            rigController.SetBool("Jump", true);
            rigController.SetBool("PistolRun", false);
            rigController.SetBool("AxRun", false);
            rigController.SetBool("RifleRun", false);
        }
        else
        {
            isJump = false;
            rigController.SetBool("Jump", false);
        }
        #endregion

        #region Crouch

        if (Input.GetKeyDown(KeyCode.LeftControl))
            rigController.SetBool("Crouch", true);
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            rigController.SetBool("Crouch", false);
        }

        #endregion

        #region WeaponSwitch


        #endregion
    }


    /* [ContextMenu ("Save Weapon Pose")]
     public void SaveWeaponPose()
     {
         GameObjectRecorder recorder = new GameObjectRecorder(gameObject);
         recorder.BindComponentsOfType<Transform>(weaponParent.gameObject, false);
         recorder.BindComponentsOfType<Transform>(weaponLeftGrip.gameObject, false);
         recorder.BindComponentsOfType<Transform>(weaponRightGrip.gameObject, false);
         recorder.BindComponentsOfType<Transform>(gameObject, false);
         UnityEditor.AssetDatabase.SaveAssets();
         recorder.TakeSnapshot(0.0f);
         recorder.SaveToClip(pickUp.animClip);

     }*/

}
