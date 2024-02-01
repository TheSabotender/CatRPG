using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CinematicPlayer : MonoBehaviour
{
    public Cinematic cinematic;
    public bool playOnAwake;

    private Coroutine coroutine;

    private void Start()
    {
        if (playOnAwake)
        {
            Play(null);
        }
    }


    public void Play(Action onComplete)
    {
        if(coroutine != null)
        {
            Debug.LogWarning("Cinematic is already playing");
            return;
        }

        coroutine = StartCoroutine(RunCinematic(onComplete));
    }

    public IEnumerator RunCinematic(Action onComplete)
    {
        CinematicBaseNode currentNode = cinematic.nodes.Where(n => n.guid == cinematic.startNodeGuid).First();

        while (currentNode != null)
        {
            yield return currentNode.Run();
            currentNode = cinematic.nodes.Where(n => n.guid == currentNode.nextNode).First();
        }

        onComplete?.Invoke();
    }
}
