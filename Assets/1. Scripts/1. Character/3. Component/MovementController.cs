using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMovementDirection { Left = -1, None = 0, Right = 1 }

// This class manages Velocity and Direction(Rotation).
[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    private Rigidbody rigidBody;

    [SerializeField] private Transform body;

    private readonly int RotationSpeed = 1500;
    
    public static readonly int YAngle_Left = 270;
    public static readonly int YAngle_Right = 90;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
    
    // Direction
    #region Direction
    public event Action<EMovementDirection> OnDirectionChange;
    public EMovementDirection Direction { get; private set; }
    public EMovementDirection FacingDirection 
    {
        get
        {
            float tolerance = 10;

            if (transform.eulerAngles.y <= YAngle_Left + tolerance && transform.eulerAngles.y > YAngle_Left - tolerance)
            {
                return EMovementDirection.Left;
            }
            else if(transform.eulerAngles.y >= YAngle_Right - tolerance && transform.eulerAngles.y < YAngle_Right + tolerance)
            {
                return EMovementDirection.Right;
            }
            return EMovementDirection.None;
        }
    }

    public void ChangeMovementDirection(EMovementDirection newDirection, bool smoothRotation = true)
    {
        if (newDirection == EMovementDirection.None) return;
        if (newDirection == Direction) return;

        Direction = newDirection;

        if (directionChangeCoroutine != null)
        {
            StopCoroutine(directionChangeCoroutine);
            directionChangeCoroutine = null;
        }

        if (smoothRotation)
            directionChangeCoroutine = StartCoroutine(UpdateDirectionRoutine(newDirection));
        else
            SetRotation(Direction.GetYAngle());
        

        OnDirectionChange?.Invoke(newDirection);
    }

    private Coroutine directionChangeCoroutine;
    private IEnumerator UpdateDirectionRoutine(EMovementDirection wishDirection)
    {
        while (wishDirection != EMovementDirection.None)
        {
            if (wishDirection == EMovementDirection.Left)
            {
                if (transform.eulerAngles.y < wishDirection.GetYAngle())
                {
                    Rotate(RotationSpeed * Time.deltaTime);
                }
                else
                {
                    SetRotation(wishDirection.GetYAngle());
                    wishDirection = EMovementDirection.None;
                }
            }
            else if (wishDirection == EMovementDirection.Right)
            {
                if (transform.eulerAngles.y > wishDirection.GetYAngle())
                {
                    Rotate(-RotationSpeed * Time.deltaTime);
                }
                else
                {
                    SetRotation(wishDirection.GetYAngle());
                    wishDirection = EMovementDirection.None;
                }
            }

            yield return new WaitForFixedUpdate();
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

    // Movement; [velocity] basically refers to [velocity of X] in this project.
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
        SetVelocity(GetVelocity() + velocity);
    }

    public void AddForce(Vector3 force)
    {
        rigidBody.AddForce(force);
    }

    public void Jump(float velocityOnJump)
    {
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, velocityOnJump, 0);
    }

    public void StopMovement()
    {
        Direction = EMovementDirection.None;
        SetVelocity(0);

        OnDirectionChange?.Invoke(Direction);
    }

    public void EnableKinematic()
    {
        rigidBody.isKinematic = true;
    }

    public void DisableKinematic() 
    {
        rigidBody.isKinematic = false;
    }

    public void FreezePosition()
    {
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
    }

    public void UnfreezePosition()
    {
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    }

    #endregion

    // Body(Child's Transform)
    #region Child's Transform
    public void SetBodyLocalPosition(Vector3 position)
    {
        body.localPosition = position;
    }

    public void SetBodyLocalEulerAngles(Vector3 rotation)
    {
        body.localEulerAngles = rotation;
    }
    #endregion
}
