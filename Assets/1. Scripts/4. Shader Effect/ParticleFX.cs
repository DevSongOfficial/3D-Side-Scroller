using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFX : MonoBehaviour
{
    private new ParticleSystem particleSystem;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }
    public void Initialize(ParticleInfo info)
    {
        var main = particleSystem.main;
        main.startColor = info.color;
        main.startLifetime = info.lifeTime;
    }
}

public struct ParticleInfo
{
    public Color color;
    public float lifeTime;
}