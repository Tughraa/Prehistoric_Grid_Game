using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLookout : MonoBehaviour
{
    public bool introPlayed = false;
    [SerializeField] GameObject camAgentFab;
    GameObject camAgent;
    CamFollow camFollow;
    float animTimer = 0f;
    public float animTimerMax = 10f;
    public Transform playerTransform;
    [SerializeField] Transform gameEnd;
    
    float savedCamSpeed;

    void Update()
    {
        if (animTimer > 0f)
        {
            animTimer -= Time.deltaTime;
            float time = animTimer/animTimerMax;
            if (time > 0.75f)//At the start zoom out from the middle
            {
                camAgent.transform.position = new Vector3(75f,-70f,0f);
            }
            if (time <= 0.75f && time > 0.50f)//Zoom into the totem
            {
                camFollow.toFollow = gameEnd;
                camFollow.followSpeed = 7f;
                //Totem anims?
            }
            if (time > 0f && time <= 0.50f)//zoom back out as you go through the map
            {
                camFollow.toFollow = camAgent.transform;
                camFollow.ChangeZoom(animTimerMax*0.5f, 12f);
                float xLerp = Vector3.Lerp(playerTransform.position, gameEnd.position, time*2f).x;
                Debug.Log("sin is: "+(1+Mathf.Sin(((time-0.25f)*2f+1.5f)*Mathf.PI))+ "\nsinput is: "+((time-0.25f)*2f+1.5f)*Mathf.PI);
                camAgent.transform.position = new Vector3(xLerp,-130+ (1+Mathf.Sin(((time-0.25f)*2f+1.5f)*Mathf.PI)) *100f,0f);
            }
        }
        if (animTimer < 0f) //come back to the player
        {
            animTimer = 0f;
            camFollow.toFollow = playerTransform;
            Destroy(camAgent);
            camFollow.followSpeed = savedCamSpeed;
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<PlayerGeneral>() && introPlayed == false)
        {
            introPlayed = true;
            StartAnims(col.transform);
        }
    }
    void StartAnims(Transform player)
    {
        camAgent = Instantiate(camAgentFab,this.transform.position,Quaternion.identity);
        camFollow = player.GetComponent<PlayerGeneral>().playerCam.transform.parent.GetComponent<CamFollow>();
        playerTransform = player;
        camFollow.toFollow = camAgent.transform;
        savedCamSpeed = camFollow.followSpeed;

        Debug.Log(camFollow);
        camFollow.ChangeZoom(animTimerMax*0.4f, 110f);
        animTimer = animTimerMax;
        camFollow.followSpeed = 2f;
    }   
}
