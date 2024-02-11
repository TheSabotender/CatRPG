using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    public enum ConditionType
    {
        Variable,
        Random
    }    
    public enum ComparisonType
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanOrEquals,
        LessThanOrEquals
    }

    public ConditionType conditionType;
    public ComparisonType comparisonType;


    // Variables
    public string variableGuid;
    public string stringValue;
    public bool boolValue;
    public int intValue;


    public bool Validate()
    {
        switch (conditionType)
        {
            case ConditionType.Variable:
                Variable v = GuidDatabase.Find<Variable>(variableGuid);
                return ValidateVariable(v);                
            case ConditionType.Random:
                return Random.Range(0, 100f) > 50;
        }

        return true;
    }

    public bool ValidateVariable(Variable v)
    {
        switch (v.type)
        {
            case Variable.VariableType.String:
                switch (comparisonType)
                {
                    case ComparisonType.Equals:
                        return v.stringValue == stringValue;
                    case ComparisonType.NotEquals:
                        return v.stringValue != stringValue;
                }
                break;
            case Variable.VariableType.Bool:
                switch (comparisonType)
                {
                    case ComparisonType.Equals:
                        return v.boolValue == boolValue;
                    case ComparisonType.NotEquals:
                        return v.boolValue != boolValue;
                }
                break;
            case Variable.VariableType.Int:
                switch (comparisonType)
                {
                    case ComparisonType.Equals:
                        return v.intValue == intValue;
                    case ComparisonType.NotEquals:
                        return v.intValue != intValue;
                    case ComparisonType.GreaterThan:
                        return v.intValue > intValue;
                    case ComparisonType.LessThan:
                        return v.intValue < intValue;
                    case ComparisonType.GreaterThanOrEquals:
                        return v.intValue >= intValue;
                    case ComparisonType.LessThanOrEquals:
                        return v.intValue <= intValue;
                }
                break;
        }
        return false;
    }

    void EditorDraw()
    {

    }
}
