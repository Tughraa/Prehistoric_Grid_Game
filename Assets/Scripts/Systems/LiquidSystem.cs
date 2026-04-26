using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidSystem : MonoBehaviour
{
    public AllSystems allSystems;
    public MapManager mapManager;
    
    private HashSet<Vector3Int> activeLiquids = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> settledLiquids = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> dirtyPositions = new HashSet<Vector3Int>();

    private void Start()
    {
        mapManager.BlockPlaced += OnBlockPlaced;
        mapManager.BlockRemoved += OnBlockRemoved;
    }
    void Update()
    {
        Tick(Time.deltaTime);
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    public void Cleanup()
    {
        mapManager.BlockPlaced -= OnBlockPlaced;
        mapManager.BlockRemoved -= OnBlockRemoved;
    }

    void OnBlockPlaced(Vector3Int pos, BlockData data)
    {
        if (!mapManager.HasBlock(pos)) return;

        BlockState state = mapManager.GetBlock(pos);
        FlowBehaviour liquid = state.GetBehaviour<FlowBehaviour>();

        if (liquid != null)
        {
            activeLiquids.Add(pos);
            settledLiquids.Remove(pos);
            WakeNeighbors(pos);
        }
        else
        {
            // A non-liquid block was placed, wake liquid neighbors
            // since their flow paths may now be blocked
            WakeNeighbors(pos);
        }
    }

    void OnBlockRemoved(Vector3Int pos, BlockData data)
    {
        activeLiquids.Remove(pos);
        settledLiquids.Remove(pos);
        // A block was removed, wake liquid neighbors
        // since new flow paths may have opened up
        WakeNeighbors(pos);
    }

    public void Tick(float dt)
    {
        // Apply dirty flags from last frame, wake any settled liquids
        // that had a neighbor change
        foreach (Vector3Int pos in dirtyPositions)
        {
            if (settledLiquids.Contains(pos))
            {
                settledLiquids.Remove(pos);
                activeLiquids.Add(pos);
            }
        }
        dirtyPositions.Clear();

        // Sort bottom-up so lower blocks move first
        List<Vector3Int> toProcess = new List<Vector3Int>(activeLiquids);
        toProcess.Sort((a, b) => a.y.CompareTo(b.y));

        foreach (Vector3Int pos in toProcess)
        {
            if (!mapManager.HasBlock(pos))
            {
                // Block no longer exists, clean up
                activeLiquids.Remove(pos);
                continue;
            }

            FlowBehaviour liquid = mapManager.GetBlock(pos).GetBehaviour<FlowBehaviour>();

            if (liquid == null)
            {
                activeLiquids.Remove(pos);
                continue;
            }

            liquid.moveTimer += dt;

            if (liquid.moveTimer < liquid.movePeriod) continue;
            liquid.moveTimer = 0f;

            bool moved = TryFlow(pos, liquid);

            if (!moved)
            {
                // Didn't move, mark as settled
                activeLiquids.Remove(pos);
                settledLiquids.Add(pos);
            }
        }
    }

    bool TryFlow(Vector3Int pos, FlowBehaviour liquid)
    {
        Vector3Int down = pos + new Vector3Int(0, -1, 0);

        if (IsAvailable(down))
        {
            MoveLiquid(pos, down);
            return true;
        }

        bool rightBlocked = false;
        bool leftBlocked = false;

        for (int i = 1; i <= liquid.checkWide; i++)
        {
            if (!rightBlocked)
            {
                Vector3Int right = pos + new Vector3Int(i, 0, 0);
                if (!IsAvailable(right))
                {
                    rightBlocked = true;
                }
                else if (IsAvailable(right + new Vector3Int(0, -1, 0)))
                {
                    MoveLiquid(pos, pos + new Vector3Int(1, 0, 0));
                    return true;
                }
            }

            if (!leftBlocked)
            {
                Vector3Int left = pos + new Vector3Int(-i, 0, 0);
                if (!IsAvailable(left))
                {
                    leftBlocked = true;
                }
                else if (IsAvailable(left + new Vector3Int(0, -1, 0)))
                {
                    MoveLiquid(pos, pos + new Vector3Int(-1, 0, 0));
                    return true;
                }
            }

            if (rightBlocked && leftBlocked) break;
        }

        return false;
    }

    void MoveLiquid(Vector3Int from, Vector3Int to)
    {
        mapManager.MoveBlock(from, to);
        //'to' is now active, 'from' is now empty
        activeLiquids.Remove(from);
        settledLiquids.Remove(from);
        activeLiquids.Add(to);
        WakeNeighbors(from);
        WakeNeighbors(to);
    }

    bool IsAvailable(Vector3Int pos)
    {
        return !mapManager.HasBlock(pos);
    }

    void WakeNeighbors(Vector3Int pos)
    {
        dirtyPositions.Add(pos + new Vector3Int(0, 1, 0));
        dirtyPositions.Add(pos + new Vector3Int(0, -1, 0));
        dirtyPositions.Add(pos + new Vector3Int(1, 0, 0));
        dirtyPositions.Add(pos + new Vector3Int(-1, 0, 0));

        //Also diagonals
        dirtyPositions.Add(pos + new Vector3Int(1, 1, 0));
        dirtyPositions.Add(pos + new Vector3Int(1, -1, 0));
        dirtyPositions.Add(pos + new Vector3Int(-1, 1, 0));
        dirtyPositions.Add(pos + new Vector3Int(-1, -1, 0));
    }
}
