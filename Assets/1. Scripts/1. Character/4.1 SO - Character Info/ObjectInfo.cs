using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfo", menuName = "Scriptable Object/Character Info/Character Info")]
public class ObjectInfo : ScriptableObject
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private Vector3 pickupRange = new Vector3(1, 1, 1);

    [Header("Movement")]
    [SerializeField] private int movementSpeed = 5;
    [SerializeField] private float acceleration = 0.1f;


    public int MaxHealth => maxHealth;
    public int MovementSpeed => movementSpeed;
    public float Acceleration => acceleration;

    public float InteractionRange => interactionRange;
    public Vector3 PickupRange => pickupRange;

    public virtual PlayerInfo AsPlayerInfo()
    {
        return null;
    }

    public virtual ZombieInfo AsZombieInfo()
    {
        return null;
    }
}