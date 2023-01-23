using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;


public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public static PlayerHealth singleton;
    public float currentHealth;
    public float maxHealth = 400f;
    public Image healthBar;
    float lerpspeed;
    public GameObject medicine;
  

    RaycastHit raycastHit;
    [SerializeField] Transform Cam;
    public PostProcessProfile DSP;
    

    public float distance = 10f;

    public bool isDead = false;

    void Start()
    {
        singleton = this;
        currentHealth = maxHealth;

       
        

    }

    private void Update()
    {
        HealthBarFiller();
        ColorChange();

        if (currentHealth > maxHealth) currentHealth = maxHealth;

        lerpspeed = 3f * Time.deltaTime;

        if (currentHealth <= 75f)
        {
            DSP.GetSetting<ChromaticAberration>().intensity.value = 0.5f;
            DSP.GetSetting<Bloom>().intensity.value = 5f;
            DSP.GetSetting<Bloom>().color.value = Color.red;
         
            if (currentHealth <= 50f)
            {
                DSP.GetSetting<Bloom>().intensity.value = 50f;
                DSP.GetSetting<ColorGrading>().colorFilter.overrideState = true;

                if (currentHealth <= 25f)
                {
                    DSP.GetSetting<Bloom>().intensity.value = 65f;
                }
            }


        }
        else
        {
            DSP.GetSetting<ChromaticAberration>().intensity.value = 0;
            DSP.GetSetting<Bloom>().intensity.value = 0;
          
            DSP.GetSetting<ColorGrading>().colorFilter.overrideState = false;
        }


        #region MEDÝCÝNE
        if (Input.GetKeyDown(KeyCode.E))
        {
           
            if (Physics.Raycast(Cam.position, Cam.forward, out raycastHit, distance))
            {
                if (raycastHit.transform.tag == "Medicine")
                {
                    Heal();
                }
            }
        }
        #endregion
    }
    public void DamagePlayer(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            Cam.transform.DOPunchPosition(new Vector3(.5f, 0), 1, 10);
        }
        else
        {
            PlayerDead();
        }
  
        //Debug.Log("PlayerHealth" + currentHealth);

    }


    public void PlayerDead()
    {
        currentHealth = 0;
        isDead = true;
    }

    void HealthBarFiller()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, lerpspeed);
    }

    void ColorChange()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (currentHealth / maxHealth));
        healthBar.color = healthColor;

    }

    public void Heal()
    {    
        medicine.SetActive(false);
        if (currentHealth < maxHealth)
        {
            currentHealth += 100f;
        }
    }
}
