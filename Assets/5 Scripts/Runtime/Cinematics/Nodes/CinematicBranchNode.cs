using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CinematicBranchNode : CinematicBaseNode
{
    public class Branch
    {        
        public string condition;
        public string node;
    }

    public List<Branch> branches;

    public override IEnumerator Run()
    {
        foreach (var branch in branches)
        {
            if (branch.condition == "true")
            {
                nextNode = branch.node;
                break;
            }
        }

        if(false)
            yield return null;
    }
}
