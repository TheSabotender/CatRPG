using System.Collections.Generic;
using System.Linq;
using Udar.SceneManager;
using UnityEngine;

[CreateAssetMenu(fileName = "Cinematic", menuName = "Cinematic")]
public class Cinematic : ScriptableObject
{
    public string startNodeGuid;

    [SerializeReference]
    public List<CinematicBaseNode> nodes;
}