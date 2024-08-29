using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool CompareState(this StateBase state, StateBase another)
    {
        return ReferenceEquals(state, another);
    }

    public static Vector3 ConvertToVector3(this EMovementDirection direction)
    {
        switch (direction)
        {
            case EMovementDirection.Right:
                return Vector3.right;
            case EMovementDirection.Left:
                return Vector3.left;
            default:
                return Vector3.zero;
        }
    }

    public static int GetYAngle(this EMovementDirection direction)
    {
        switch (direction)
        {
            case EMovementDirection.Left:
                return MovementController.YAngle_Left;
            case EMovementDirection.Right:
                return MovementController.YAngle_Right;
            default:
                return 0;
        }
    }

    public static Vector3 ChangeVectorXWithDirection(this Vector3 vector, EMovementDirection direction)
    {
        return new Vector3(vector.x * (int)direction, vector.y, vector.z);
    }

    public static int GetMask(this Layer layer)
    {
        return 1 << (int)layer;
    }

    public static void SetLayer(this GameObject go, Layer layer)
    {
        go.layer = (int)layer;
    }

    public static bool CompareLayer(this int layer, Layer layerToCompare)
    {
        return layer == (int)layerToCompare;
    }

    public static bool CompareLayer(this Collider collider, Layer layerToCompare)
    {
        return collider.gameObject.layer == (int)layerToCompare;
    }

    public static void SetTag(this GameObject go, Tag tag)
    {
        go.tag = tag.ToString();
    }

    public static bool CompareTag(this Collider collider, Tag tag)
    {
        return collider.gameObject.CompareTag(tag.ToString());
    }
}   
