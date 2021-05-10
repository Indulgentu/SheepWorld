using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChange : MonoBehaviour
{
    [SerializeField]
    private GameObject TreeAutumn;
    [SerializeField]
    private GameObject TreeSummer;

    void FixedUpdate()
    {
        if(EnvController.CurrentSeason == EnvController.Season.AUTUMN || EnvController.CurrentSeason == EnvController.Season.WINTER)
        {
            if(tag == "SpecialFucker" && EnvController.CurrentSeason == EnvController.Season.AUTUMN)
            {
                return;
            }
            TreeSummer.SetActive(false);
            TreeAutumn.SetActive(true);
        }
        if(EnvController.CurrentSeason == EnvController.Season.SPRING || EnvController.CurrentSeason == EnvController.Season.SUMMER)
        {
            TreeSummer.SetActive(true);
            TreeAutumn.SetActive(false);
        }
    }
}
