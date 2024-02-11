using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CinematicSetVariableNode : CinematicBaseNode
{
    public enum SetType
    {
        Set,
        Add,
        Subtract
    }

    public string variableGuid;

    public SetType setType;

    public string stringValue;
    public bool boolValue;
    public int intValue;

    public override IEnumerator Run()
    {
        Variable variable = GuidDatabase.Find<Variable>(variableGuid);
        if (variable == null)
        {
            Debug.LogError("Variable not found");
            yield break;
        }

        switch (variable.type)
        {
            case Variable.VariableType.String:
                variable.stringValue = stringValue;
                break;
            case Variable.VariableType.Bool:
                variable.boolValue = boolValue;
                break;
            case Variable.VariableType.Int:
                switch (setType)
                {
                    case SetType.Set:
                        variable.intValue = intValue;
                        break;
                    case SetType.Add:
                        variable.intValue += intValue;
                        break;
                    case SetType.Subtract:
                        variable.intValue -= intValue;
                        break;
                }
                break;
        }
    }

#if UNITY_EDITOR
    public override int Connections => base.Connections;

    public override Vector2 EditorSize => new Vector2(200, 120);

    public override void EditorDraw(float LABELWIDTH)
    {
        var setVariableNode = (CinematicSetVariableNode)this;

        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Variable", GUILayout.Width(LABELWIDTH));
            List<Variable> allVars = GuidDatabase.FindAll<Variable>();
            if (allVars.Count > 0)
            {
                int selected = 0;
                var selectedList = allVars.Where(c => c.guid == setVariableNode.variableGuid);
                if (selectedList.Count() == 1)
                    selected = allVars.IndexOf(selectedList.First());

                int varIndex = EditorGUILayout.Popup(selected, allVars.Select(c => c.name).ToArray());
                setVariableNode.variableGuid = allVars[varIndex].guid;
            }
            else
            {
                setVariableNode.variableGuid = EditorGUILayout.TextField(setVariableNode.variableGuid);
            }
        }

        Variable variable = GuidDatabase.Find<Variable>(setVariableNode.variableGuid);

        if (variable != null)
        {
            using (new GUILayout.HorizontalScope())
            {
                switch (variable.type)
                {
                    case Variable.VariableType.String:
                        setVariableNode.stringValue = EditorGUILayout.TextField(setVariableNode.stringValue);
                        break;
                    case Variable.VariableType.Bool:
                        GUILayout.FlexibleSpace();
                        GUI.enabled = setVariableNode.boolValue;
                        GUI.color = !setVariableNode.boolValue ? Color.green : Color.red;
                        if(GUILayout.Button("False")) setVariableNode.boolValue = false;
                        GUI.enabled = !setVariableNode.boolValue;
                        GUI.color = setVariableNode.boolValue ? Color.green : Color.red;
                        if (GUILayout.Button("True")) setVariableNode.boolValue = true;
                        GUILayout.FlexibleSpace();

                        GUI.enabled = true;
                        GUI.color = Color.white;
                        break;
                    case Variable.VariableType.Int:
                        setVariableNode.setType = (SetType)EditorGUILayout.EnumPopup(setVariableNode.setType);
                        setVariableNode.intValue = EditorGUILayout.IntField(setVariableNode.intValue);
                        break;
                }
            }
        }
    }
#endif
}