using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockLibrary : MonoBehaviour
{
    public AllSystems allSystems;
    public Dictionary<string, BlockData> allBlocks = new Dictionary<string, BlockData>();
    public List<BlockData> allBlocksAssign;

    void Awake()
    {
        foreach (BlockData idata in allBlocksAssign) {
            allBlocks[idata.id] = idata;
        }
    }
}
