using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieInfo", menuName = "Scriptable Object/Character Info/Zombie Info")]
public class ZombieInfo : CharacterInfo
{
    [Space(10)]
    [Header("Zombie")]
    [Tooltip("Distance that ray checks for detecting objective")]
    [SerializeField] private float detectionDistance = 2;
    [SerializeField] private float attackRange = 1;


    public float DetectionDistance => detectionDistance;
    public float AttackRange => attackRange;
}
