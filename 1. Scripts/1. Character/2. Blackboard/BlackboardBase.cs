using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlackboardBase
{
    // The same as UE5's Blackboard System in Behaviour Tree
    // States 간 공용으로 사용할 Properties, Delegate 등 Key values 관리
    // Every property must be public

    /// <summary>
    /// <Builder Pattern을 통해 아래와 같이 Basic Components의 Accessability를 보호할 수 있음>
    /// public Animator animator { get; private set; }
    /// public BlackboardBase(Animator animator) { this.animator = animator; }
    /// </summary>

    // Constructors
    public BlackboardBase() { }


    // 좀비 행동에 필요한 데이터들을 구현하는 위치가
    // 1) ZombieCharacter에 직접 작성하는 경우 -> 접근제한자를 public으로 해야하므로 보안 취약해짐
    // 2) StateBase를 상속받는 ZomebieStateBase 만든 후 여기다 데이터 작성한 뒤 이를 다시 하위 State들에게 상속받게 하는 방법
    //    -> 보안 굿, 공용 X
    // 3) Blackboard 만들어서 parameter로 넘기기 -> 보안 굿 캡슐화 굿

    // 일단 좀비부터 (3)번 방식으로 시도해보고 괜찮으면 Player에도 적용해보기로..
}
