using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameSystem;

public sealed class PlaceableWater : PlaceableGround
{
    private void HandleWaterMerge(bool isOn)
    {
        if (isOn)   SplitAndShrinkWaterBlocksInARow();
        else        CombineAndStretchWaterBlocksInARow();
    }

    private void CombineAndStretchWaterBlocksInARow()
    {
        if (!IsTheLeftmostWaterBlock()) return;

        // 1) Combine by deactivating every water block on the right.
        int blockCount = ToggleNextWaterBlockActivation(isOn: false);

        // 2) Strtch
        Strech(blockCount);
    }

    private void Strech(int size)
    {
        int x = Position.x + (size - 1) / 2;

        ActualObject.transform.position = new Vector3(x, Position.y);
        ActualObject.transform.localScale = new Vector3(3, 1, size);
    }

    private void SplitAndShrinkWaterBlocksInARow()
    {
        if (!IsTheLeftmostWaterBlock()) return;

        int blockCount = ToggleNextWaterBlockActivation(isOn: true);
        Shrink(blockCount);
    }

    private void Shrink(int size)
    {
        ActualObject.transform.position = new Vector3(Position.x, Position.y);
        ActualObject.transform.localScale = new Vector3(3, 1, 1);
    }

    // Return: the number of water blocks.
    private int ToggleNextWaterBlockActivation(bool isOn)
    {
        var nextPosition = new Vector2Int(Position.x + (int)MovementDirection.Right, Position.y);

        if (!tile.ContainsKey(nextPosition) || !tile[nextPosition].AsWater()) return 1;

        tile[nextPosition].ActualObject.gameObject.SetActive(isOn);
        return tile[nextPosition].AsWater().ToggleNextWaterBlockActivation(isOn) + 1;
    }
 
    private bool IsTheLeftmostWaterBlock()
    {
        var leftPosition = new Vector2Int(Position.x + (int)MovementDirection.Left, Position.y);
        bool isWaterOnTheLeft = tile.ContainsKey(leftPosition) && tile[leftPosition].AsWater();
        return !isWaterOnTheLeft;
    }

    public override PlaceableWater AsWater()
    {
        return this;
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        LevelEditorManager.OnEditorModeToggled += HandleWaterMerge;
    }

    protected override void OnDisable()
    {
        LevelEditorManager.OnEditorModeToggled -= HandleWaterMerge;
    }
}