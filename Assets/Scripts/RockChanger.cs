using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockChanger : MonoBehaviour
{
    [SerializeField]
    private GameObject WinterRock;
    [SerializeField]
    private GameObject SummerRock;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (EnvController.CurrentSeason == EnvController.Season.AUTUMN || EnvController.CurrentSeason == EnvController.Season.WINTER)
        {
            if (SummerRock.GetComponent<Rigidbody>() != null)
            {
                SummerRock.GetComponent<Rigidbody>().isKinematic = true;
            }
            if (SummerRock.GetComponent<MeshCollider>() != null)
            {
                SummerRock.GetComponent<MeshCollider>().enabled = false;
            }
            SummerRock.SetActive(false);
            WinterRock.SetActive(true);
        }
        if (EnvController.CurrentSeason == EnvController.Season.SPRING || EnvController.CurrentSeason == EnvController.Season.SUMMER)
        {
            SummerRock.SetActive(true);
            WinterRock.SetActive(false);
            if (SummerRock.GetComponent<Rigidbody>() != null)
            {
                SummerRock.GetComponent<Rigidbody>().isKinematic = false;
            }
            if (SummerRock.GetComponent<MeshCollider>() != null)
            {
                SummerRock.GetComponent<MeshCollider>().enabled = true;
            }

        }
    }
}
