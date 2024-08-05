using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool CompareState(this StateBase state, StateBase another)
    {
        return ReferenceEquals(state, another);
    }
}   
