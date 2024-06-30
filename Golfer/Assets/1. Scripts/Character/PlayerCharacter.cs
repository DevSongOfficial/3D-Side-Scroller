using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public sealed class PlayerCharacter : CharacterBase
{
    // Movement & Rotation
    protected override EMovementDirection MovementDirection { get { return (EMovementDirection)movementValue; } }
    private EMovementDirection targetDirection;
    [HideInInspector] public int movementValue;
    private bool canRotate;
    public float YAngle { get; private set; }

    // Golf Swing
    private const int maxFrame = 119;
    [Range(0, maxFrame)]
    public float frame;
    Vector2 mousePosition;
    private bool canSwing;
    private float swingPower;

    // Character Condition
    public static readonly CharacterCondition condition_Swing = new CharacterCondition() { canMove = false };
    public static readonly CharacterCondition condition_Move = new CharacterCondition() { canMove = true };

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        ChangeConditionState(condition_Move);
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
        HandleGolfSwing();
        HandleAnimation(CurrentAnimationState);
    }

    #region Movement
    // Automatically called by Player Input Action
    private void OnMove(InputValue value)
    {
        if (CurrentCondition == condition_Swing) return;

        ChangeAnimationState(CharacterAnimation.State.BT_Move);
        ChangeConditionState(condition_Move);

        movementValue = (int)value.Get<float>();
        Xspeed = 0;

        if (movementValue == 0) return;

        targetDirection = MovementDirection;
        canRotate = true;
    }

    private void HandleMovement()
    {
        if (CurrentCondition == condition_Swing) return;

        if (targetDirection != EMovementDirection.None)
        {
            Xspeed = (int)MovementDirection * 100 * Time.deltaTime;
            rigidBody.velocity = new Vector3(Xspeed, 0, 0);
            return;
        }

        int targetSpeed = (int)MovementDirection * characterInfo.MovementSpeed;

        if (Math.Abs(Xspeed) < Math.Abs(targetSpeed))
        {
            Xspeed += (int)MovementDirection * 10 * Time.deltaTime;
        }
        else
        {
            Xspeed = targetSpeed;
        }


        rigidBody.velocity = new Vector3(Xspeed, 0, 0);
    }

    private void HandleRotation()
    {
        if (!canRotate) return;

        int targetYAngle = targetDirection == EMovementDirection.Left ? 270 : 90;

        if (targetYAngle == 270)
        {
            if (transform.eulerAngles.y < targetYAngle)
            {
                YAngle += characterInfo.RotationSpeed * Time.deltaTime;
            }
            else
            {
                YAngle = targetYAngle;
                targetDirection = EMovementDirection.None;
                canRotate = false;
            }
        }
        else if (targetYAngle == 90)
        {
            if (transform.eulerAngles.y > targetYAngle)
            {
                YAngle -= characterInfo.RotationSpeed * Time.deltaTime;
            }
            else
            {
                YAngle = targetYAngle;
                targetDirection = EMovementDirection.None;
                canRotate = false;
            }
        }

        transform.eulerAngles = new Vector3(transform.rotation.x, YAngle, transform.rotation.z);
    }
    #endregion

    private void ChargeGolfSwing(Vector2 newMousePosition)
    {
        if (swingPower > 0) return;

        if (newMousePosition.x < mousePosition.x)
        {
            if (frame < 34) frame += 120 * Time.deltaTime;
            if (frame > 34) frame = 34;
        }

        mousePosition = newMousePosition;
    }

    private void HandleGolfSwing()
    {
        if (!canSwing) return;

        if (frame < 110)
            frame += swingPower * Time.deltaTime;
        else
        {
            canSwing = false;
            swingPower = 0;
            frame = 0;

            Cursor.lockState = CursorLockMode.None;
            ChangeAnimationState(CharacterAnimation.State.BT_Move);
            ChangeConditionState(condition_Move);
        }
    }

    private float SetSwingPower(float frame)
    {
        swingPower = frame * 7;
        if (swingPower > 200) swingPower = 200;
        if (swingPower < 200) swingPower *= 0.75f;
        if (swingPower < 50) swingPower = 50;
        return swingPower;
    }

    private void OnClick()
    {
        if(CurrentCondition == condition_Move)
        {
            transform.eulerAngles = new Vector3(transform.rotation.x, 90, transform.rotation.z);
            ChangeAnimationState(CharacterAnimation.State.Swing);
            ChangeConditionState(condition_Swing);
        }
        else if(CurrentCondition == condition_Swing)
        {
            if(frame <= 34 && frame > 0)
            {
                Cursor.lockState = CursorLockMode.Locked;
                canSwing = true;
                swingPower = SetSwingPower(frame);
            }
        }

    }

    private void OnDrag(InputValue value)
    {
        if (CurrentCondition != condition_Swing) return;

        var newMousePosition = value.Get<Vector2>();
        ChargeGolfSwing(newMousePosition);
    }

    protected override void ChangeConditionState(CharacterCondition condition)
    {
        base.ChangeConditionState(condition);
    }

    private void HandleAnimation(CharacterAnimation.State state)
    {
        animator.speed = 1;

        switch (state)
        {
            case CharacterAnimation.State.BT_Move:
                HandleMoveAnimation(MathF.Abs(Xspeed));
                break;
            case CharacterAnimation.State.Swing:
                HandleGolfAnimation(frame);
                break;
        }
    }

    private void HandleMoveAnimation(float moveSpeed)
    {
        var param_MoveSpeed = CharacterAnimation.Parameter.MoveSpeed.ToString();
        animator.SetFloat(param_MoveSpeed, moveSpeed, 0.1f, Time.deltaTime);
    }

    private void HandleGolfAnimation(float frame)
    {
        animator.speed = 0;
        animator.Play(CharacterAnimation.State.Swing.ToString(), 0, frame / maxFrame);
    }
}