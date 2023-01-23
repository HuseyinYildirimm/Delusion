using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace AxWeapon
{

    public class Ax : MonoBehaviour
    {
        [Header ("AXE")]
        public GameObject axe;
        public Collider col;
        [SerializeField] float damageAxZombie = 50f;
        private float time = 0.0f;
         float nextFire = 0f;
        [SerializeField] float rateOffire;

        [Header ("EFFECT")]
        public GameObject bloodEffect;

        Rigidbody rbAxe;
        AnimationEvent animEvent;

        [Header("AUDÝO")]
        [SerializeField] AudioSource axAS;
        [SerializeField] AudioClip axAC;


        [Header("THROW")]
        public Transform target;
        public Transform curvePoint;
        public Transform ShootPoint;
        private bool isReturning = false;
        private Vector3 oldPos;
        public bool isThrowAx;
        public bool isResetAx;
        public float throwPower = 200f;
        

        #region Raycast & Ragdoll
        RaycastHit ray;
        public float range;
        Vector3 hitPoint;
        [Header ("FORCE")]
        [SerializeField] private float maxForce;
        [SerializeField] private float maxForceTime;
        #endregion

        [HideInInspector] AxeMechanic axeMechanic;
        [HideInInspector] WeaponName pickUp;
        [HideInInspector] WeaponDataHolder weaponDataHolder;
        

        

        public void Start()
        {

            axeMechanic = GameObject.Find("Axe").GetComponent<AxeMechanic>();
            pickUp = GameObject.Find("Axe").GetComponent<WeaponName>();
            weaponDataHolder = GameObject.Find("Man").GetComponent<WeaponDataHolder>();

            axAS = GetComponent<AudioSource>();
            animEvent = new AnimationEvent();
            animEvent.functionName = "AxeThrow";
        }

        public void Update()
        {
 
            if (Input.GetButtonDown("Fire1") && weaponDataHolder.isAx)
            {
                if (Time.time > nextFire)
                {
                    axAS.PlayOneShot(axAC);
                    weaponDataHolder.rigController.Play("Ax_Attack");

                    nextFire = 0f;
                    
                    nextFire = Time.time + rateOffire;

                }
            }
            

            if (Input.GetKeyDown(KeyCode.C) && isThrowAx && weaponDataHolder.isAx)
            {
                weaponDataHolder.rigController.Play("AxThrow");
                weaponDataHolder.rigController.SetBool("Ax", false);
              
            }
            if (Input.GetMouseButton(1) && isResetAx)
            {
                ReturnAxe();
            }
            if (isReturning)
            {
                if (time < 1.0f)
                {
                    rbAxe.position = QuadraticBezierCurvePoint(time, oldPos, curvePoint.position, target.position);
                    rbAxe.rotation = Quaternion.Slerp(rbAxe.transform.rotation, target.rotation, 50 * Time.deltaTime);
                    time += Time.deltaTime;
                }
                else
                {
                    ResetAxe();
                }
            }
        }

        public void AxAttack()
        {
            col.isTrigger = true;

            if (Physics.Raycast(ShootPoint.position, ShootPoint.forward, out ray, range))
            {
                if (ray.transform.tag == "Zombie" || ray.transform.tag == "Head"
            || ray.transform.tag == "TopBody" || ray.transform.tag == "LowBody")
                {

                    ZombieAI zombieRagdoll = ray.collider.GetComponentInParent<ZombieAI>();
                    ZombieHealth zombieHealth = ray.transform.GetComponentInParent<ZombieHealth>();
                    GameObject bloodClone =  Instantiate(bloodEffect, ray.point, transform.rotation);

                    zombieRagdoll.ZombieDamageEffect();

                    if (zombieHealth.zombieHealth > 50f && zombieRagdoll != null)
                    {
                        zombieHealth.BulletDamage(damageAxZombie);
                    }
                    else
                    {
                        damageAxZombie = 50f;
                        zombieHealth.BulletDamage(damageAxZombie);
                        Instantiate(bloodEffect, ray.point, transform.rotation);
                        float mouseButtonDown = Time.time - 1;
                        float forcePercentage = mouseButtonDown / maxForceTime;
                        float forceMagnitude = Mathf.Lerp(1, maxForce, forcePercentage);

                        Vector3 forceDirection = zombieRagdoll.transform.position - transform.position;
                        forceDirection.y = 1;
                        forceDirection.Normalize();
                        Vector3 force = forceMagnitude * forceDirection;

                        zombieRagdoll.TriggerRagdoll(force, hitPoint);
                    }
                    Destroy(bloodClone, 2f);
                }
            }

        }



        public void AxeThrow()
        {
            weaponDataHolder.isAx = false;
            isThrowAx = false;
            isResetAx = true;
            col.isTrigger = false;
            axeMechanic.activated = true;
            isReturning = false;
            rbAxe.transform.parent = null;
            rbAxe.isKinematic = false;
            rbAxe.useGravity = true;
            rbAxe.AddForce(ShootPoint.transform.TransformDirection(Vector3.forward) * throwPower, ForceMode.Impulse);
            rbAxe.AddTorque(rbAxe.transform.TransformDirection(Vector3.right) * 100, ForceMode.Impulse);
        }

        public void ReturnAxe()
        {
            rbAxe.isKinematic = false;
            
            axeMechanic.activated = true;
            time = 0.0f;
            oldPos = rbAxe.position;
            isReturning = true;
            rbAxe.velocity = Vector3.zero;

            rbAxe.useGravity = false;

            // StartCoroutine(AxAnimWait());
            
        }

        public void ResetAxe()
        {
            weaponDataHolder.rigController.Play("AxeResetThrow");
            isReturning = false;
            rbAxe.useGravity = false;
            isResetAx = false;
            weaponDataHolder.isAx = true;
            rbAxe.transform.parent = this.transform;
            pickUp.transform.localRotation = Quaternion.Euler(-126, 0, -25);
            pickUp.transform.localPosition = new Vector3(0f, 0f, 0f);


            axeMechanic.activated = false;
            rbAxe.isKinematic = true;
            isThrowAx = true;

            weaponDataHolder.rigController.SetBool("Ax", true);
        }


        Vector3 QuadraticBezierCurvePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float v = 1 - t;
            float tt = t * t;
            float uu = v * v;
            Vector3 p = (uu * p0) + (2 * v * t * p1) + (tt * p2);
            return p;

        }


        public void AddRigidBody()
        {
            rbAxe = GameObject.Find("Axe").AddComponent<Rigidbody>();
            rbAxe.useGravity = false;
            rbAxe.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }
    }
}
