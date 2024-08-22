using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class RequiredAttribute : PropertyAttribute
{
    /// <summary> ����, ��� �ڽ��� ǥ������ ���� </summary>
    public bool ShowMessageBox { get; set; } = true;

    /// <summary> Null�� �� ����� ������ ȣ������ ���� </summary>
    public bool ShowLogError { get; set; } = false;

    public RequiredAttribute() { }
}
