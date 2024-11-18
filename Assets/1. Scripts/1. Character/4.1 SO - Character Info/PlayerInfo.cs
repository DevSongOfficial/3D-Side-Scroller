using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerInfo", menuName = "Scriptable Object/Character Info/Player Info")]
public class PlayerInfo : ObjectInfo
{
    [SerializeField] [Range(0, 99)] private int jumpPower = 4;
    [SerializeField][Range(1, 99)] private int mass = 7;

    [Header("Attack")]
    [SerializeField] private Vector3 localPosition_Attack;
    [SerializeField] private float attackRadius;
    [SerializeField] private int attackDamage;
    [SerializeField] private float damageDelay = 0.3f;

    [Header("Swing")]
    [SerializeField] private Vector3 localPosition_Swing;

    public int JumpPower => jumpPower;
    public int Mass => mass;

    public Vector3 LocalPosition_Attack => localPosition_Attack;
    public float AttackRadius => attackRadius;
    public float DamageDelay => damageDelay;

    public Vector3 LocalPosition_Swing => localPosition_Swing;

    public override PlayerInfo AsPlayerInfo()
    {
        return this;
    }
}
