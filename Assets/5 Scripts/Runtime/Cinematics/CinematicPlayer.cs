using System;
using System.Collections;
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

            var nodes = cinematic.nodes.Where(n => n.guid == currentNode.nextNode);
            if(nodes.Count() > 0)
                currentNode = nodes.First();
            else
                currentNode = null;
        }

        onComplete?.Invoke();
    }
}
