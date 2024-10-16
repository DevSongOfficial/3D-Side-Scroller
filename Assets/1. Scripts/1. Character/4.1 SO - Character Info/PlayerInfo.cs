using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerInfo", menuName = "Scriptable Object/Character Info/Player Info")]
public class PlayerInfo : ObjectInfo
{
    [SerializeField] [Range(0, 99)] private int jumpPower = 4;

    [Header("Attack")]
    [SerializeField] private Vector3 localPosition_Attack;
    [SerializeField] private float attackRadius;
    [SerializeField] private int attackDamage;

    [Header("Swing")]
    [SerializeField] private Vector3 localPosition_Swing;

    public int JumpPower => jumpPower;
    public Vector3 LocalPosition_Attack => localPosition_Attack;
    public float AttackRadius => attackRadius;
    public Vector3 LocalPosition_Swing => localPosition_Swing;

    public override PlayerInfo AsPlayerInfo()
    {
        return this;
    }
}
