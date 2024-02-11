using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class CinematicBaseNode
{
    public string name;
    public string guid;    

    //Map stuff
    public Vector2 position;
    public string nextNode;

    public virtual IEnumerator Run()
    {
        yield return null;        
    }

#if UNITY_EDITOR
    public virtual int Connections => 1;

    public virtual Vector2 EditorSize => new Vector2(200, 120);


    public virtual void EditorDraw(float LABELWIDTH) { }
#endif
}
