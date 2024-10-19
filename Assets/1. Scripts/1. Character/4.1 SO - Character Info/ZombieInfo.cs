using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieInfo", menuName = "Scriptable Object/Character Info/Zombie Info")]
public class ZombieInfo : ObjectInfo
{
    [SerializeField][Range(1, 99)] private int mass = 7;

    [Header("Attack & Detection")]
    [Tooltip("Distance that ray checks for detecting objective")]
    [SerializeField] private float detectionDistance = 2;
    [SerializeField] private float attackRange = 1;


    public int Mass => mass;
    public float DetectionDistance => detectionDistance;
    public float AttackRange => attackRange;

    public override ZombieInfo AsZombieInfo()
    {
        return this;
    }
}
