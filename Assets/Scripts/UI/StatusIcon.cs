using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatusIcon : MonoBehaviour
{
    public IStatusEffect effect;
    public float maxTime = 10f;
    public Vector3 desiredPos;
    public BehaviourAdder behaviourAdder;
    //public Dictionary<string, > allBlocks = new Dictionary<string, BlockData>();
    void Start()
    {
        maxTime = effect.remainingDuraton;
        transform.GetChild(2).gameObject.GetComponent<Image>().color = effect.GetColor;
        desiredPos = this.transform.position;
        foreach (NameAndIcon ic in behaviourAdder.statusIcons)
        {
            if (ic.statusName == effect.GetName) //Add a default state maybe
            {
                transform.GetChild(0).gameObject.GetComponent<Image>().sprite = ic.statusIcon;
            }
        }
    }
    void Update()
    {
        if (effect.IsFinished)
        {
            Destroy(this.gameObject);
        }
        transform.GetChild(2).gameObject.GetComponent<Image>().fillAmount = effect.remainingDuraton/maxTime;
        this.transform.position += (desiredPos-this.transform.position)*Time.deltaTime*10f;
    }
}
