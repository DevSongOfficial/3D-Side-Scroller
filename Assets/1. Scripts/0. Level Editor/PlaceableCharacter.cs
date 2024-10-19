using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterBase))]
[RequireComponent(typeof(CharacterController))]
public sealed class PlaceableCharacter : PlaceableObjectBase
{
    private CharacterBase character;

    protected override void Awake()
    {
        base.Awake();

        character = GetComponent<CharacterBase>();
    }

    protected override void Start()
    {
        base.Start();

        character.MovementController.ChangeMovementDirection(MovementDirection.Right);
    }

    protected override void OnLevelEditorToggled(bool isOn)
    {
        base.OnLevelEditorToggled(isOn);

        character.enabled = !isOn;
        character.Detector.enabled = !isOn;
        character.MovementController.SetActive(!isOn);
    }

    public override void InverseRotation()
    {
        if(character.MovementController.Direction == MovementDirection.Left)
            character.MovementController.ChangeMovementDirection(MovementDirection.Right);
        else
            character.MovementController.ChangeMovementDirection(MovementDirection.Left);
    }
}