using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ZombieMovementBase : ScriptableObject
{   // Crawl, Walk, Run1, Run2, .. 등등 depending on preference, 잔여 HP 등등..
    // 본 클래스 상속해서 움직이는 코드들만 구현, 좀비는 SO로 움직이는 방법 선택    

    private Animation.State animationType = Animation.State.None;
    public abstract void SetAnimationType();
    protected void SetAnimationType(Animation.State state) { animationType = state; }
    public Animation.State GetAnimationType() 
    {
        if(animationType == Animation.State.None)
        {
            Debug.LogWarning("Make sure to set the animation type");
        }
        return animationType; 
    }

    public abstract void Execute(ZombieCharacter zombie, Vector3 targetPosition, Rigidbody rigidbody);
}
