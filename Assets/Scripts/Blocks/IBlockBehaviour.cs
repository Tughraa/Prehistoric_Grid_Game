using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlockBehaviour
{
    void OnPlaced(MapManager map, Vector3Int pos, BlockState state);
    void OnRemoved(MapManager map, Vector3Int pos, BlockState state);
    void OnBreak(MapManager map, Vector3Int pos, BlockState state);
    void Tick(MapManager map, Vector3Int pos, BlockState state, float dt);
}
