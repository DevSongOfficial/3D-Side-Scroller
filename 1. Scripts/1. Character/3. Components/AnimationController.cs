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

    public bool ChangeState(string state, float transitionDuration = 0.1f)
    {
        if (CurrentState == state)
        {
            return false;
        }

        CurrentState = state;
        animator.CrossFade(CurrentState, transitionDuration);

        return true;
    }

    public bool ChangeState<T>(T newState, float transitionDuration = 0.1f) where T : System.Enum
    {
        return ChangeState(newState.ToString(), transitionDuration);
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

    public int GetCurrentFrame(int maxFrame)
    {
        try
        {
            AnimatorClipInfo[] animationClip = animator.GetCurrentAnimatorClipInfo(0);
            int currentFrame = (int)(animator.GetCurrentAnimatorStateInfo(0).normalizedTime * animationClip [0].clip.length * animationClip[0].clip.frameRate);
            currentFrame %= maxFrame;
        
            return currentFrame;
        }
        catch
        {
            return 0;
        }
    }

    // Must be the same as the animation nodes in the controllers.
    // I used enum for creating dropdown menu in scriptable objects.
    public static class Player
    {
        public enum Movement
        {
            BT_1, // Movement behaviour tree depending on [MoveSpeed]
            Jump,
        }

        public enum Attack
        {
            Downward_1,
            Swing,
        }
    }

    public static class Zombie
    {
        public enum Movement
        {
            Idle,
            ZombieMove1, // Walk
            ZombieMove2, // Walk, twice as fast
            ZombieMove3, // Run
        }

        public enum Attack
        {
            ZombieThreaten,
            ZombieAttack1,
            ZombieAttack2,
        }
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