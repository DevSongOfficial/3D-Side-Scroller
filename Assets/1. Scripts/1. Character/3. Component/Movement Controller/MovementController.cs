using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public enum MovementDirection { Left = -1, None = 0, Right = 1 }
public abstract class MovementController : MonoBehaviour
{
    public Vector3 Velocity => velocity;
    protected Vector3 velocity;
    
    // todo: refactor these temporary codes. 
    protected float VelocityMultiplier { get; private set; } = 1;
    public void SetVelocityMultiplier(float multiplier)
    {
        if(velocityChangeCoroutine != null) StopCoroutine(velocityChangeCoroutine);
        velocityChangeCoroutine = StartCoroutine(SmoothChangeToTargetMultiplier(multiplier));
    }

    private Coroutine velocityChangeCoroutine;
    private IEnumerator SmoothChangeToTargetMultiplier(float targetMultiplier)
    {
        float changeRate = Time.fixedDeltaTime;

        while (!Mathf.Approximately(VelocityMultiplier, targetMultiplier))
        {
            VelocityMultiplier = Mathf.MoveTowards(VelocityMultiplier, targetMultiplier, changeRate);
            yield return new WaitForFixedUpdate();
        }

        VelocityMultiplier = targetMultiplier;
        velocityChangeCoroutine = null;
    }

    public abstract bool IsGrounded { get; }

    protected virtual void Awake() { }

    protected virtual void FixedUpdate()
    {
        Move();
        CheckLanding();
    }

    protected abstract void Move();

    // DIRECTION SECTION
    #region Direction
    protected readonly int RotationSpeed = 1500;
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

    public virtual void ChangeMovementDirection(MovementDirection newDirection, Space space = Space.World, bool smoothRotation = true)
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
            float targetYRotation = wishDirection.GetYRotationValue();
            float rotationSpeed = (wishDirection == MovementDirection.Left ? 1 : -1) * RotationSpeed * Time.fixedDeltaTime;

            // Check if rotation's completed.
            if (!IsRotationComplete(transform.eulerAngles.y, targetYRotation, wishDirection))
                Rotate(rotationSpeed, space);
            else
            {
                SetEulerAngleY(targetYRotation, space);
                wishDirection = MovementDirection.None;
            }

            yield return new WaitForFixedUpdate();
        }

        directionChangeCoroutine = null;
    }

    private bool IsRotationComplete(float currentY, float targetY, MovementDirection direction)
    {
        return direction == MovementDirection.Left
            ? currentY >= targetY
            : currentY <= targetY;
    }

    protected void Rotate(float y, Space space = Space.World)
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
        return target.transform.position.x > transform.position.x 
            ? MovementDirection.Right 
            : MovementDirection.Left;
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

        velocity.x = Math.Abs(velocity.x) < speed 
            ? velocity.x + acceleration * (int)FacingDirection 
            : speed * (int)FacingDirection;

        return Velocity;
    }

    protected readonly float MinYSpeed = -20;
    protected readonly float DefaultYSpeed = -1f;
    protected readonly float YSpeedMultiplier = 0.1f;
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
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.2f, Layer.Ground.GetMask()))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

            // Convert Quaternion to Euler for clamiping X rotation and then convert it back into Quaternion.
            Vector3 targetEulerAngles = targetRotation.eulerAngles;
            if (targetEulerAngles.x > 180) targetEulerAngles.x -= 360;
            if (targetEulerAngles.x > 40 || targetEulerAngles.x < -40) targetEulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            targetRotation = Quaternion.Euler(targetEulerAngles);

            return transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        else // if there's no ground, lerp eulerAnlge X into 0;
        {
            var targetRotation = Quaternion.Euler(Vector3.up * Direction.GetYRotationValue());
            return transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    // LANDING SECTION
    public event Action OnLanded;
    private bool wasGrounded; // True when controller was grounded a frame ago.
    private void CheckLanding()
    {
        if(!wasGrounded && IsGrounded)
        {
            OnLand();
        }

        wasGrounded = IsGrounded;
    }

    protected virtual void OnLand()
    {
        OnLanded?.Invoke();

        velocity.y = DefaultYSpeed;
    }

    // EXTRA
    public void WalkForward()
    {
        
    }
}