using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerInfo", menuName = "Scriptable Object/Character Info/Player Info")]
public class PlayerInfo : CharacterInfo
{
    [SerializeField] private int jumpPower = 4;

    [Header("Attack")]
    [SerializeField] private Vector3 localPosition_Attack;
    [SerializeField] private float attackRadius;
    [SerializeField] private int attackDamage;

    [Header("Swing")]
    [SerializeField] private Vector3 localPosition_Swing;
    [SerializeField] private float swingRadius;
    [SerializeField] private int swingDamage;

    public int JumpPower => jumpPower;
    public Vector3 LocalPosition_Attack => localPosition_Attack;
    public float AttackRadius => attackRadius;
    public int AttackDamage => attackDamage;
    public Vector3 LocalPosition_Swing => localPosition_Swing;
    public float SwingRadius => swingRadius;
    public int SwingDamage => swingDamage;
}
