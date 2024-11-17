using System.Collections;
using UnityEngine;
using static AnimationController;

// This class manages Characters' Velocity and Direction(Rotation).
[RequireComponent(typeof(CharacterController))]
public sealed class CharacterMovementController : MovementController
{
    public override bool IsGrounded => controller.isGrounded;
    public bool IsStunned { get; private set; }

    // [CharacterMovementController] uses [CharacterController] for movement.
    private CharacterController controller;

    public bool IsMovingOnZAxis { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();

        controller = GetComponent<CharacterController>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Move()
    {
        controller.Move(Velocity * Time.fixedDeltaTime);

        if (IsMovingOnZAxis) return;
        transform.position = new Vector3(transform.position.x, transform.position.y, CurrentZAxis.GetPositionZ());
    }

    public override void ChangeMovementDirection(MovementDirection newDirection, Space space = Space.World, bool smoothRotation = true)
    {
        if (IsMovingOnZAxis) return;

        base.ChangeMovementDirection(newDirection, space, smoothRotation);
    }

    protected override void OnLand()
    {
        base.OnLand();

        UnstunCharacter();
    }

    public void SetStepOffset(float offset)
    {
        controller.stepOffset = offset;
    }

    public void SetActive(bool isOn)
    {
        enabled = isOn;
        controller.enabled = isOn;
    }

    public void StunCharacter()
    {
        IsStunned = true;
    }

    // Unstun after landing.
    private void UnstunCharacter()
    {
        IsStunned = false;
    }

    // Transform
    public void SetPosition(Vector3 position)
    {
        bool isEnabled = enabled;

        controller.enabled = false;
        transform.position = position;
        controller.enabled = isEnabled;
    }

    public void SetBodyLocalPosition(Vector3 position)
    {
        transform.GetChild(0).localPosition = position;
    }

    public void SetBodyLocalEulerAngles(Vector3 rotation)
    {
        transform.GetChild(0).localEulerAngles = rotation;
    }


    // (Testing)
    // Walking forward and backward.
    public enum ZAxisMovementDirection { Down = -1 /* 180 */, Up = 1 /* 0 */ }
    public ZAxisMovementDirection CurrentZAxis { get; private set; } = ZAxisMovementDirection.Up;

    public void MoveOnZAxis(ZAxisMovementDirection newDirection, Space space = Space.World)
    {
        if (CurrentZAxis == newDirection) return;

        CurrentZAxis = newDirection;

        if (directionChangeCoroutine != null)
        {
            StopCoroutine(directionChangeCoroutine);
            directionChangeCoroutine = null;
        }

        directionChangeCoroutine = StartCoroutine(MoveOnZAxisRoutine(CurrentZAxis, space));
    }

    private Coroutine directionChangeCoroutine;
    private IEnumerator MoveOnZAxisRoutine(ZAxisMovementDirection wishDirection, Space space = Space.World)
    {
        IsMovingOnZAxis = true;

        float tolerance = 20f;

        // Change direction.
        while (true)
        {
            float targetYRotation = (wishDirection == ZAxisMovementDirection.Down) ? 180f : 0f;
            float rotationSpeed = (wishDirection == ZAxisMovementDirection.Down ? 1 : -1) * (int)Direction * RotationSpeed * Time.fixedDeltaTime;

            // Check if rotation is within tolerance.
            if (!Utility.IsWithinTolerance(transform.eulerAngles.y, targetYRotation, tolerance))
            {
                Rotate(rotationSpeed, space);
            }
            else
            {
                SetEulerAngleY(targetYRotation, space);
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        // Walk on Z axis.
        velocity = new Vector3(0, 0, (int)wishDirection * 2);
        Move();

        float timer = 0;
        if (wishDirection == ZAxisMovementDirection.Down)
            while (transform.position.z > CurrentZAxis.GetPositionZ())
                yield return new WaitForFixedUpdate();
        else
            while (transform.position.z < CurrentZAxis.GetPositionZ())
            {
                timer += Time.fixedDeltaTime;
                if (timer > 1.3f) MoveOnZAxis(ZAxisMovementDirection.Down);
                yield return new WaitForFixedUpdate();
            }

        IsMovingOnZAxis = false;
        velocity = Vector3.zero;

        // Change direction back.
        while (true)
        {
            float targetYRotation = Direction.GetYRotationValue();
            float rotationSpeed = RotationSpeed * Time.fixedDeltaTime;
            bool shouldBreak = false;

            // Determine rotation direction and apply rotation.
            if (Direction == MovementDirection.Left)
            {
                if (wishDirection == ZAxisMovementDirection.Down)
                    Rotate(rotationSpeed, space);
                else
                    Rotate(-rotationSpeed, space);

                shouldBreak = Utility.IsWithinTolerance(transform.eulerAngles.y, targetYRotation, tolerance);
            }
            else if (Direction == MovementDirection.Right)
            {
                if (wishDirection == ZAxisMovementDirection.Down)
                    Rotate(-rotationSpeed, space);
                else
                    Rotate(rotationSpeed, space);

                shouldBreak = Utility.IsWithinTolerance(transform.eulerAngles.y, targetYRotation, tolerance);
            }

            // If target rotation is reached, set exact value and exit loop.
            if (shouldBreak)
            {
                SetEulerAngleY(targetYRotation, space);
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        directionChangeCoroutine = null;
    }
}