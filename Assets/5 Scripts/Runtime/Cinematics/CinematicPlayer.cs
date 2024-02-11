using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class CinematicPlayer : MonoBehaviour
{
    public Cinematic cinematic;
    public bool playOnAwake;    

    private Coroutine coroutine;

    public static bool IsInBlockingCutscene { get; private set; }

    private void Start()
    {
        if (playOnAwake)
        {
            Play(null);
        }
    }


    public void Play(Action onComplete)
    {
        Play(cinematic, onComplete);
    }

    public void Play(Cinematic cinematic, Action onComplete)
    {
        if(coroutine != null)
        {
            Debug.LogWarning("Cinematic is already playing");
            return;
        }

        coroutine = StartCoroutine(RunCinematic(cinematic, onComplete));
    }

    public static IEnumerator RunCinematic(Cinematic cinematic, Action onComplete)
    {
        IsInBlockingCutscene = cinematic.blocksMenu;
        CinematicBaseNode currentNode = cinematic.nodes.Where(n => n.guid == cinematic.startNodeGuid).First();

        while (currentNode != null)
        {
            Debug.Log("Running node: " + currentNode.name);
            yield return currentNode.Run();

            CinematicBaseNode nextNode = null;
            var nodes = cinematic.nodes.Where(n => n.guid == currentNode.nextNode);
            if(nodes.Count() > 0)
                nextNode = nodes.First();            

            // Hide dialogue if the next node is not a dialogue or options node
            if (currentNode is CinematicDialogueNode && nextNode is not CinematicDialogueNode)
                Dialogue.HideDialogue();

            currentNode = nextNode;
        }

        IsInBlockingCutscene = false;
        onComplete?.Invoke();
    }
}
