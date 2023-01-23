using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public bool isEntry;
    public LayerMask targetMask;
    public LayerMask enemyMask;

    public List<Transform> visibleTargets = new List<Transform>();


    public void Start()
    {
        StartCoroutine("FindTargetWithDelay", .2f);
    }


    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    public void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            Transform target = targetInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward ,dirToTarget)<viewAngle /2)
            {
                float distanceTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position , dirToTarget , distanceTarget , enemyMask))
                {
                    visibleTargets.Add(target);

                    if (visibleTargets.Count == 1f)
                    {
                        isEntry = true;
                        
                    }
                    else isEntry = false;
                }
            }
        }
    }


    public Vector3 DirFromAngle(float angleInDegress , bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegress += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegress * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegress * Mathf.Deg2Rad));
    }
}
