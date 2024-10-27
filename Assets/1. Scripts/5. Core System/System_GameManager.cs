using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameSystem;

public enum Layer
{
    Default = 0,
    Character = 8,
    Ground = 9,
    Wall = 10,
    Placeable = 20,
    Damageable = 24,
    Interactable = 25,
    RaycastBlockerForLevelEditor = 31,
}

public enum Tag
{
    Untagged, Player, Enemy, Prob
}

public class System_GameManager : MonoBehaviour
{
    // Main Camera
    [SerializeField] private CinemachineBrain cinemachineBrain;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        LevelEditorManager.OnEditorModeToggled += (bool enable) => ToggleInput(!enable);
    }

    // Cinemachine Camera
    public void SetCameraUpdateMethod(CinemachineBrain.UpdateMethod method)
    {
        cinemachineBrain.m_UpdateMethod = method;
    }

    // Input System
    private PlayerInput input;
    public void ToggleInput(bool enable) => input.enabled = enable;

    public event Action<MovementDirection> Input_OnChangeDirection;
    public event Action Input_OnJump;
    public event Action<bool> Input_OnClick;
    public event Action<Vector2> Input_OnDrag;
    public event Action Input_OnInteract;
    public event Action Input_OnSwitchClub;
    public event Action Input_OnTogglePickup;

    // Automatically called by Player Input Action
    private void OnMove(InputValue value) // [A/D], [LeftArrow/RightArrow] key down & up
    {
        var valueConverted = (int)value.Get<float>();
        var directionToMove = (MovementDirection)valueConverted;
        Input_OnChangeDirection.Invoke(directionToMove);
    }

    private void OnJump() // [Space Bar] pressed
    {
        Input_OnJump.Invoke();
    }

    private void OnClick(InputValue value) // [Mouse 0]
    {
        var mouseDown = (int)value.Get<float>() == 1;
        Input_OnClick.Invoke(mouseDown);
    }

    private void OnDrag(InputValue value) // [Cursor position] changed
    {
        var mousePosition = value.Get<Vector2>();
        Input_OnDrag.Invoke(mousePosition);
    }

    private void OnInteract() // [E] pressed
    {
        Input_OnInteract.Invoke();
    }

    private void OnSwitchClub() // [Q] pressed
    {
        Input_OnSwitchClub.Invoke();
    }

    private void OnTogglePickup() // [R] pressed
    {
        Input_OnTogglePickup.Invoke();
    }
}
