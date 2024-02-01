using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CinematicWaitNode : CinematicBaseNode
{
    public float waitTime;

    override public IEnumerator Run()
    {
        yield return new WaitForSeconds(waitTime);
    }
}
