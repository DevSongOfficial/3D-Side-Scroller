using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ZombieMovementBase : ScriptableObject
{   // Crawl, Walk, Run1, Run2, .. ��� depending on preference, �ܿ� HP ���..
    // �� Ŭ���� ����ؼ� �����̴� �ڵ�鸸 ����, ����� SO�� �����̴� ��� ����    

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
