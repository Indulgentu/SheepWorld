using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGround : MonoBehaviour
{
    [SerializeField]
    private Material AutumnGrass;
    [SerializeField]
    private Material WinterGrass;
    [SerializeField]
    private Material SummerGrass;

    void FixedUpdate()
    {
        if (EnvController.CurrentSeason == EnvController.Season.AUTUMN)
        {
            gameObject.GetComponent<MeshRenderer>().material = AutumnGrass;
        }
        if (EnvController.CurrentSeason == EnvController.Season.WINTER)
        {
            gameObject.GetComponent<MeshRenderer>().material = WinterGrass;
        }
        if (EnvController.CurrentSeason == EnvController.Season.SPRING || EnvController.CurrentSeason == EnvController.Season.SUMMER)
        {
            gameObject.GetComponent<MeshRenderer>().material = SummerGrass;
        }

    }
}
