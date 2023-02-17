using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StaminaController : MonoBehaviour
{

    [HideInInspector] public MovePlayer moveplayer;
    [HideInInspector] public Pistol.Gun run;

    [SerializeField] public bool isShift = false;
    [SerializeField] public bool isRegenera = true;
    [SerializeField] public float maxFill = 100f;
    [SerializeField] public float playerstamina = 100f;
    [SerializeField] public Camera cam;
    float staminaEnergy;


    [SerializeField] private Image[] staminaRun = null;
    public float staminaSpeed = 15f;


    public void Start()
    {
        moveplayer = GetComponent<MovePlayer>();
        run = GameObject.FindGameObjectWithTag("Gun").GetComponentInChildren<Pistol.Gun>();

        
    }



    public void Update()
    {
         staminaEnergy = playerstamina / maxFill;

        if (playerstamina <= maxFill - 0.01)
        {
            playerstamina += 5 *Time.deltaTime;
            UpdateStamina();

            if (playerstamina >= maxFill || !isShift)
            {
                DOTween.To(() => moveplayer.Speed, x => moveplayer.Speed = x, 15, 1);
            
                cam.DOFieldOfView(60, 2);
                isRegenera = true;

                if (moveplayer.isCrouch)
                {
                    DOTween.To(() => moveplayer.Speed, x => moveplayer.Speed = x, 10, 1);
                }

            }
        }
    }

    public void Sprinting()
    {
        
        if (isRegenera)
        {
            
            isShift = true;
            playerstamina -= .2f;
            UpdateStamina();

            DOTween.To(() => moveplayer.Speed, x => moveplayer.Speed = x, staminaSpeed, 1);
            cam.DOFieldOfView(68, 2);
            moveplayer.Run();
             
            if (playerstamina <= 0 )
            {
                isShift = false;
                DOTween.To(() => moveplayer.Speed, x => moveplayer.Speed = x, 15, 1);
                cam.DOFieldOfView(60, 2);
            }
            if (!isShift)
            {
                DOTween.To(() => moveplayer.Speed, x => moveplayer.Speed = x, 15, 1);
                cam.DOFieldOfView(60, 2);
            }

        }
    }

    void UpdateStamina()
    {
        for (int i = 0; i < staminaRun.Length; i++)
        {
            float fillAmount = 0f;
            if (staminaEnergy >= 0.20f * (i + 1))
            {
                fillAmount = 1f;
            }
            else if (staminaEnergy > 0.20f * i)
            {
                fillAmount = (staminaEnergy % 0.20f) * 5f;
            }
            staminaRun[staminaRun.Length - 1 - i].fillAmount = fillAmount;
        }
    }
}
