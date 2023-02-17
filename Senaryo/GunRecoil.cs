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
    [SerializeField] float aimRecoilX;
    [SerializeField] float aimRecoilY;
    [SerializeField] float aimRecoilZ;


    public float snappiness, returnAmount;

    public void Start()
    {
        initialGunPosition = transform.localPosition;
        rifle = GameObject.FindGameObjectWithTag("Rifle").GetComponent<Rifle>();
        gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Pistol.Gun>();
    }

    public void Update()
    {
   

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnAmount * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void Recoil()
    {
        
        if(!gun.isAim && !rifle.isAim) targetRotation += new Vector3(recoilX, UnityEngine.Random.Range(-recoilY, recoilY), UnityEngine.Random.Range(-recoilZ, recoilZ));

        else  targetRotation += new Vector3(aimRecoilX, UnityEngine.Random.Range(-aimRecoilY, aimRecoilY), UnityEngine.Random.Range(-aimRecoilZ, aimRecoilZ));

    }

   
}
