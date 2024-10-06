using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public enum EMovementDirection { Left = -1, None = 0, Right = 1 }

// This class manages Velocity and Direction(Rotation).
[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    private Rigidbody rigidBody;

    public Transform Body => body;
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

    public void ChangeMovementDirection(EMovementDirection newDirection, Space space = Space.World, bool smoothRotation = true)
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
            directionChangeCoroutine = StartCoroutine(UpdateDirectionRoutine(newDirection, space));
        else
            SetEulerAngleY(Direction.GetYAngle(), space);
        

        OnDirectionChange?.Invoke(newDirection);
    }

    private Coroutine directionChangeCoroutine;
    private IEnumerator UpdateDirectionRoutine(EMovementDirection wishDirection, Space space = Space.World)
    {
        while (wishDirection != EMovementDirection.None)
        {
            if (wishDirection == EMovementDirection.Left)
            {
                if (transform.eulerAngles.y < wishDirection.GetYAngle())
                {
                    Rotate(RotationSpeed * Time.deltaTime, space);
                }
                else
                {
                    SetEulerAngleY(wishDirection.GetYAngle(), space);
                    wishDirection = EMovementDirection.None;
                }
            }
            else if (wishDirection == EMovementDirection.Right)
            {
                if (transform.eulerAngles.y > wishDirection.GetYAngle())
                {
                    Rotate(-RotationSpeed * Time.deltaTime, space);
                }
                else
                {
                    SetEulerAngleY(wishDirection.GetYAngle(), space);
                    wishDirection = EMovementDirection.None;
                }
            }

            yield return new WaitForFixedUpdate();
        }

        directionChangeCoroutine = null;
    }

    private void Rotate(float y, Space space = Space.World)
    {
        SetEulerAngleY(transform.eulerAngles.y + y, space);
    }

    public void SetEulerAngleY(float y, Space space = Space.World)
    {
        transform.Rotate(0, y - transform.eulerAngles.y, 0, space);
        return;
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
        rigidBody.velocity = new Vector3((int)FacingDirection * velocity, rigidBody.velocity.y, 0);
    }

    public void SetVelocity(float velocityX, float velocityY)
    {
        rigidBody.velocity = new Vector3((int)FacingDirection * velocityX, velocityY, 0);
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

    public static RigidbodyConstraints FreezeRotation = RigidbodyConstraints.FreezeRotation;
    public static RigidbodyConstraints FreezeRotationYandZ = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    public void FreezePosition(RigidbodyConstraints rotationConstraint)
    {
        rigidBody.constraints = rotationConstraint | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    public void UnfreezePosition(RigidbodyConstraints rotationConstraint)
    {
        rigidBody.constraints = rotationConstraint | RigidbodyConstraints.FreezePositionZ;
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
