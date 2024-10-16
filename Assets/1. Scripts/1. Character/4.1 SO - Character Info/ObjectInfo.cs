using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfo", menuName = "Scriptable Object/Character Info/Character Info")]
public class ObjectInfo : ScriptableObject
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 2f;

    [Header("Movement")]
    [SerializeField] private int movementSpeed = 5;
    [SerializeField] private float acceleration = 0.1f;
    [SerializeField][Range(1, 99)] private int mass = 1;


    public int MaxHealth => maxHealth;
    public int MovementSpeed => movementSpeed;
    public float Acceleration => acceleration;
    public int Mass => mass;

    public float InteractionRange => interactionRange;

    public virtual PlayerInfo AsPlayerInfo()
    {
        return null;
    }

    public virtual ZombieInfo AsZombieInfo()
    {
        return null;
    }
}