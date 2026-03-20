using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSystem : MonoBehaviour
{
    public float timeScale = 1f;
    void Start()
    {
        ChangeTimeScale(0.9f);
    }

    
    public void ChangeTimeScale(float newTime)
    {
        timeScale = newTime;
        Time.timeScale = newTime;
        //Time.fixedDeltaTime = newTime;
    }
}
