using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour
{
    
    [SerializeField] Transform ShootPoint;
   
    float range = 150f;
    RaycastHit raycastHit;

    public GameObject crossHair;
    public Image[] colorCrosshair;


    // Update is called once per frame
    void Update()
    {
        CrossColorChange();
    }

    void CrossColorChange()
    {
        if (Physics.Raycast(ShootPoint.position, ShootPoint.forward, out raycastHit, range))
        {

            if (raycastHit.transform.tag == "Zombie" || raycastHit.transform.tag == "Head" || raycastHit.transform.tag == "TopBody"
                || raycastHit.transform.tag == "LowBody")
            {
                ZombieAI zombie = raycastHit.collider.GetComponentInParent<ZombieAI>();

                for (int i = 0; i < colorCrosshair.Length; i++)
                {
                    Image ui = colorCrosshair[i];
                    ui.GetComponent<Image>().color = Color.white;
                }
            }

            else
            {
                 for (int i = 0; i < colorCrosshair.Length; i++)
                 {
                     Image ui = colorCrosshair[i];
                     ui.GetComponent<Image>().color = Color.red;
                 }
            }
        }

    }
}
