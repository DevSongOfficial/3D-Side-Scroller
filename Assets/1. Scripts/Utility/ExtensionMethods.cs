using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool CompareState(this StateBase state, StateBase another)
    {
        return ReferenceEquals(state, another);
    }

    public static Vector3 ConvertToVector3(this MovementDirection direction)
    {
        switch (direction)
        {
            case MovementDirection.Right:
                return Vector3.right;
            case MovementDirection.Left:
                return Vector3.left;
            default:
                return Vector3.zero;
        }
    }

    public static MovementDirection GetFlippedDirection(this MovementDirection direction)
    {
        switch (direction)
        {
            case MovementDirection.Right:
                return MovementDirection.Left;
            case MovementDirection.Left:
                return MovementDirection.Right;
            default:
                return MovementDirection.None;
        }
    }

    public static int GetYRotationValue(this MovementDirection direction)
    {
        switch (direction)
        {
            case MovementDirection.Left:
                return CharacterMovementController.YAngle_Left;
            case MovementDirection.Right:
                return CharacterMovementController.YAngle_Right;
            default:
                return 0;
        }
    }

    public static Vector3 ChangeVectorXWithDirection(this Vector3 vector, MovementDirection direction)
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
