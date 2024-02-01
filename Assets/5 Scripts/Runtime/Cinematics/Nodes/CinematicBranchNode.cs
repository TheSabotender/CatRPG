using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CinematicBranchNode : CinematicBaseNode
{
    public enum Condition { True }

    [System.Serializable]
    public class Branch
    {        
        public Condition condition;
        public string conditionData;
        public string node;
    }

    public List<Branch> branches = new List<Branch>();
    public string elseBranch;

    public override IEnumerator Run()
    {
        nextNode = elseBranch;
        foreach (var branch in branches)
        {
            if (Validate(branch.condition, branch.conditionData))
            {
                nextNode = branch.node;
                break;
            }
        }

        if(false)
            yield return null;
    }

    public static bool Validate(Condition condition, string data)
    {
        return false;
    }
}
