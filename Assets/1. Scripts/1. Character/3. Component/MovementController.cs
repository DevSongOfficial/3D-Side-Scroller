﻿using System;
using System.Collections;
using UnityEngine;

public enum MovementDirection { Left = -1, None = 0, Right = 1 }

// This class manages Velocity and Direction(Rotation).
[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    private CharacterController controller;

    public Vector3 Velocity { get; private set; }

    public bool IsGrounded => controller.isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    #region Direction
    private readonly int RotationSpeed = 1500;
    public static readonly int YAngle_Left = 270;
    public static readonly int YAngle_Right = 90;


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
                    Rotate(RotationSpeed * Time.fixedDeltaTime, space);
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
                    Rotate(-RotationSpeed * Time.fixedDeltaTime, space);
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

    private float wishVelocity;
    public void ChangeDirectionSmooth(EMovementDirection newDirection)
    {
        if (newDirection != Direction)
            wishVelocity = 0;

        ChangeMovementDirection(newDirection, Space.Self);
    }
    #endregion

    #region Movement
    public void SetVelocity(float velocity)
    {
        //rigidBody.velocity = new Vector3((int)FacingDirection * velocity, rigidBody.velocity.y, 0);
        //Velocity = FacingDirection.ConvertToVector3() * velocity * 0.02f;
        //controller.Move(Velocity);
    }

    public void Move(Vector3 velocity)
    {
        controller.Move(velocity);
    }

    public void Jump(float jumpPower)
    {
        if (!controller.isGrounded) return;

        ySpeed = -0.8f;
        ySpeed = jumpPower;
    }

    public Vector3 CalculateVelocity(float speed, float mass)
    {
        var velocity = Vector3.zero;
        velocity += GetHorizontalVelocity(speed, speed);
        velocity += GetVerticalVelocity(mass);

        return velocity;
    }

    public Vector3 CalculateVelocity(float speed, float acceleration, float mass)
    {
        var velocity = Vector3.zero;
        velocity += GetHorizontalVelocity(speed, acceleration);
        velocity += GetVerticalVelocity(mass);

        return velocity;
    }

    private Vector3 GetHorizontalVelocity(float speed, float acceleration)
    {
        if (Math.Abs(wishVelocity) < speed)
        {
            wishVelocity += acceleration;
        }
        else
        {
            wishVelocity = speed;
        }

        return Vector3.right * (int)FacingDirection * wishVelocity;
    }

    private float ySpeed;
    private readonly float MaxYSpeed = -30;
    private Vector3 GetVerticalVelocity(float mass)
    {
        if ((!controller.isGrounded))
        {
            ySpeed -= mass * 0.01f;
        }
        ySpeed = ySpeed < MaxYSpeed ? MaxYSpeed : ySpeed;
        return Vector3.up * ySpeed;
    }
    #endregion









    public void AddForce(Vector3 force)
    {
    }


    public void StopMovement()
    {
        Direction = EMovementDirection.None;
        SetVelocity(0);

        OnDirectionChange?.Invoke(Direction);
    }

    public void EnableKinematic()
    {
        //rigidBody.isKinematic = true;
    }

    public void DisableKinematic() 
    {
        //rigidBody.isKinematic = false;
    }

    // Body(Child's Transform)
    #region Child's Transform
    public void SetBodyLocalPosition(Vector3 position)
    {
        //body.localPosition = position;
    }

    public void SetBodyLocalEulerAngles(Vector3 rotation)
    {
        //body.localEulerAngles = rotation;
    }
    #endregion
}
