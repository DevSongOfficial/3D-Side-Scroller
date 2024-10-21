using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public sealed class ZombieBlackboard : BlackboardBase
{
    // Properties
    public CharacterBase targetCharacter;
    public bool isDead;

    // Shader effects
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public List<Material> skinMaterials;
}