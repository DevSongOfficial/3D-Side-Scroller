using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlaceableCharacter : PlaceableObject
{
    private CharacterBase character;
    private Detector detector;

    protected override void Awake()
    {
        base.Awake();

        character = GetComponent<CharacterBase>();
        detector = GetComponent<Detector>();
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
    }

    public override void InverseRotation()
    {
        if(character.MovementController.Direction == EMovementDirection.Left)
            character.MovementController.ChangeMovementDirection(EMovementDirection.Right);
        else
            character.MovementController.ChangeMovementDirection(EMovementDirection.Left);
    }
}