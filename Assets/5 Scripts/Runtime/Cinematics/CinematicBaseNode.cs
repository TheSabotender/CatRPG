using System.Collections;
using UnityEngine;

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
}
