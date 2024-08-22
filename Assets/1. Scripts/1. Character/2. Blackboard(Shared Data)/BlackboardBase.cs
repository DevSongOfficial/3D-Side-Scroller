using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlackboardBase
{
    // The same as UE5's Blackboard System in Behaviour Tree
    // States 간 공용으로 사용할 Properties, Delegate 등 Key values 관리
    // Every property must be public

    // Builder Pattern을 통해 아래와 같이 Basic Components의 Accessability를 보호할 수 있음>
    /// public Animator animator { get; private set; }
    /// public BlackboardBase(Animator animator) { this.animator = animator; }

    // Constructors
    public BlackboardBase() { }
}
