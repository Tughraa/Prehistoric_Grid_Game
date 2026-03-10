using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class GodsHands : MonoBehaviour
{
    public bool godIsReal = true;

    public MapManager mapManager;

    public ExplosionSystem explosionSystem;

    public BlockData[] blocksAtHand;
    public int blockPref = 0;

    public TMP_Text blockNameIndic;

    void Start()
    {
        blockNameIndic.text = blocksAtHand[blockPref].id;
    }
    public void MakeGodReal(bool real)
    {
        godIsReal = real;
        blockNameIndic.gameObject.SetActive(real);
    }

    void Update()
    {
        if (godIsReal)
        {
            ChangeSlots();
            
            if (Input.GetMouseButtonDown(0))
            {
                //TileBase clickedTile = blockLayer.GetTile(gridPos);
                mapManager.PlaceBlock(MouseGridPos(),blocksAtHand[blockPref]);

                //Debug.Log("found: "+ tilesToBlocks[clickedTile].id);
            }
            if (Input.GetMouseButtonDown(1))
            {
                mapManager.RemoveBlock(MouseGridPos(),false);
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                Vector3Int mousePos = MouseGridPos();
                if (mapManager.HasBlock(mousePos))
                {
                    mapManager.GetBlock(mousePos).BlockBreak(1f,mapManager);
                }
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                explosionSystem.ExplodeSimple(mousePos,5.3f,0.001f,null);
            }
        }
    }
    public void ChangeSlots()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && Input.GetKey(KeyCode.LeftShift))
        {
            BlockPrefSwitch(-1);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && Input.GetKey(KeyCode.LeftShift))
        {
            BlockPrefSwitch(1);
        }
    }
    public void BlockPrefSwitch(int changeDir)
    {
        blockPref += changeDir;
        if (blockPref >= blocksAtHand.Length)
        {
            blockPref = 0;
        }
        else if (blockPref<0)
        {
            blockPref = blocksAtHand.Length-1;
        }
        blockNameIndic.text = blocksAtHand[blockPref].blockName;
        
    }
    Vector3Int MouseGridPos()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPos = mapManager.blockLayer.WorldToCell(mousePos);
        return gridPos;
    }
}
