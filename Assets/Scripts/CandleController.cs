using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Light LightSrc;

    void Start()
    {
        LightSrc = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if(EnvController.CurrentHour >= 17f || EnvController.CurrentHour >= 0f && EnvController.CurrentHour <= 6f)
        {
            LightSrc.intensity = 4f;
            LightSrc.range = 2.5f;
        }else if(EnvController.CurrentHour > 6f)
        {
            LightSrc.intensity = 1.94f;
            LightSrc.range = 1.33f;
        }
    }
}
