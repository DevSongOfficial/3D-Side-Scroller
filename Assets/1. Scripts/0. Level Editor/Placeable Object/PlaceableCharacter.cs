using static GameSystem;

public sealed class PlaceableCharacter : PlaceableObjectBase
{
    private CharacterBase childCharacter;

    protected override void Awake()
    {
        base.Awake();

        childCharacter = ActualObject.GetComponent<CharacterBase>();
    }

    protected override void Start()
    {
        base.Start();

        childCharacter.MovementController.ChangeMovementDirection(MovementDirection.Right);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        childCharacter.OnDestroy += RemovePlaceableCharacter;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        childCharacter.OnDestroy -= RemovePlaceableCharacter;
    }

    protected override void LateUpdate()
    {
        if (!isEditorMode) return;
        childCharacter.MovementController.SetPosition(transform.position);
    }

    protected override void OnLevelEditorToggled(bool isOn)
    {
        base.OnLevelEditorToggled(isOn);
        
        childCharacter.enabled = !isOn;
        childCharacter.Detector.enabled = !isOn;
        childCharacter.MovementController.SetActive(!isOn);
    }

    private void RemovePlaceableCharacter()
    {
        LevelEditorManager.RemovePlaceableObject(this);
    }

    public override void InverseRotation()
    {
        if(childCharacter.MovementController.Direction == MovementDirection.Left)
            childCharacter.MovementController.ChangeMovementDirection(MovementDirection.Right);
        else
            childCharacter.MovementController.ChangeMovementDirection(MovementDirection.Left);
    }
}