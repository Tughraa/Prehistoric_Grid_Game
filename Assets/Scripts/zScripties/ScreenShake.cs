using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public AnimationCurve curvy;
    bool currentlyShaking = false;
    public void StartShaking(float time,float str)
    {
        if (!currentlyShaking)
        {
            StartCoroutine(ShakeIt(time,str));
        }
    }
    IEnumerator ShakeIt(float duration, float strength)
    {
        currentlyShaking = true;
        Vector3 startPos = this.transform.localPosition;
        float elapsed = 0f;
        while (elapsed<duration)
        {
            elapsed += Time.deltaTime;
            float curStr = curvy.Evaluate(elapsed/duration);
            this.transform.localPosition = startPos + Random.insideUnitSphere*strength;
            yield return null;
        }
        
        this.transform.localPosition = startPos;
        currentlyShaking = false;
    } 
}
