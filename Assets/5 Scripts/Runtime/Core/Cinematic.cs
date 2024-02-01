using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cinematic", menuName = "Cinematic")]
public class Cinematic : ScriptableObject
{
    public string startNodeGuid;

    [SerializeReference]
    public List<CinematicBaseNode> nodes;
}