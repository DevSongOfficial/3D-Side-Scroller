using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class RequiredAttribute : PropertyAttribute
{
    /// <summary> 인포, 경고 박스를 표시할지 여부 </summary>
    public bool ShowMessageBox { get; set; } = true;

    /// <summary> Null일 때 디버그 에러를 호출할지 여부 </summary>
    public bool ShowLogError { get; set; } = false;

    public RequiredAttribute() { }
}
