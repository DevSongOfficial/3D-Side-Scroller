public sealed class PlaceableSpawnPoint : PlaceableObjectBase
{
    protected override void OnLevelEditorToggled(bool isOn)
    {
        base.OnLevelEditorToggled(isOn);

        ActualObject.gameObject.SetActive(isOn);
    }
}
