using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerInfo", menuName = "Scriptable Object/Player Info")]
public class PlayerInfo : CharacterInfo
{
    [SerializeField] private int jumpPower = 4;



    public int JumpPower => jumpPower;
}
