using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class ZombieMovementBase : ScriptableObject
{   // Crawl, Walk, Run1, Run2, .. depending on the preference or 잔여 HP or ...
    // 본 클래스 상속해서 움직이는 코드들만 구현, 좀비는 SO로 움직이는 방법 선택    

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
