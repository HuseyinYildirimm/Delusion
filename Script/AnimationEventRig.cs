using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventRig : MonoBehaviour
{
    public AxWeapon.Ax AxEvent;


    public void Start()
    {
        AxEvent = GameObject.Find("WeaponPivot").GetComponent<AxWeapon.Ax>();
    }

    void AxeThroww()
    {
        AxEvent.AxeThrow();
    }

    void ResetAxee()
    {
        AxEvent.ResetAxe();
    }

    void AxeAttack()
    {
        AxEvent.AxAttack();
    }
   
}
