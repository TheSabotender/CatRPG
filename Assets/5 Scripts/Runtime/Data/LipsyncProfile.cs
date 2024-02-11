using UnityEngine;

[CreateAssetMenu(fileName = "LipsyncProfile", menuName = "Lipsync Profile")]
public class LipsyncProfile : ScriptableObject
{
    [System.Serializable]
    public class Phoneme
    {
        public string blendshape;
        [Range(-100, 100)] public float weight;
        public string[] triggers;
    }

    public Phoneme[] phonemes;
}
