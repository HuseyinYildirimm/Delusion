using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;


public class StaminaController : MonoBehaviour
{

    [HideInInspector] public MovePlayer moveplayer;
    [HideInInspector] public Pistol.Gun run;

    [SerializeField] public bool isShift = false;
    [SerializeField] public bool isRegenera = true;
    [SerializeField] public float maxFill = 100f;
    [SerializeField] public float playerstamina = 100f;
    [SerializeField] public Camera cam;
    [SerializeField] private PostProcessVolume volume;
    private ChromaticAberration chromatic;


    [SerializeField] private Image staminaRun = null;
    public float staminaSpeed = 15f;


    public void Start()
    {
        moveplayer = GetComponent<MovePlayer>();
        run =GameObject.FindGameObjectWithTag("Gun"). GetComponentInChildren<Pistol.Gun>();
    }
   


    public void Update()
    {
        if (playerstamina <= maxFill - 0.01)
        {
            playerstamina += 5 *Time.deltaTime;
            UpdateStamina();

            if (playerstamina >= maxFill || !isShift)
            {
               
                DOTween.To(() => moveplayer.Speed, x => moveplayer.Speed = x, 15, 1);
              //  DOTween.To(() => chromatic.intensity.value, x => chromatic.intensity.value = x, 0, 2);
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
            playerstamina -= .2f ;
            UpdateStamina();
            DOTween.To(() => moveplayer.Speed, x => moveplayer.Speed = x, staminaSpeed, 1);
            //DOTween.To(() => chromatic.intensity.value, x => chromatic.intensity.value = x, 0.5f, 2);
            cam.DOFieldOfView(68, 2);
            moveplayer.Run();
             
            if (playerstamina <= 0 )
            {
                isShift = false;
                DOTween.To(() => moveplayer.Speed, x => moveplayer.Speed = x, 15, 1);
             // DOTween.To(() => chromatic.intensity.value, x => chromatic.intensity.value = x, 0, 2);
                cam.DOFieldOfView(60, 2);
            }
            if (!isShift)
            {
                Debug.Log("xd");
                DOTween.To(() => moveplayer.Speed, x => moveplayer.Speed = x, 15, 1);
               // DOTween.To(() => chromatic.intensity.value, x => chromatic.intensity.value = x, 0, 2);
                cam.DOFieldOfView(60, 2);
            }

        }
    }

    void UpdateStamina()
    {
        staminaRun.fillAmount = playerstamina / maxFill ;
    }


}
