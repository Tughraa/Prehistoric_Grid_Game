using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform toFollow;
    public float followSpeed = 5f;
    
    public float camSize;
    public Camera cam;
    void Start()
    {
        camSize = cam.orthographicSize;
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
    }
}
