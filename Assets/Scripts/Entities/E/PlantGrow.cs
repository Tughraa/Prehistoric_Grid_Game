using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrow : MonoBehaviour
{
    public float growth = 6f;
    public BlockData plantBlock;
    public AllSystems allSystems;
    [SerializeField] float stepSize = 0.5f;
    public float impulseThresh = 1f;
    Rigidbody2D rigid;

    void Start()
    {
        //StartGrowth(new Vector3(-1f,0.5f,0f),6f,2f);
        rigid = this.GetComponent<Rigidbody2D>();
        allSystems = this.GetComponent<EntityGeneral>().allSystems;
    }

    public void StartGrowth(Vector3 direction, float growAmount, float totalTime)
    {
        rigid.simulated = false;
        StartCoroutine(GrowRoutine(direction,growAmount,totalTime));
    }
    IEnumerator GrowRoutine(Vector3 direction, float growAmount, float totalTime)
    {
        int totalSteps = Mathf.FloorToInt(growAmount/stepSize);
        Vector3 originPos = this.transform.position;
        for (int i = 0; i < totalSteps; i++)
        {
            //Place The block
            Vector3Int placeAt = allSystems.mapManager.FloatToGridPos(originPos + direction.normalized*i*stepSize);
            if (allSystems.mapManager.HasBlock(placeAt) == false) //if no block there already
            {
                allSystems.mapManager.PlaceBlock(placeAt,plantBlock); //place the block
            }
            //Do some waiting
            yield return new WaitForSeconds(totalTime/totalSteps);
        }
        AstarPath.active.Scan(); //REMOVE THIS IN THE FUTURE
        
        Destroy(this.gameObject);
    } 


    /*void LifeTimeManag()
    {
        currentLife += Time.deltaTime;
        if (currentLife > lifeTime)
        {   
            //Maybe particle?
            Destroy(this.gameObject);
        }
    }*/
    
    void OnCollisionEnter2D(Collision2D col)
    {
        float impulse = col.relativeVelocity.magnitude * rigid.mass;
        if (impulse > impulseThresh) 
        {
            StartGrowth(col.relativeVelocity,growth,2f);
        }
    }
}
