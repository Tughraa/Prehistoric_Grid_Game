using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalikHareketi : MonoBehaviour
{
    public float yukselmeHizi = 0.001f;
    public float buymeHizi = 0.005f;
    void Start()
    {
        //this.transform.localScale = new Vector3(9f,1f,1f);
    }
    void Update()
    {
        this.transform.position = this.transform.position + new Vector3(0f,yukselmeHizi,0f);
        this.transform.localScale = this.transform.localScale + new Vector3(buymeHizi,0f,0f);
        if (Input.GetKeyDown(KeyCode.J))
        {
            BuyumeyiArttir();
        }
    }
    public void BuyumeyiArttir()
    {
        buymeHizi = buymeHizi +1;
    }
}
