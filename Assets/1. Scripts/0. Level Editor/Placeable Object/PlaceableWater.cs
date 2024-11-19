using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameSystem;

public sealed class PlaceableWater : PlaceableGround
{
    protected override void Start()
    {
        base.Start();

        // This is going to be overlapped with other blocks so should be a bit smaller.
        ActualObject.localScale = new Vector3(0.99f, 1, 0.99f);
    }

    protected override void LateUpdate()
    {
        if (isEditorMode)
        {
            ActualObject.position = transform.position;
            ActualObject.rotation = transform.rotation;
        }
    }

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

    private void SplitAndShrinkWaterBlocksInARow()
    {
        if (!IsTheLeftmostWaterBlock()) return;
        ToggleNextWaterBlockActivation(isOn: true);
        Shrink();
    }

    private void Strech(int size)
    {
        float x = Position.x + (size - 1) * 0.5f;

        ActualObject.transform.position = new Vector3(x, Position.y);
        ActualObject.transform.localScale = new Vector3(3.99f, 1, size);
    }

    private void Shrink()
    {
        ActualObject.transform.position = new Vector3(Position.x, Position.y);
        ActualObject.transform.localScale = new Vector3(3.99f, 1, 0.99f);
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
        base.OnDisable();
        LevelEditorManager.OnEditorModeToggled -= HandleWaterMerge;
    }
}