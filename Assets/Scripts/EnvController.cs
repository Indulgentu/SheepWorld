using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvController : MonoBehaviour
{
    [SerializeField]
    private DayNight controller;
    [SerializeField]
    private AudioSource EnvSource;
    [SerializeField]
    private AudioClip NightSfx;
    [SerializeField]
    private AudioClip DaySfx;
    [SerializeField]
    private AudioSource MusicSrc;
    [SerializeField]
    private AudioClip NightMusic;
    [SerializeField]
    private AudioClip DayMusic;
    [SerializeField]
    private int Chances = 0;
    [SerializeField]
    private Transform Snow;
    [SerializeField]
    private Transform Rain;

    private float[] AvgT = { -1f, 10f, 20f, 9f };
    public static float CurrentTemperature;
    public static float CurrentHour;
    public static float CurrentMinute;
    public static int CurrentMonth = 1;
    public static int CurrentDay = 1;
    public static int CurrentYear = 1998;
    public static int ScaleOfTime = 1;
    public static int SheepsAlive = 0;
    public static Season CurrentSeason = Season.WINTER;
    public static Dictionary<Needs.Need, Transform> LastKnownPlaces = new Dictionary<Needs.Need, Transform>();
    public static List<string> KnownPredators = new List<string>();
    public static List<Vector3> SafePlaces = new List<Vector3>();
    public static Weather CurrentWeather;

    public enum Season
    {
        WINTER, SPRING, SUMMER, AUTUMN
    }

    public enum Weather
    {
        CLEAR, CLOUDY, RAIN, SNOW
    }

    void Update()
    {
        Time.timeScale = ScaleOfTime;
        CurrentHour = 24 * controller.currentTimeOfDay;
        CurrentMinute = 60 * (CurrentHour - Mathf.Floor(CurrentHour));
        if(CurrentDay >= 30)
        {
            CurrentMonth++;
            CurrentDay = 1;
            ChangeSeason();
        }
        if(CurrentMonth > 12)
        {
            CurrentYear++;
            CurrentMonth = 1;
        }
    }

    void FixedUpdate()
    {
        ChangeMusic();
        if ((int)CurrentHour % 4 == 0 && (int)CurrentMinute == 0)
        {
            ChangeTemperature();
            Precipitation();
        }
    }

    public void ChangeMusic()
    {
        if (CurrentHour >= 17f || CurrentHour >= 0f && CurrentHour <= 6f)
        {
            if (EnvSource.clip != NightSfx)// && MusicSrc.clip != NightMusic)
            {
                EnvSource.clip = NightSfx;
                EnvSource.volume = 0.8f;
                //MusicSrc.clip = NightMusic;
                EnvSource.Play();
                //MusicSrc.Play();
            }
        }
        else if (CurrentHour > 6f)
        {
            if (EnvSource.clip != DaySfx)// && MusicSrc.clip != DayMusic)
            {
                EnvSource.clip = DaySfx;
                EnvSource.volume = 0.9f;
                //MusicSrc.clip = DayMusic;
                EnvSource.Play();
                //MusicSrc.Play();
            }
        }
    }

    public void ChangeSeason()
    {
        if(CurrentMonth < 3 || CurrentMonth == 12)
        {
            CurrentSeason = Season.WINTER;
        }
        if(CurrentMonth >= 3 && CurrentMonth <= 5)
        {
            CurrentSeason = Season.SPRING;
        }
        if(CurrentMonth >= 6 && CurrentMonth <= 8)
        {
            CurrentSeason = Season.SUMMER;
        }
        if(CurrentMonth >= 9 && CurrentMonth <= 11)
        {
            CurrentSeason = Season.AUTUMN;
        }
    }

    public void ChangeTemperature()
    {
        float HourModifier = (CurrentHour >= 17f || CurrentHour >= 0f && CurrentHour <= 6f) ? Random.Range(-10f, -2f) : Random.Range(0f, 3f);
        CurrentTemperature = Random.Range(-1f, 1f) + AvgT[(int)CurrentSeason] + HourModifier;
    }

    public void Precipitation()
    {
        Chances += (CurrentSeason == Season.WINTER ? Random.Range(-5, 5) : 0) +
                   (CurrentSeason == Season.SPRING ? Random.Range(-2, 10) : 0) +
                   (CurrentSeason == Season.SUMMER ? Random.Range(-1, 20) : 0) +
                   (CurrentSeason == Season.AUTUMN ? Random.Range(-1, 15) : 0);
        Chances = Chances < 0 ? 0 : Chances;
        Chances = Chances > 100 ? 100 : Chances;
        Chances -= (CurrentWeather == Weather.RAIN || CurrentWeather == Weather.SNOW) ? 20 : 0;

        if(Chances >= 25)
        {
            if(Mathf.Floor(CurrentTemperature) <= 0)
            {
                if (Rain.gameObject.activeSelf)
                {
                    Rain.gameObject.SetActive(false);
                }
                Snow.gameObject.SetActive(true);
                CurrentWeather = Weather.SNOW;
            }
            else
            {
                if (Snow.gameObject.activeSelf)
                {
                    Snow.gameObject.SetActive(false);
                }
                Rain.gameObject.SetActive(true);
                CurrentWeather = Weather.RAIN;
            }
        }
        if (Chances >= 10 && Chances < 25)
        {
            Snow.gameObject.SetActive(false);
            Rain.gameObject.SetActive(false);
            CurrentWeather = Weather.CLOUDY;
        }
        else if(Chances < 10)
        {
            Snow.gameObject.SetActive(false);
            Rain.gameObject.SetActive(false);
            CurrentWeather = Weather.CLEAR;
        }

    }
}
