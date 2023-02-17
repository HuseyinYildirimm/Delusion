using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject spawner;
    public Transform[] spawnPoints;
    public Light directionLight;
    bool lightning = true;
    BoxCollider trigger;

    private void Start()
    {
        trigger = GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            trigger.enabled = false;
            SpawnZombies();
        }
    }
    public void Update()
    {
        /* if(lightning)
       StartCoroutine(Light(30f));*/
    }

    void SpawnZombies()
    {
        foreach (var spawn in spawnPoints)
        {
            Instantiate(spawner, spawn.position, spawn.rotation);
        }

        //  StartCoroutine(SpawnTime());
    }
    /* IEnumerator SpawnTime()
     {
         yield return new WaitForSeconds(1f);
     }*/

        /*  IEnumerator Light(float time)
          {
              lightning = false;
              directionLight.intensity = 5f;
              yield return new WaitForSeconds(0.3f);
              directionLight.intensity = 0f;
              yield return new WaitForSeconds(0.1f);
              directionLight.intensity = 5f;
              yield return new WaitForSeconds(0.1f);
              directionLight.intensity = 0f;

              yield return new WaitForSeconds(time);
              lightning = true;
          }*/
    }
