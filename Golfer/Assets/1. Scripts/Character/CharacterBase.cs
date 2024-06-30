using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    [Tooltip("Character Information")]
    public CharacterInfo characterInfo;

    // Components for every character
    protected Rigidbody rigidBody;
    protected Animator animator;

    // Character Movement & Direction
    public enum EMovementDirection { Left = -1, None = 0, Right = 1 }
    protected virtual EMovementDirection MovementDirection { get; set; }
    public float Xspeed { get; protected set; }

    // Character Animation
    protected CharacterAnimation.State CurrentAnimationState { get; private set; }
    protected CharacterCondition CurrentCondition { get; private set; }

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    public virtual void TakeDamage(DamageEvent damageEvent)
    {
        characterInfo.TakeDamage(damageEvent.damage);
    }

    protected virtual bool ChangeAnimationState(CharacterAnimation.State state)
    {
        if (CurrentAnimationState == state) return false;

        CurrentAnimationState = state;
        animator.Play(CurrentAnimationState.ToString());

        return true;
    }

    protected virtual void ChangeConditionState(CharacterCondition condition)
    {
        CurrentCondition = condition;
    }

    public static class CharacterAnimation
    {
        public enum State { BT_Move, Jump, Swing }

        public enum Parameter { MoveSpeed }
    }

    // 클래스로는 State 체크 + 내부 프로퍼티들로는 Condition 체크
    public class CharacterCondition
    {
        public bool canMove;
    }
}
