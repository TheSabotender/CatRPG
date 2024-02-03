using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cinematic", menuName = "Cinematic")]
public class Cinematic : ScriptableObject
{
    public string startNodeGuid;
    public bool blocksMenu;

    [SerializeReference]
    public List<CinematicBaseNode> nodes;
}