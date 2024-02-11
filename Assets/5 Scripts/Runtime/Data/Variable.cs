using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Variable", menuName = "Variable")]
public class Variable : GuidScriptableObject
{
    public enum VariableType
    {
        String,
        Bool,
        Int
    }
    public VariableType type;

    // Runtime
    [Header("Runtime values")]
    public string stringValue;
    public bool boolValue;
    public int intValue;

    public static void CleanVariables()
    {
        GuidDatabase.FindAll<Variable>().ForEach(
            (v) =>
            {
                v.intValue = 0;
                v.boolValue = false;
                v.stringValue = string.Empty;
            }
        );
    }
}
