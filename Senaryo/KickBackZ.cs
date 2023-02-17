using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickBackZ : MonoBehaviour
{

    [SerializeField] float kickBackZ;
    Vector3 targetPosition, currentPosition , initialGunPosition , currentRotation;
    public Transform cam;

    [HideInInspector] public Rifle rifle;
    [HideInInspector] public Pistol.Gun gun;


    public float snappiness, returnAmount;
    public void Start()
    {
        initialGunPosition = transform.localPosition;
        rifle = GameObject.FindGameObjectWithTag("Rifle").GetComponent<Rifle>();
        gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Pistol.Gun>();
    }

    void Update()
    {
        KickBack();
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
    public void KickBack()
    {
        targetPosition = Vector3.Lerp(targetPosition, initialGunPosition, Time.deltaTime * returnAmount);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * snappiness);
        transform.localPosition = currentPosition;
    }

    public void Back()
    {
       
        targetPosition -= new Vector3(0, 0, kickBackZ);
    }
}
