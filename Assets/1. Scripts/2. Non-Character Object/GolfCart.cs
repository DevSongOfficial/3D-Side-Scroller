using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfCart : MonoBehaviour, IInteractable
{
    private CharacterBase driver;
    public event Action<CharacterBase> OnCharacterInteract;

    private void GetInTheCart(CharacterBase character)
    {
        driver = character;
        driver.FadeOut();
    }

    private void GetOutOfTheCart()
    {
        driver.FadeIn();
        driver = null;
    }

    public void Interact(CharacterBase character)
    {
        OnCharacterInteract?.Invoke(character);

        if (driver is null)
        {
            GetInTheCart(character);
        }
        else
        {
            GetOutOfTheCart();
        }
    }

    private void Update()
    {
        if (driver is null) return;

        Vector3 offset = new Vector3(0, 1.5f, 0);
        transform.position = driver.transform.position + offset;
    }
}