using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static T StringToEnum<T>(string e)
    {
        return (T)Enum.Parse(typeof(T), e);
    }

    public static int GetCloserNumber(float number , int num1, int num2)
    {
        return Mathf.Abs(number - num1) > Math.Abs(number - num2) ? num2 : num1 ;
    }

    public static float GetBiggerNumber(float num1, float num2)
    {
        return num1 > num2 ? num1 : num2;
    }
}
