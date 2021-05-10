using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DayNight : MonoBehaviour
{

    public Light sun;
    public float secondsInFullDay = 120f;
    [Range(0, 1)]
    public float currentTimeOfDay = 0;
    [HideInInspector]
    public float timeMultiplier = 1f;

    float sunInitialIntensity;

    void Start()
    {
        sunInitialIntensity = sun.intensity;
        sun = GetComponent<Light>();
    }

    void Update()
    {
        UpdateSun();

        currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * timeMultiplier;

        if (currentTimeOfDay >= 1)
        {
            EnvController.CurrentDay++;
            currentTimeOfDay = 0;
        }
    }

    void UpdateSun()
    {
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0);

        float intensityMultiplier = 1;
        if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f)
        {
            intensityMultiplier = 0.1f;
        }
        else if (currentTimeOfDay <= 0.25f)
        {
            intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
        }
        else if (currentTimeOfDay >= 0.73f)
        {
            intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f)));
        }
        switch (EnvController.CurrentWeather)
        {
             case EnvController.Weather.CLEAR:
                if(EnvController.CurrentSeason == EnvController.Season.WINTER)
                {
                    sun.color = new Color32(255, 255, 255, 255);
                    sun.shadowStrength = 0.656f;
                }
                if(EnvController.CurrentSeason == EnvController.Season.SPRING)
                {
                    sun.color = new Color32(228, 255, 168, 255);
                    sun.shadowStrength = 0.35f;
                }
                if(EnvController.CurrentSeason == EnvController.Season.SUMMER)
                {
                    sun.color = new Color32(255, 205, 71, 255);
                    sun.shadowStrength = 0.656f;
                }
                if (EnvController.CurrentSeason == EnvController.Season.AUTUMN)
                {
                    sun.color = new Color32(255, 173, 71, 255);
                    sun.shadowStrength = 0.556f;
                }
                break;
            case EnvController.Weather.CLOUDY:
                if(EnvController.CurrentSeason == EnvController.Season.WINTER)
                {
                    sun.color = new Color32(176, 176, 176, 255);
                }
                if (EnvController.CurrentSeason == EnvController.Season.SUMMER)
                {
                    sun.color = new Color32(219, 176, 61, 255);
                }
                if (EnvController.CurrentSeason == EnvController.Season.AUTUMN)
                {
                    sun.color = new Color32(222, 152, 64, 255);
                }
                if (EnvController.CurrentSeason == EnvController.Season.SPRING)
                {
                    sun.color = new Color32(179, 200, 131, 255);
                }
                sun.shadowStrength = 0.45f;
                break;
            case EnvController.Weather.RAIN:
                sun.color = new Color32(164, 164, 164, 255);
                sun.shadowStrength = 0.35f;
                break;
            case EnvController.Weather.SNOW:
                sun.color = new Color32(164, 164, 164, 255);
                sun.shadowStrength = 0.35f;
                break;
            default: break;
        }
        sun.intensity = sunInitialIntensity * intensityMultiplier;
    }
}
