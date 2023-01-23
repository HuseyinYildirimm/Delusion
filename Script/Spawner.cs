using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawner;
    public Transform[] spawnPoints;

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
}
