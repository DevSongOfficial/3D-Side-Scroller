public sealed class PlaceableSpawnPoint : PlaceableProb
{
    protected override void OnLevelEditorToggled(bool isOn)
    {
        base.OnLevelEditorToggled(isOn);

        ActualObject.gameObject.SetActive(isOn);
    }
}
