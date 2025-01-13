using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class Utility
{
    public static bool CompareState(this StateBase state, StateBase another)
    {
        return ReferenceEquals(state, another);
    }

    public static Vector3 ConvertToVector3(this MovementDirection direction)
    {
        switch (direction)
        {
            case MovementDirection.Right:
                return Vector3.right;
            case MovementDirection.Left:
                return Vector3.left;
            default:
                return Vector3.zero;
        }
    }

    public static MovementDirection GetFlippedDirection(this MovementDirection direction)
    {
        switch (direction)
        {
            case MovementDirection.Right:
                return MovementDirection.Left;
            case MovementDirection.Left:
                return MovementDirection.Right;
            default:
                return MovementDirection.None;
        }
    }

    public static float GetPositionZ(this ZAxisMovementDirection direction)
    {
        return direction == ZAxisMovementDirection.Up ? 0 : -1.6f;
    }

    public static int GetYRotationValue(this MovementDirection direction)
    {
        switch (direction)
        {
            case MovementDirection.Left:
                return CharacterMovementController.YAngle_Left;
            case MovementDirection.Right:
                return CharacterMovementController.YAngle_Right;
            default:
                return 0;
        }
    }

    public static Vector3 ChangeVectorXWithDirection(this Vector3 vector, MovementDirection direction)
    {
        return new Vector3(vector.x * (int)direction, vector.y, vector.z);
    }

    public static int GetMask(this Layer layer)
    {
        return 1 << (int)layer;
    }

    public static void SetLayer(this GameObject go, Layer layer)
    {
        go.layer = (int)layer;
    }

    public static bool CompareLayer(this int layer, Layer layerToCompare)
    {
        return layer == (int)layerToCompare;
    }

    public static bool CompareLayer(this Collider collider, Layer layerToCompare)
    {
        return collider.gameObject.layer == (int)layerToCompare;
    }

    public static bool CompareLayer(this Collision collision, Layer layerToCompare)
    {
        return collision.gameObject.layer == (int)layerToCompare;
    }

    public static void SetTag(this GameObject go, Tag tag)
    {
        go.tag = tag.ToString();
    }

    public static bool CompareTag(this Collider collider, Tag tag)
    {
        return collider.gameObject.CompareTag(tag.ToString());
    }

    public static bool BetterThan(this ScoreType scoreType, ScoreType scoreToCompare)
    {
        if (scoreToCompare == ScoreType.InComplete) return true;
        else return (int)scoreType < (int)scoreToCompare;
    }

    public static bool IsWithinTolerance(float current, float target, float tolerance)
    {
        return Mathf.Abs(Mathf.DeltaAngle(current, target)) <= tolerance;
    }

    public static T StringToEnum<T>(string e)
    {
        return (T)Enum.Parse(typeof(T), e);
    }

    public static string FromBASE64(this string s)
    {
        var bytes = Convert.FromBase64String(s);
        return Encoding.UTF8.GetString(bytes);
    }

    public static string ToBASE64(this string s)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
        return Convert.ToBase64String(bytes);
    }

    public static Dictionary<string, object> ToStageData(this string rawData)
    {
        var convertedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawData);
        var stageData = JsonConvert.DeserializeObject<Dictionary<string, object>>(convertedData["data"].ToString());
        return stageData;
    }

    public static Dictionary<string, object> ToStagesData(this string rawData)
    {
        if (string.IsNullOrEmpty(rawData)) return null; 

        var convertedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawData);
        var stageData = JsonConvert.DeserializeObject<Dictionary<string, object>>(convertedData["documents"].ToString());
        return stageData;
    }

    public static int GetCloserNumber(float number , int num1, int num2)
    {
        return Mathf.Abs(number - num1) > Math.Abs(number - num2) ? num2 : num1 ;
    }

    public static float GetBiggerNumber(float num1, float num2)
    {
        return num1 > num2 ? num1 : num2;
    }

    public static int GetLayerMask(Layer layer1, Layer layer2)
    {
        return 1 << (int)layer1 | 1 << (int)layer2;
    }

    public static int GetLayerMask(Layer layer1, Layer layer2, Layer layer3)
    {
        return 1 << (int)layer1 | 1 << (int)layer2 | 1 << (int)layer3;
    }

    public static IEnumerator FadeInRoutine(Image image, float speed = 1)
    {
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += speed * Time.fixedDeltaTime;
            image.color = new Color(0, 0, 0, alpha);
            yield return new WaitForFixedUpdate();
        }
    }

    public static IEnumerator FadeOutRoutine(Image image, float speed = 1)
    {
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= speed * Time.fixedDeltaTime;
            image.color = new Color(0, 0, 0, alpha);
            yield return new WaitForFixedUpdate();
        }
    }

    public static string GenerateSN()
    {
        string date = DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss tt"));
        string result = "";
        for (int i = 0; i < 5; i++)
        {
            char randomChar = (char)UnityEngine.Random.Range(65, 91);
            result += randomChar;
        }

        return result;
    }

    public class Timer
    {
        private event Action OnTimerEnd;
        
        public float TimeLeft { get ; private set; }
        public Timer(Action callback, float duration)
        {
            TimeLeft = duration;
            OnTimerEnd = callback;
        }

        public bool Tick(float deltaTime)
        {
            if (TimeLeft <= 0) return true;

            TimeLeft -= deltaTime;

            if(TimeLeft <= 0)
            {
                OnTimerEnd.Invoke();
            }

            return false;
        }
    }
}
