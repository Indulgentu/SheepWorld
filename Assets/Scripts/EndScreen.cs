using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject Panel;
    [SerializeField]
    private Text Stats;
    private static int SimCount = 0;

    void Update()
    {
        if(EnvController.SheepsAlive <= 0)
        {
            System.IO.File.AppendAllText(@"C:\Users\indul\Desktop\Stats.txt", "10, " + SimCount + ": " + ((EnvController.CurrentMonth > 1) ? (EnvController.CurrentMonth - 1) * 30 : (EnvController.CurrentDay)) + System.Environment.NewLine);
            //Panel.SetActive(true);
            //Stats.text = "Your sheeps survived for " + (EnvController.CurrentYear - 1998) + " years.";
            EnvController.ScaleOfTime = 0;
            SimCount++;
            PressBtn();
        }
    }

    public void PressBtn()
    {
        EnvController.ScaleOfTime = 15;
        EnvController.CurrentDay = 1;
        EnvController.CurrentMonth = 1;
        EnvController.CurrentYear = 1998;
        EnvController.LastKnownPlaces.Clear();
        EnvController.KnownPredators.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
