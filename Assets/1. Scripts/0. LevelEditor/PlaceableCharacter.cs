using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlaceableCharacter : PlaceableObject
{
    private CharacterBase character;
    private Detector detector;
    private Rigidbody rigidBody;

    protected override void Awake()
    {
        base.Awake();

        character = GetComponent<CharacterBase>();
        detector = GetComponent<Detector>();
        rigidBody = GetComponent<Rigidbody>();
    }

    protected override void Start()
    {
        base.Start();

        character.MovementController.ChangeMovementDirection(EMovementDirection.Right);
    }

    protected override void OnLevelEditorTriggered(bool active)
    {
        base.OnLevelEditorTriggered(active);

        character.enabled = !active;
        detector.enabled = !active;
        rigidBody.isKinematic = active;
    }

    public override void InverseRotation()
    {
        if(character.MovementController.Direction == EMovementDirection.Left)
            character.MovementController.ChangeMovementDirection(EMovementDirection.Right);
        else
            character.MovementController.ChangeMovementDirection(EMovementDirection.Left);
    }
}