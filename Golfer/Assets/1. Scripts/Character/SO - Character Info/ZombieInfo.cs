using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieInfo", menuName = "Scriptable Object/Zombie Info")]
public class ZombieInfo : CharacterInfo
{
    [Space(10)]
    [Header("Zombie")]
    [SerializeField] private float detectionDistance = 5;


    public float DetectionDistance => detectionDistance;
}
