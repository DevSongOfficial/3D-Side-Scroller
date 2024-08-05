using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class ZombieMovementBase : ScriptableObject
{   // Crawl, Walk, Run1, Run2, .. depending on the preference or �ܿ� HP or ...
    // �� Ŭ���� ����ؼ� �����̴� �ڵ�鸸 ����, ����� SO�� �����̴� ��� ����    

    #region Animation
    private string animationType = AnimationController.State.None;
    public abstract void SetAnimationType();
    protected void SetAnimationType(string state) { animationType = state; }
    public string GetAnimationType() 
    {
        if(animationType == AnimationController.State.None)
        {
            Debug.LogWarning("Make sure to set the animation type");
        }
        return animationType; 
    }
    #endregion

    public abstract void Execute
        (MovementController movementController, CharacterInfo characterInfo, EMovementDirection wishDirection);
}
