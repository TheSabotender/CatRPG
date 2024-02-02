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
    public override Vector2 EditorSize => new Vector2(200, 140 + branches.Count * 50);

    public override void EditorDraw(float LABELWIDTH)
    {
        var branchNode = (CinematicBranchNode)this;
        EditorGUILayout.LabelField("Branches");
        branchNode.branches.ForEach(branch =>
        {
            using (new GUILayout.VerticalScope("box"))
            {
                /*
                branch.condition = (Condition)EditorGUILayout.EnumPopup("Condition", branch.condition);
                if (branch.condition == CinematicBranchNode.Condition.True)
                {
                    branch.conditionData = EditorGUILayout.TextField("Variable", branch.conditionData);
                }
                */
            }
        });
        if (GUILayout.Button("Add Branch"))
        {
            branchNode.branches.Add(new CinematicBranchNode.Branch());
        }
        EditorGUILayout.LabelField("Else");
    }
    #endif
}
