using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public sealed class ShaderFX : MonoBehaviour
{
    [SerializeField] private string referenceName;

    public Renderer Renderer { get; private set; }
    private bool isSharedMaterial;

    public void Initialize(bool isSharedMaterial)
    {
        Renderer = GetComponent<Renderer>();
        this.isSharedMaterial = isSharedMaterial;
    }

    public void SetFloat(float value)
    {
        try
        {
            if (isSharedMaterial) Renderer.sharedMaterial.SetFloat(referenceName, value);
            else Renderer.material.SetFloat(referenceName, value);
        }
        catch(Exception e)
        {
            Debug.Log($"Unvalid value or name - {e.Message}");
        }
    }

    public float GetFloat()
    {
        try
        {
            if (isSharedMaterial) return Renderer.sharedMaterial.GetFloat(referenceName);
            else return Renderer.material.GetFloat(referenceName);
        }
        catch (Exception e)
        {
            Debug.Log($"Unvalid value or name - {e.Message}");
            return 0f;
        }
    }

    public void SmoothDecreaseFloat(float target, float speed = 1)
    {
        if(SmoothDecreaseCoroutine != null) StopCoroutine(SmoothDecreaseCoroutine);
        SmoothDecreaseCoroutine = StartCoroutine(SmoothDecreaseRoutine(target, speed));
    }

    private Coroutine SmoothDecreaseCoroutine;
    private IEnumerator SmoothDecreaseRoutine(float target, float speed)
    {
        float value = GetFloat();
        while(GetFloat() > target)
        {
            value -= Time.fixedDeltaTime * speed;
            SetFloat(value);

            yield return new WaitForFixedUpdate();
        }

        SetFloat(target);
    }
}
