using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.XR.Haptics;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    public string CurrentState { get; private set; }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public bool ChangeState(string state)
    {
        if (CurrentState == state) return false;

        CurrentState = state;
        animator.Play(CurrentState);

        return true;
    }

    public void Play(float value)
    {
        animator.Play(CurrentState, 0, value);
    }

    public void SetFloat(string name, float value, float dampTime = 0.1f)
    {
        animator.SetFloat(name, value, dampTime, Time.deltaTime);
    }

    public void SetSpeed(float speed = Speed.Normal)
    {
        animator.speed = speed;
    }

    // Must be the same as the animation nodes in the controllers.
    public static class State
    {
        public const string None = "None";
        
        // For Player
        public const string PlayerMove1 = "PlayerMove1";
        public const string Jump = "Jump";
        public const string Swing = "Swing";

        // For Zombies
        public const string ZombieMove1 = "ZombieMove1";
        public const string ZombieMove2 = "ZombieMove2"; // Same as [ZombieMove1] but twice as fast
    }

    public static class Parameter
    {
        public const string MoveSpeed = "MoveSpeed";
    }

    public static class Speed
    {
        public const int Pause = 0;
        public const int Normal = 1;
    }
}