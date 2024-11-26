using System.Collections.Generic;
using UnityEngine;
using static GameSystem;

public class PlaceableGround : PlaceableProb
{
    protected static Dictionary<Vector2Int, PlaceableGround> Tile = new Dictionary<Vector2Int, PlaceableGround>();

    // Size the ground takes up in Level Editor Coorninates.
    [SerializeField] protected Vector2Int size = Vector2Int.one;

    private Vector2Int startingPoint;
    private Vector2Int endPoint;

    public Vector2Int Position => new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

    private void OnDestroy()
    {
        RemoveFromTile();
    }

    public void AddToTile()
    {
        int startingPoint = Position.x - ((size.x - 1) / 2);
        int endPoint = startingPoint + size.x - 1;

        for (int x = startingPoint; x <= endPoint; x++)
        {
            var area = new Vector2Int(x, Position.y);
            Tile.Add(area, this);
        }

        this.startingPoint = new Vector2Int(startingPoint, Position.y);
        this.endPoint = new Vector2Int(endPoint, Position.y);
    }

    public void RemoveFromTile()
    {
        int y = startingPoint.y;

        for(int x = startingPoint.x; x <= endPoint.x; x++)
        {
            Tile.Remove(new Vector2Int(x, y));
        }
    }

    public override bool NotOverlapped => base.NotOverlapped && IsTileAreaEmpty();

    protected bool IsTileAreaEmpty()
    {
        int startingPoint = Position.x - ((size.x - 1) / 2);
        int endPoint = startingPoint + size.x - 1;

        for (int x = startingPoint; x <= endPoint; x++)
        {
            var area = new Vector2Int(x, Position.y);
            if (Tile.ContainsKey(area)) return false;
        }

        return true;
    }

    protected override void Start()
    {
        base.Start();

        actualObject_rigidBody.isKinematic = true;
        actualObject_IsKinematic = true;

        ActualObject.gameObject.SetLayer(Layer.Ground);
        try { ActualObject.GetChild(0).gameObject.SetLayer(Layer.Ground); }
        catch { }
    }

    protected override void OnLevelEditorToggled(bool isOn)
    {
        isEditorMode = isOn;
    }

    protected override void OnTriggerEnter(Collider other) { }

    protected override void OnTriggerExit(Collider other) { }

    public override PlaceableGround AsGround()
    {
        return this;
    }

    public virtual PlaceableWater AsWater()
    {
        return null;
    }
}