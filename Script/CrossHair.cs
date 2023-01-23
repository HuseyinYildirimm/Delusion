using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour
{
    public GameObject crosshair;
    public Image[] colorCrosshair;
    [SerializeField] Transform ShootPoint;
    float range = 150f;
    RaycastHit raycastHit;
    void Start()
    {

    }

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
                foreach (Image ui in colorCrosshair)
                {
                    ui.GetComponent<Image>().color = Color.white;
                }
            }

            else
            {
                foreach (Image ui in colorCrosshair)
                {
                    ui.GetComponent<Image>().color = Color.red;
                }
            }
        }

    }
}
