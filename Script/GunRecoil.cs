using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GunRecoil : MonoBehaviour
{

    [HideInInspector] public Cinemachine.CinemachineFreeLook playerCamera;
    [HideInInspector] public Rifle rifle;
    [HideInInspector] public Pistol.Gun gun;


    Vector3 currentRotation, targetRotation, targetPosition, currentPosition, initialGunPosition;
    public Transform cam;

    [SerializeField] float recoilX;
    [SerializeField] float recoilY;
    [SerializeField] float recoilZ;

    [SerializeField]  float kickBackZ;

    public float snappiness, returnAmount;

    public void Start()
    {
        initialGunPosition = transform.localPosition;
        rifle = GameObject.FindGameObjectWithTag("Rifle").GetComponent<Rifle>();
        gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Pistol.Gun>();
    }

    public void Update()
    {
        /*targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * returnAmount);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, Time.fixedDeltaTime * snappiness);
        transform.localRotation = Quaternion.Euler(currentRotation);*/

        Back();

        if (rifle.isAim)
        {
            kickBackZ = 0.05f;
        }
        else if (gun.isAim)
        {
            kickBackZ = 0.2f;
        }
        else
        {
            kickBackZ = 0.5f;
        }
       
    }

    public void Recoil()
    {

        targetPosition -= new Vector3(0, 0, kickBackZ);
        targetRotation += new Vector3(recoilX, UnityEngine.Random.Range(-recoilY, recoilY), UnityEngine.Random.Range(-recoilZ, recoilZ));
        

    }

    private void Back()
    {
        targetPosition = Vector3.Lerp(targetPosition, initialGunPosition, Time.deltaTime * returnAmount);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * snappiness);
        transform.localPosition = currentPosition;
    }




}
