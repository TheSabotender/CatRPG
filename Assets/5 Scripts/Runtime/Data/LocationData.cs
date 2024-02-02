using Udar.SceneManager;
using UnityEngine;

[CreateAssetMenu(fileName = "New Location", menuName = "Location")]
public class LocationData : GuidScriptableObject
{
    public string displayName;
    public SceneField scene; 

    //public Location prefab;
}
