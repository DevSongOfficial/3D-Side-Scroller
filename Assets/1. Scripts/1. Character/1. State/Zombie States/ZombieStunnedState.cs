using System.Collections.Generic;
using UnityEngine;

public class ZombieStunnedState : ZombieStateBase
{
    public ZombieStunnedState(ZombieCharacter zombieCharacter, ZombieBlackboard zomebieBlackboard) : base(zombieCharacter, zomebieBlackboard) { }

    private float dissolveProgress;

    public override void EnterState()
    {
        base.EnterState();

        zombie.MovementController.StunCharacter();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        DissoveOutOnDie();
        SwitchToPatrolState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void DissoveOutOnDie()
    {
        if (!blackBoard.isDead) return;

        dissolveProgress += 0.45f * Time.deltaTime;
        blackBoard.skinnedMeshRenderer.GetMaterials(blackBoard.skinMaterials);

        for (int i = 0; i < blackBoard.skinMaterials.Count; i++)
        {
            blackBoard.skinMaterials[i].SetFloat("_Dissolve_Ratio", dissolveProgress);
        }
    }

    private void SwitchToPatrolState()
    {
        if (blackBoard.isDead) return;
        if (zombie.MovementController.IsStunned) return;
        
        zombie.ChangeState(zombie.PatrolState);
    }
}