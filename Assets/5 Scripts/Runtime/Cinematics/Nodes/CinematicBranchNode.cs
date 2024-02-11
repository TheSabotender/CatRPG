using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

[System.Serializable]
public class CinematicBranchNode : CinematicBaseNode
{
    [System.Serializable]
    public class Branch
    {        
        public Condition condition;
        public string node;
    }

    public List<Branch> branches = new List<Branch>();
    public string elseBranch;

    public override IEnumerator Run()
    {
        nextNode = elseBranch;
        foreach (var branch in branches)
        {
            if (branch.condition.Validate())
            {
                nextNode = branch.node;
                break;
            }
        }

        if(false)
            yield return null;
    }

#if UNITY_EDITOR
    public override int Connections => branches.Count + 1;

    public override Vector2 EditorSize
    {
        get
        {
            int height = 140;
            foreach(var b in branches)
            {
                if(b.condition != null)
                {
                    height += 30;

                    switch (b.condition.conditionType)
                    {
                        case Condition.ConditionType.Variable:
                            height += 40;
                            break;
                        case Condition.ConditionType.Random:
                            break;
                    }                    
                }                
            }
            return new Vector2(200, height);
        }
    }


    public override void EditorDraw(float LABELWIDTH)
    {
        var branchNode = (CinematicBranchNode)this;
        EditorGUILayout.LabelField("Branches");
        branchNode.branches.ForEach(branch =>
        {
            using (new GUILayout.VerticalScope("box"))
            {
                if(branch.condition == null)
                    branch.condition = new Condition();
                EditorDrawCondition(branch.condition, LABELWIDTH);
            }
        });
        if (GUILayout.Button("Add Branch"))
        {
            branchNode.branches.Add(new CinematicBranchNode.Branch());
        }
        EditorGUILayout.LabelField("Else");
    }

    void EditorDrawCondition(Condition condition, float LABELWIDTH)
    {
        // Select condition type (variable or random)
        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Condition Type", GUILayout.Width(LABELWIDTH));
            var conditionType = (Condition.ConditionType)EditorGUILayout.EnumPopup(condition.conditionType);
            if (conditionType != condition.conditionType)
            {
                condition.conditionType = conditionType;
                condition.stringValue = "";
                condition.boolValue = false;
                condition.intValue = 0;
            }
        }


        switch (condition.conditionType)
        {
            case Condition.ConditionType.Variable:
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Variable", GUILayout.Width(LABELWIDTH));
                    List<Variable> allVars = GuidDatabase.FindAll<Variable>();
                    if (allVars.Count > 0)
                    {
                        int selected = 0;
                        var selectedList = allVars.Where(c => c.guid == condition.variableGuid);
                        if (selectedList.Count() == 1)
                            selected = allVars.IndexOf(selectedList.First());

                        int varIndex = EditorGUILayout.Popup(selected, allVars.Select(c => c.name).ToArray());
                        condition.variableGuid = allVars[varIndex].guid;
                    }
                    else
                    {
                        condition.variableGuid = EditorGUILayout.TextField(condition.variableGuid);
                    }
                }

                var variable = GuidDatabase.Find<Variable>(condition.variableGuid);
                if (variable != null)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        var basicComp = new string[2] { "Equals", "NotEquals" };

                        switch (variable.type)
                        {
                            case Variable.VariableType.String:
                                condition.comparisonType = (Condition.ComparisonType)EditorGUILayout.Popup(System.Array.IndexOf(basicComp, condition.comparisonType.ToString()), basicComp, GUILayout.Width(LABELWIDTH));
                                condition.stringValue = EditorGUILayout.TextField(condition.stringValue);
                                break;
                            case Variable.VariableType.Bool:
                                condition.comparisonType = (Condition.ComparisonType)EditorGUILayout.Popup(System.Array.IndexOf(basicComp, condition.comparisonType.ToString()), basicComp, GUILayout.Width(LABELWIDTH));
                                condition.boolValue = EditorGUILayout.Toggle(condition.boolValue);
                                break;
                            case Variable.VariableType.Int:
                                condition.comparisonType = (Condition.ComparisonType)EditorGUILayout.EnumPopup(condition.comparisonType, GUILayout.Width(LABELWIDTH));
                                condition.intValue = EditorGUILayout.IntField(condition.intValue);
                                break;
                        }
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Variable", "None");
                }
                break;
            case Condition.ConditionType.Random:
                break;
        }        
    }
    #endif
}
