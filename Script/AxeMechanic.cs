using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AxWeapon
{
    public class AxeMechanic : MonoBehaviour
    {
        public bool activated;
        public float rSpeed;
        [SerializeField] Transform ShootPoint;

        RaycastHit raycastHit;
        Vector3 hitPoint;

        [SerializeField] public float range;
        [HideInInspector] public AxWeapon.Ax ax;
        [SerializeField] private float maxForce;
        [SerializeField] private float maxForceTime;

       


        public void Start()
        {
            ax = FindObjectOfType<AxWeapon.Ax>();
           

        }
        public void Update()
        {
            if (activated)
            {
                transform.Rotate(0f, 0f, -6.0f * rSpeed * Time.deltaTime);
            }
        }
        public void OnCollisionEnter(Collision collision)
        {
            activated = false;

            if (Physics.Raycast(ShootPoint.position, ShootPoint.forward, out raycastHit, range) )

            { 
                if (collision.transform.gameObject.tag == "Zombie" || collision.transform.gameObject.tag == "Head"
            || collision.transform.gameObject.tag == "TopBody" || collision.transform.gameObject.tag == "LowBody")

                {
                    ax.ReturnAxe();

                    ZombieAI zombieRagdoll = raycastHit.transform.GetComponentInParent<ZombieAI>();
                    ZombieHealth zHealth = raycastHit.transform.GetComponentInParent<ZombieHealth>();
                    if (!ax.isThrowAx)
                    {

                        zHealth.BulletDamage(100f);
                        GetComponent<Rigidbody>().isKinematic = false;
                        float mouseButtonDown = Time.time - 1;
                        float forcePercentage = mouseButtonDown / maxForceTime;
                        float forceMagnitude = Mathf.Lerp(1, maxForce, forcePercentage);

                        Vector3 forceDirection = zombieRagdoll.transform.position - transform.position;
                        forceDirection.y = 1;
                        forceDirection.Normalize();
                        Vector3 force = forceMagnitude * forceDirection;

                        zombieRagdoll.TriggerRagdoll(force, hitPoint);

                    }
                    else return;
                    
                }
                else GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }
}