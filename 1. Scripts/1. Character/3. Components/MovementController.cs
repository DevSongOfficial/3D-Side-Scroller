using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Monobehaviour 상속 받아 컴포넌트로 넣는 것이 더 나은가?

public enum EMovementDirection { Left = -1, None = 0, Right = 1 }

// This class manages Velocity and Direction(Rotation).
[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    private static readonly int rotationSpeed = 1500;
    private Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Direction
    #region Direction
    public event Action<EMovementDirection> OnDirectionChange;
    public EMovementDirection Direction { get; private set; }
    public void ChangeMovementDirection(EMovementDirection newDirection)
    {
        if (newDirection == EMovementDirection.None) return;
        if (newDirection == Direction) return;

        Direction = newDirection;

        if (directionChangeCoroutine != null) return;
        directionChangeCoroutine = StartCoroutine(UpdateDirectionRoutine(newDirection));
        OnDirectionChange?.Invoke(newDirection);
    }

    private Coroutine directionChangeCoroutine;
    private IEnumerator UpdateDirectionRoutine(EMovementDirection wishDirection)
    {
        int wishYAngle = wishDirection == EMovementDirection.Left ? 270 : 90;

        while (wishDirection != EMovementDirection.None)
        {
            if (wishDirection == EMovementDirection.Left)
            {
                if (transform.eulerAngles.y < wishYAngle)
                {
                    Rotate(rotationSpeed * Time.deltaTime);
                }
                else
                {
                    SetRotation(wishYAngle);
                    wishDirection = EMovementDirection.None;
                }
            }
            else if (wishDirection == EMovementDirection.Right)
            {
                if (transform.eulerAngles.y > wishYAngle)
                {
                    Rotate(-rotationSpeed * Time.deltaTime);
                }
                else
                {
                    SetRotation(wishYAngle);
                    wishDirection = EMovementDirection.None;
                }
            }

            yield return null;
        }

        directionChangeCoroutine = null;
    }

    private void Rotate(float y)
    {
        SetRotation(transform.eulerAngles.y + y);
    }

    public void SetRotation(float y)
    {
        transform.eulerAngles = new Vector3(transform.rotation.x, y, transform.rotation.z);
    }

    public EMovementDirection GetDirectionFrom<T>(T target) where T : CharacterBase
    {
        return target.transform.position.x > transform.position.x ? EMovementDirection.Right : EMovementDirection.Left;
    }
    #endregion

    // Movement
    // [velocity] basically refers to [velocity of X] in this project.
    #region Physical Movement
    public float GetVelocity()
    {
        return (int)Direction * rigidBody.velocity.x;
    }

    public void SetVelocity(float velocity)
    {
        rigidBody.velocity = new Vector3((int)Direction * velocity, rigidBody.velocity.y, 0);
    }

    public void AddVelocity(float velocity)
    {
        Debug.Log(GetVelocity() + velocity);
        SetVelocity(GetVelocity() + velocity);
    }

    public void Jump(float velocityOnJump)
    {
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, velocityOnJump, 0);
    }
    #endregion
}