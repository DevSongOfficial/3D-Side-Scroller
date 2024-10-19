using System;
using System.Collections;
using UnityEngine;

public enum MovementDirection { Left = -1, None = 0, Right = 1 }
public abstract class MovementController : MonoBehaviour
{
    public Vector3 Velocity => velocity;
    protected Vector3 velocity;
    private float wishVelocity; // Velocity for calculating accleration.

    public abstract bool IsGrounded { get; }

    protected virtual void Awake() { }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected abstract void Move();

    // DIRECTION SECTION
    #region Direction
    private readonly int RotationSpeed = 1500;
    public static readonly int YAngle_Left = 270;
    public static readonly int YAngle_Right = 90;

    public event Action<MovementDirection> OnDirectionChange;
    public MovementDirection Direction { get; private set; }
    public MovementDirection FacingDirection
    {
        get
        {
            float tolerance = 10;

            if (transform.eulerAngles.y <= YAngle_Left + tolerance && transform.eulerAngles.y > YAngle_Left - tolerance)
            {
                return MovementDirection.Left;
            }
            else if (transform.eulerAngles.y >= YAngle_Right - tolerance && transform.eulerAngles.y < YAngle_Right + tolerance)
            {
                return MovementDirection.Right;
            }
            return MovementDirection.None;
        }
    }

    public void ChangeMovementDirection(MovementDirection newDirection, Space space = Space.World, bool smoothRotation = true)
    {
        if (newDirection == MovementDirection.None) return;
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
    private IEnumerator UpdateDirectionRoutine(MovementDirection wishDirection, Space space = Space.World)
    {
        while (wishDirection != MovementDirection.None)
        {
            if (wishDirection == MovementDirection.Left)
            {
                if (transform.eulerAngles.y < wishDirection.GetYAngle())
                {
                    Rotate(RotationSpeed * Time.fixedDeltaTime, space);
                }
                else
                {
                    SetEulerAngleY(wishDirection.GetYAngle(), space);
                    wishDirection = MovementDirection.None;
                }
            }
            else if (wishDirection == MovementDirection.Right)
            {
                if (transform.eulerAngles.y > wishDirection.GetYAngle())
                {
                    Rotate(-RotationSpeed * Time.fixedDeltaTime, space);
                }
                else
                {
                    SetEulerAngleY(wishDirection.GetYAngle(), space);
                    wishDirection = MovementDirection.None;
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

    public MovementDirection GetDirectionFrom<T>(T target) where T : CharacterBase
    {
        return target.transform.position.x > transform.position.x ? MovementDirection.Right : MovementDirection.Left;
    }

    public void ChangeDirectionSmooth(MovementDirection newDirection)
    {
        if (newDirection != Direction)
            wishVelocity = 0;

        ChangeMovementDirection(newDirection, Space.Self);
    }
    #endregion

    // MOVEMENT SECTION
    #region Velocity
    public void Jump(float jumpVelocity)
    {
        ySpeed = -0.8f;
        ySpeed = jumpVelocity;
    }

    private bool canMoveHorizontally = true;
    public void ToggleHorizontalMovement(bool isOn, float wishVelocity = 0)
    {
        canMoveHorizontally = isOn;
        this.wishVelocity = wishVelocity;
        velocity.x = wishVelocity;
    }

    // Horizontal move without acceleration.
    public Vector3 ApplyHorizontalVelocity(float speed)
    {
        if (!canMoveHorizontally) return Vector3.zero;

        wishVelocity = speed;
        velocity.x = (int)FacingDirection * wishVelocity;
        return Velocity;
    }

    // Horizontal move w/ acceleration.
    public Vector3 ApplyHorizontalVelocity(float speed, float acceleration)
    {
        if (!canMoveHorizontally) return Vector3.zero;

        wishVelocity = Math.Abs(wishVelocity) < speed ? wishVelocity + acceleration : speed;
        velocity.x = (int)FacingDirection * wishVelocity;
        return Velocity;
    }

    private float ySpeed;
    private readonly float MinYSpeed = -30;
    private readonly float YSpeedMultiplier = 0.1f;
    public Vector3 ApplyVerticalVelocity(float mass)
    {
        if ((!IsGrounded))
        {
            ySpeed -= mass * YSpeedMultiplier;
        }

        ySpeed = ySpeed < MinYSpeed ? MinYSpeed : ySpeed;
        velocity.y = ySpeed;

        return Velocity;
    }
    #endregion


}