using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CinematicDialogueNode : CinematicBaseNode
{
    public string speakerGuid;
    public string text;

    public override IEnumerator Run()
    {
        Debug.Log("[Dialogue] " + speakerGuid + ": " + text);
        if (false)
            yield return null;
    }
}
