using System;
using System.Collections;
using UnityEngine;

public enum MovementDirection { Left = -1, None = 0, Right = 1 }
public abstract class MovementController : MonoBehaviour
{
    public Vector3 Velocity => velocity;
    protected Vector3 velocity;

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
    public bool IsChangingDirection => Direction != FacingDirection;

    public void StopAndChangeDirection(MovementDirection newDirection)
    {
        if (newDirection != Direction)
            velocity.x = 0;

        ChangeMovementDirection(newDirection, Space.Self);
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
            SetEulerAngleY(Direction.GetYRotationValue(), space);


        OnDirectionChange?.Invoke(newDirection);
    }

    private Coroutine directionChangeCoroutine;
    private IEnumerator UpdateDirectionRoutine(MovementDirection wishDirection, Space space = Space.World)
    {
        while (wishDirection != MovementDirection.None)
        {
            if (wishDirection == MovementDirection.Left)
            {
                if (transform.eulerAngles.y < wishDirection.GetYRotationValue())
                {
                    Rotate(RotationSpeed * Time.fixedDeltaTime, space);
                }
                else
                {
                    SetEulerAngleY(wishDirection.GetYRotationValue(), space);
                    wishDirection = MovementDirection.None;
                }
            }
            else if (wishDirection == MovementDirection.Right)
            {
                if (transform.eulerAngles.y > wishDirection.GetYRotationValue())
                {
                    Rotate(-RotationSpeed * Time.fixedDeltaTime, space);
                }
                else
                {
                    SetEulerAngleY(wishDirection.GetYRotationValue(), space);
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
    #endregion

    // MOVEMENT SECTION
    #region Velocity
    public void SetVelocity(Vector3 velocity)
    {
        SetVelocity(velocity.x, velocity.y);
    }
    public void SetVelocity(float velocityX, float velocityY)
    {
        velocity.x = velocityX;
        velocity.y = velocityY;
    }

    private bool canMoveHorizontally = true;
    public void ToggleHorizontalMovement(bool isOn, float wishVelocity = 0)
    {
        canMoveHorizontally = isOn;
        velocity.x = wishVelocity;
    }

    // Horizontal move without acceleration.
    public Vector3 ApplyHorizontalVelocity(float speed)
    {
        if (!canMoveHorizontally) return Vector3.zero;

        velocity.x = speed * (int)FacingDirection;

        return Velocity;
    }

    // Horizontal move w/ acceleration.
    public Vector3 ApplyHorizontalVelocity(float speed, float acceleration)
    {
        if (!canMoveHorizontally) return Vector3.zero;

        velocity.x = Math.Abs(velocity.x) < speed ? velocity.x + acceleration * (int)FacingDirection : speed * (int)FacingDirection;

        return Velocity;
    }

    private readonly float MinYSpeed = -30;
    private readonly float YSpeedMultiplier = 0.1f;
    public Vector3 ApplyVerticalVelocity(float mass)
    {
        if ((!IsGrounded))
        {
            velocity.y -= mass * YSpeedMultiplier;
        }

        velocity.y = velocity.y < MinYSpeed ? MinYSpeed : velocity.y;

        return Velocity;
    }
    #endregion

    // ROTATION SECTION
    public virtual Quaternion AlignToGround()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.2f, Layer.Ground.GetMask()))
            return Quaternion.identity;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

        // Clamp x rotation by converting Quaternion to Euler and then convert it back into Quaternion.
        Vector3 targetEulerAngles = targetRotation.eulerAngles;
        if (targetEulerAngles.x > 180) targetEulerAngles.x -= 360;
        targetEulerAngles.x = Mathf.Clamp(targetEulerAngles.x, -40, 40);
        targetRotation = Quaternion.Euler(targetEulerAngles);

        return transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }
}