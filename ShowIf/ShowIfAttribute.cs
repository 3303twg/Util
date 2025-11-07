using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)] // <--- 여기 중요
public class ShowIfAttribute : PropertyAttribute
{
    public string ConditionFieldName;

    public ShowIfAttribute(string conditionFieldName)
    {
        ConditionFieldName = conditionFieldName;
    }
}
