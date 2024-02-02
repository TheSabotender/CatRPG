using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class CinematicWaitNode : CinematicBaseNode
{
    public float waitTime;

    override public IEnumerator Run()
    {
        yield return new WaitForSeconds(waitTime);
    }

#if UNITY_EDITOR    
    public override Vector2 EditorSize => new Vector2(200, 100);

    public override void EditorDraw(float LABELWIDTH)
    {
        var waitNode = (CinematicWaitNode)this;
        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Seconds", GUILayout.Width(LABELWIDTH));
            waitNode.waitTime = EditorGUILayout.FloatField(waitNode.waitTime);
        }
    }
#endif
}
