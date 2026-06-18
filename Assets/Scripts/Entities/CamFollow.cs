using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform toFollow;
    public float followSpeed = 5f;
    
    public float defCamSize;
    public Camera cam;

    public AnimationCurve zoomAnim;

    float zoomTime = 0f;
    float zoomTimeMax = 0f;
    float currentZoomAmount = 1f;

    //public float 
    void Start()
    {
        defCamSize = cam.orthographicSize;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            ChangeZoom(5f, 100f);
        }
    }
    void FixedUpdate()
    {
        Vector2 originPos = new Vector2(this.transform.position.x,this.transform.position.y);
        Vector2 followPos = new Vector2(toFollow.position.x,toFollow.position.y);
        Vector2 moveVec = originPos+ (followPos-originPos)*followSpeed/100f;
        this.transform.position = new Vector3(moveVec.x,moveVec.y,this.transform.position.z);

        //Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //camSize = Mathf.Clamp(toFollow.GetComponent<Rigidbody2D>().velocity.magnitude/5f,1f,20f);
        //Debug.Log(Vector2.Distance(toFollow.transform.position,mousePos));
        //cam.orthographicSize = camSize;
        
        //cam.orthographicSize = defCamSize;
        if (zoomTimeMax > zoomTime)
        {
            zoomTime += Time.deltaTime;
            float halfway = (zoomTimeMax+0.00001f)/2f;
            cam.orthographicSize = defCamSize + currentZoomAmount*(-(Mathf.Abs(zoomTime-halfway)-halfway)/halfway);
        }
        if (zoomTime > zoomTimeMax)
        {
            zoomTime = 0f;
            zoomTimeMax = 0f;
            cam.orthographicSize = defCamSize;
        }
    }

    public void ChangeZoom(float time, float amount)
    {
        zoomTimeMax = time;
        currentZoomAmount = amount;
    }
}
