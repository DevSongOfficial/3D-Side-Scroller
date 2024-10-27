using Unity.Collections.LowLevel.Unsafe;

public sealed class PlaceableCharacter : PlaceableObjectBase
{
    private CharacterBase childCharacter;

    protected override void Awake()
    {
        base.Awake();

        childCharacter = child.GetComponent<CharacterBase>();
    }

    protected override void Start()
    {
        base.Start();

        childCharacter.MovementController.ChangeMovementDirection(MovementDirection.Right);
    }

    protected override void LateUpdate()
    {
        if (!isOn) return;
        childCharacter.MovementController.SetPosition(transform.position);
    }

    protected override void OnLevelEditorToggled(bool isOn)
    {
        base.OnLevelEditorToggled(isOn);
        
        childCharacter.enabled = !isOn;
        childCharacter.Detector.enabled = !isOn;
        childCharacter.MovementController.SetActive(!isOn);
    }

    public override void InverseRotation()
    {
        if(childCharacter.MovementController.Direction == MovementDirection.Left)
            childCharacter.MovementController.ChangeMovementDirection(MovementDirection.Right);
        else
            childCharacter.MovementController.ChangeMovementDirection(MovementDirection.Left);
    }
}