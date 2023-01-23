using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RotatePlayer : MonoBehaviour
{
    public float Sensitivity = 80;

    public Transform playerBody;

    float xRotation = 0f;

    public float aimDuration = 0.3f;

    public Rig aimLayer;
    //public Rig TsLayer

    public Transform peekRight;
    public Transform peekLeft;
    public Transform ýdle;
   
    float rotateY;
    float rotateX;

    [HideInInspector] public WeaponDataHolder rigAnimation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        rigAnimation = GetComponentInChildren<WeaponDataHolder>();
    }


    void Update()
    {
         rotateY = Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;
         rotateX = Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;

        xRotation -= rotateY;

        xRotation = Math.Clamp(xRotation, -90f, 90f);
        
        playerBody.Rotate(Vector3.up * rotateX);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
      



        //WEAPON MOVE
        if (Input.GetMouseButton(3))
           {
               aimLayer.weight += Time.deltaTime / aimDuration;

           }
           else
           {
               aimLayer.weight -= Time.deltaTime / aimDuration;
           }

         /*  if (Input.GetMouseButton(1) )
           {
               TsLayer.weight += Time.deltaTime / aimDuration;
           }
           else
           {
               TsLayer.weight -= Time.deltaTime / aimDuration;
           }*/

    }






}

