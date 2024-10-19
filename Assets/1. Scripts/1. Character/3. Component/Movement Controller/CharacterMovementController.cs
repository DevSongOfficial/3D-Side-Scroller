using System;
using System.Collections;
using UnityEngine;

// This class manages Characters' Velocity and Direction(Rotation).
[RequireComponent(typeof(CharacterController))]
public sealed class CharacterMovementController : MovementController
{
    public override bool IsGrounded => controller.isGrounded;

    // [CharacterMovementController] uses [CharacterController] for movement.
    private CharacterController controller;

    protected override void Awake()
    {
        base.Awake();

        controller = GetComponent<CharacterController>();
    }

    protected override void Move()
    {
        controller.Move(Velocity * Time.fixedDeltaTime);
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

    // Transform
    #region Child's Transform
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
    #endregion
}