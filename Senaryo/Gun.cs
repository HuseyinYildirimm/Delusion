using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


namespace Pistol
{

    public class Gun : MonoBehaviour
    {

        [SerializeField] float damageZombie = 25f;
        [SerializeField] Transform ShootPoint;
        [SerializeField] float range;
        [SerializeField] float rateOffire;
        float nextFire = 0f;
        public bool isAim = false;
        GameObject bloodClone;
    
        #region Audio
        [Header("Audio")]
        public AudioSource gunAS;
        public AudioClip shootAC;
        public AudioSource reloadAS;
        public AudioClip reloadAC;
        #endregion

        #region Ammo
        [Header("Ammo")]
        public Text currentAmmotext;
        public Text carriedAmmotext;
        public int currentAmmo = 12;
        public int maxAmmo = 12;
        public int carriedAmmo = 60;
        public bool isReloading;
        public bool isShoot = true;
        public bool shootRay = false;
        #endregion

        #region Effect
        [Header("Effect")]
        public GameObject MuzzleFlash;
        public GameObject bloodEffect;
        public GameObject bulletEffect;
        public GameObject bulletHole;
        [SerializeField] private TrailRenderer Bullettrail;
        public GameObject TrailBulletPos;
        #endregion

        #region Cam
        /*  Camera mainCam;
         private int normalView = 60;
         private int zoomView = 30;
         private float smoothZoom = 1.5f;*/
        #endregion

        #region Force

        [Header("Force")]
        [SerializeField] private float maxForce;
        [SerializeField] private float maxForceTime;
        #endregion


        RaycastHit raycastHit;
        Animator anim;

        [HideInInspector] public StaminaController speedRun;
        [HideInInspector] public MovePlayer move;
        [HideInInspector] public WeaponDataHolder weaponDataHolder;
        [HideInInspector] GunRecoil recoil;
        [HideInInspector] KickBackZ kick;
        SphereCollider sphere;

        public GameObject crossHair;
        public Image[] colorCrosshair;

        void Start()
        {
            gunAS = GetComponent<AudioSource>();
            anim = GetComponent<Animator>();
            speedRun = GameObject.Find("Player").GetComponent<StaminaController>();
            move = GameObject.Find("Player").GetComponent<MovePlayer>();
            weaponDataHolder = GameObject.Find("Man").GetComponent<WeaponDataHolder>();
            recoil = GameObject.Find("CameraRecoil").GetComponentInParent<GunRecoil>();
            kick = GameObject.Find("WeaponHolder").GetComponentInParent<KickBackZ>();
          

            reloadAS.Stop();
            gunAS.volume = 0f;
            UpdateAmmoUI();
            MuzzleFlash.SetActive(false);
        }
        void Update()
        {
            #region AÝM
            if (Input.GetButtonDown("Fire2"))
            {

                weaponDataHolder.rigController.SetBool("Aim", true);

                isAim = true;

                crossHair.SetActive(false);



                if (isAim)
                {
                    move.isRunning = true;
                    speedRun.isShift = false;
                    move.SetRunSpeed(15f);
                }
            }
            if (isAim)
            {

                if (Input.GetButtonUp("Fire2"))
                {

                    weaponDataHolder.rigController.SetBool("Aim", false);


                    isAim = false;
                    crossHair.SetActive(true);

                }
            }
            #endregion

            #region SHOOT

            if (Input.GetButtonDown("Fire1") && currentAmmo > 0 && isShoot)
            {
                Shoot();
            }
            else if (weaponDataHolder.isGun)
            {

                weaponDataHolder.rigController.SetBool("AimShoot", false);
            }
            else if (weaponDataHolder.isRifle)
            {
                //aimshoot rifle false
            }

            if (Input.GetButtonDown("Fire1") && currentAmmo <= 0)
            {
                DryFire();

            }
            if (Input.GetKeyDown(KeyCode.R))
            {

                Reload();
            }
            #endregion
        }

        void Shoot()
        {
            if (Time.time > nextFire)
            {
                if (isShoot)
                {
                    nextFire = 0;

                    nextFire = Time.time + rateOffire;

                    currentAmmo--;

                    MuzzleFlash.SetActive(true);

                    ShootRay();
                    recoil.Recoil();
                    kick.Back();
                    UpdateAmmoUI();
                    if (isAim && shootRay)
                    {
                        if (weaponDataHolder.isGun)
                        {
                            weaponDataHolder.rigController.SetBool("AimShoot", true);
                        }
                        else if (weaponDataHolder.isRifle)
                        {
                            //aimshoot rifle true
                        }

                    }
                    else if (!isAim && shootRay)
                    {
                        if (weaponDataHolder.isGun)
                        {
                            weaponDataHolder.isRifle = false;
                            weaponDataHolder.rigController.Play("Pistol_Shoot");

                        }
                        if (weaponDataHolder.isRifle)
                        {
                            weaponDataHolder.isGun = false;
                            weaponDataHolder.rigController.Play("Rifle_Shoot");
                        }
                    }
                    if (weaponDataHolder.isGun)
                    {
                        weaponDataHolder.rigController.Play("Slide_Trigger_Hammer");
                    }

                }
            }
        }

        void ShootRay()
        {
            StartCoroutine(GunSoundAndMuzzleflash());
            if (Physics.Raycast(ShootPoint.position, ShootPoint.forward, out raycastHit, range))
            {
                
                TrailRenderer trail = Instantiate(Bullettrail, TrailBulletPos.transform.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, raycastHit));

                shootRay = true;

                if (raycastHit.transform.tag == "Head")
                {

                    damageZombie = 100f;
                    ZombieHealth zombieHealthScript = raycastHit.transform.GetComponentInParent<ZombieHealth>();
                    zombieHealthScript.BulletDamage(damageZombie);

                    bloodClone =Instantiate(bloodEffect, raycastHit.point, transform.rotation);//blood

                    ZombieAI zombie = raycastHit.collider.GetComponentInParent<ZombieAI>();


                    if (zombie != null && zombieHealthScript.zombieHealth <=0f)
                    {
                        #region RagdollForce
                        float mouseButtonDown = Time.time - 1;
                        float forcePercentage = mouseButtonDown / maxForceTime;
                        float forceMagnitude = Mathf.Lerp(1, maxForce, forcePercentage);

                        Vector3 forceDirection = zombie.transform.position - transform.position;
                        forceDirection.y = 1;
                        forceDirection.Normalize();
                        Vector3 force = forceMagnitude * forceDirection;
                        #endregion

                        zombie.TriggerRagdoll(force, raycastHit.point);
                    }

                }
                else if (raycastHit.transform.tag == "TopBody")
                {
                    damageZombie = 50f;

                    ZombieHealth zombieHealthScript = raycastHit.transform.GetComponentInParent<ZombieHealth>();
                    zombieHealthScript.BulletDamage(damageZombie);
                    bloodClone =Instantiate(bloodEffect, raycastHit.point, transform.rotation);//blood
                    ZombieAI zombie = raycastHit.collider.GetComponentInParent<ZombieAI>();



                    if (zombie != null && zombieHealthScript.zombieHealth <= 0f)
                    {
                        #region RagdollForce
                        float mouseButtonDown = Time.time - 1;
                        float forcePercentage = mouseButtonDown / maxForceTime;
                        float forceMagnitude = Mathf.Lerp(1, maxForce, forcePercentage);

                        Vector3 forceDirection = zombie.transform.position - transform.position;
                        forceDirection.y = 1;
                        forceDirection.Normalize();
                        Vector3 force = forceMagnitude * forceDirection;
                        #endregion

                        zombie.TriggerRagdoll(force, raycastHit.point);
                    }

                }
                else if (raycastHit.transform.tag == "LowBody")
                {

                    damageZombie = 25f;
                    ZombieHealth zombieHealthScript = raycastHit.transform.GetComponentInParent<ZombieHealth>();
                    zombieHealthScript.BulletDamage(damageZombie);
                    bloodClone = Instantiate(bloodEffect, raycastHit.point, transform.rotation);//blood               
                    
                    ZombieAI zombie = raycastHit.collider.GetComponentInParent<ZombieAI>();

                    if (zombie != null && zombieHealthScript.zombieHealth <= 0f)
                    {
                        #region RagdollForce
                        float mouseButtonDown = Time.time - 1;
                        float forcePercentage = mouseButtonDown / maxForceTime;
                        float forceMagnitude = Mathf.Lerp(1, maxForce, forcePercentage);

                        Vector3 forceDirection = zombie.transform.position - transform.position;
                        forceDirection.y = 1;
                        forceDirection.Normalize();
                        Vector3 force = forceMagnitude * forceDirection;
                        #endregion

                        zombie.TriggerRagdoll(force, raycastHit.point);
                    }
                }
                else
                {

                    Instantiate(bulletEffect, raycastHit.point, Quaternion.LookRotation(raycastHit.normal));//bullet
                }
                Destroy(bloodClone, 20f);
             
            }
        }

        void DryFire() //kuru ateþ
        {
            if (Time.time > nextFire)
            {
                isShoot = false;
                nextFire = 0;
                nextFire = Time.time + rateOffire;
            }
        }

        void Reload()
        {
            if (!isReloading && currentAmmo != 12)
            {
               
                isShoot = false;
            
                if (!isAim)
                {
                    crossHair.SetActive(true);
                }
                if (weaponDataHolder.isGun)
                {
                    weaponDataHolder.rigController.Play("Pistol_Reload");
                    weaponDataHolder.rigController.Play("Reload_Magazine");
                }
             

                reloadAS.PlayOneShot(reloadAC);
                if (carriedAmmo <= 0) return;

                StartCoroutine(ReloadCountdown(1.2f));
            }

        }

        void UpdateAmmoUI()
        {
            currentAmmotext.text = currentAmmo.ToString();
            carriedAmmotext.text = carriedAmmo.ToString();
        }

        IEnumerator ReloadCountdown(float timer)
        {
            while (timer > 0f)
            {
                isReloading = true;
                timer -= Time.deltaTime;
                yield return null;

            }
            if (timer <= 0f)
            {
                int bulletsNeededToFillMag = maxAmmo - currentAmmo;
                int bulletsToDeduct = (carriedAmmo >= bulletsNeededToFillMag) ? bulletsNeededToFillMag : carriedAmmo;

                carriedAmmo -= bulletsToDeduct;
                currentAmmo += bulletsToDeduct;

                isReloading = false;
                UpdateAmmoUI();
                isShoot = true;

            }
        }

        IEnumerator GunSoundAndMuzzleflash()
        {
            gunAS.volume = 0.5f;
            gunAS.PlayOneShot(shootAC);
            yield return new WaitForSeconds(0.25f);
            MuzzleFlash.SetActive(false);
        }

        private IEnumerator SpawnTrail(TrailRenderer Trail, RaycastHit Hit)
        {
            float time = 0f;

            Vector3 startPosition = Trail.transform.position;

            while (time < 1)
            {
                Trail.transform.position = Vector3.Lerp(startPosition, Hit.point, time);

                time += Time.deltaTime / Trail.time;

                yield return null;
            }

            Trail.transform.position = Hit.point;


        }

      
    }
}
