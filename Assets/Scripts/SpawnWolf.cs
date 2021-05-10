using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWolf : MonoBehaviour
{
    [SerializeField]
    private Transform[] Spawners;
    [SerializeField]
    private Transform ReferenceObj;

    private GameObject Copy;
    private bool HasSpawned = false;
    private System.Random r;

    void Start()
    {
        r = new System.Random();
    }

    void Update()
    {
        if ((EnvController.CurrentHour >= 17f || EnvController.CurrentHour >= 0f && EnvController.CurrentHour <= 6f) && !HasSpawned)
        {
            ReferenceObj.gameObject.SetActive(true);
            Copy = Instantiate(ReferenceObj, Spawners[r.Next(0, 4)]).gameObject;
            Copy.GetComponent<Wanderer>().enabled = true;
            HasSpawned = true;
            ReferenceObj.gameObject.SetActive(false);
        }
        else if (EnvController.CurrentHour > 6f && EnvController.CurrentHour < 17f && HasSpawned)
        {
            HasSpawned = false;
            Destroy(Copy);
        }
    }
}
