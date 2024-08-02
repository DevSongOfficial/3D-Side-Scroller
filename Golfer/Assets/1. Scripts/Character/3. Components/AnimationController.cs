using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.XR.Haptics;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    public string currentState { get; private set; }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public bool ChangeState(string state)
    {
        if (currentState == state) return false;

        currentState = state;
        animator.Play(currentState);

        return true;
    }

    public void Play(float value)
    {
        animator.Play(currentState, 0, value);
    }

    public void SetFloat(string name, float value, float dampTime = 0.1f)
    {
        animator.SetFloat(name, value, dampTime, Time.deltaTime);
    }

    public void SetSpeed(float speed = 1)
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
    }

    public static class Parameter
    {
        public const string MoveSpeed = "MoveSpeed";
    }
}