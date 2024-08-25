using System.Collections;
using UnityEngine;

public sealed class PlayerAttackState : PlayerStateBase
{
    public PlayerAttackState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }

    // Temporary attack info.
    // todo: fix this according to OOP.
    private Vector3 attackPosition => player.transform.position + new Vector3(1, 1.35f, 0);
    private const float attackRadius = 1.5f;

    public bool IsAttacking =>  attackCoroutine != null;

    // Delay before attacking
    private const float attackDelay = 0.15f;
    private float attackDelayLeft;

    public override void EnterState()
    {
        base.EnterState();

        player.MovementController.StopMovement();

        sharedData.Input_MouseUp += TryAttack;

        attackDelayLeft = attackDelay;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        HandleSwingPreparation();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();

        sharedData.Input_MouseUp -= TryAttack;
        
        if (IsAttacking)
        {
            player.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    private void HandleSwingPreparation()
    {
        if (IsAttacking) return;

        attackDelayLeft -= Time.deltaTime;
        
        if(attackDelayLeft <= 0)
        {
            attackDelayLeft = 0;

            player.ChangeState(player.SwingState);
        }
    }

    private void TryAttack()
    {
        if (attackCoroutine != null) return;

        attackCoroutine = player.StartCoroutine(Attack_1_Routine());
    }

    private Coroutine attackCoroutine;
    private IEnumerator Attack_1_Routine()
    {
        player.AnimationController.ChangeState(AnimationController.Player.Attack.Downward_1, 0);

        // todo: Use animation event instead of coroutine, to delay applying damage.
        yield return new WaitForSeconds(0.3f);

        if (Detector.CharactersDetected(attackPosition, attackRadius, out Collider[] characters))
        {
            foreach(Collider collider in characters)
            {
                var character = collider.GetComponentInParent<ZombieCharacter>();
                if (character is null) continue;

                character.TakeDamage(new DamageEvent() { damage = 10 });
            }
        }

        yield return new WaitForSeconds(0.3f);

        player.ChangeState(player.MoveState);
    }

    private IEnumerator Attack_2_Routine()
    {
        yield return null;
    }
}