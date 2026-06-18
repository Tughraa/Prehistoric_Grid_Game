using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotemTemp : MonoBehaviour
{
    [SerializeField] Transform totemHalo;
    [SerializeField] SpriteRenderer totemBeams;
    Vector3 origin;
    float timer = 0f;
    float timer2 = 0f;
    [SerializeField] float sinAmount = 1f;
    [SerializeField] float haloSpeed = 2f;

    PlayerGeneral playerThatWon;
    public AllSystems allSystems;

    public Image dankeScreen;
    public Image dankeScreenText;
    Vector3 textOrigin;

    void Start()
    {
        origin = this.transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;
        this.transform.position = origin + new Vector3(0f,Mathf.Sin(timer)*sinAmount,0f);
        totemHalo.transform.Rotate(0f, 0f, Time.deltaTime*haloSpeed, Space.Self);

        //CircleCast physic
        if (playerThatWon == null)
        {
            CheckIfReached(8f);
        }
        else
        {
            timer2 += Time.deltaTime;
            dankeScreenText.transform.position = dankeScreen.transform.position +new Vector3(0f,Mathf.Sin(timer)*1f,0f);
            float transition = Mathf.Clamp(timer2/3f,0f,1f);
            dankeScreen.color = new Color(1f,1f,1f,transition);
            dankeScreenText.color = new Color(1f,1f,1f,transition);
        }
    }
    void CheckIfReached(float range)
    {
        Collider2D[] results = Physics2D.OverlapBoxAll(this.transform.position,new Vector2(range, range),0f);
        if (results.Length > 0)
        {
            foreach (var col in results)
            {    
                if (col.gameObject.GetComponent<PlayerGeneral>())
                {
                    //uhmm time for a win I guess
                    StartCoroutine(Win(col.gameObject.GetComponent<PlayerGeneral>()));
                }
            }
        }
    }
    IEnumerator Win(PlayerGeneral player) //Add totem lights and all
    {
        Debug.Log("you won yippeee");

        allSystems.timeSystem.ChangeTimeScale(0.8f);
        playerThatWon = player;
        player.Announce("DU HAST DAS TOTEM ERREICHT",4f,new Color(0.3f,1f,0.2f,1f));
        haloSpeed *= 1.5f;
        totemBeams.gameObject.SetActive(true);

        yield return new WaitForSeconds(4f);

        //player.GetComponent<EntityStatusEffects>().AddEffect(new FloatEffect(duration: 80f,strength: 0.2f));
        player.GetComponent<EntityStatusEffects>().AddEffect(new HealingEffect(duration: 80f,strength: 0.5f,period: 0.75f));
        yield return new WaitForSeconds(2f);
        dankeScreen.gameObject.SetActive(true);
        timer2 = 0f;
        //Something new
    }
}
