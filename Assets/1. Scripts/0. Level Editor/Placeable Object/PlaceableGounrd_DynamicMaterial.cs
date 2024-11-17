using UnityEngine;
using static GameSystem;

public class PlaceableGounrd_DynamicMaterial : PlaceableGround
{
    [SerializeField] protected Material materialOnEditing;
    [SerializeField] protected Material materialOnPlaying;

    private new Renderer renderer;

    protected override void Awake()
    {
        base.Awake();

        renderer = ActualObject.GetComponent<Renderer>();
        LevelEditorManager.OnEditorModeToggled += ShiftMaterial;
    }

    private void ShiftMaterial(bool isOn)
    {
        renderer.material = isOn ? materialOnEditing : materialOnPlaying;
    }

    private void OnDestroy()
    {
        LevelEditorManager.OnEditorModeToggled -= ShiftMaterial;
    }
}