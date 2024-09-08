using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfCart : MonoBehaviour, IInteractable
{
    private InteractionInfo interactorInfo;

    private void FixedUpdate()
    {
        MoveTheCart();
    }

    private void MoveTheCart()
    {
        if (interactorInfo is null) return;

        Vector3 offset = new Vector3(0, 1.5f, 0);
        transform.position = interactorInfo.GetPosition() + offset;
    }

    private void GetInTheCart()
    {
        //driver = character;
        //driver.FadeOut();
    }

    private void GetOutOfTheCart()
    {
        //driver.FadeIn();
        interactorInfo = null;
    }

    public void Interact(InteractionInfo info)
    {
        if (interactorInfo is null)
        {
            interactorInfo = info;
            GetInTheCart();
        }
        else
        {
            GetOutOfTheCart();
        }
    }
}