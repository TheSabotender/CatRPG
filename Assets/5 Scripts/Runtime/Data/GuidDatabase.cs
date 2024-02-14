using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New GuidDatabase", menuName = "GuidDatabase")]
public class GuidDatabase : ScriptableObject
{
    private static GuidDatabase _instance;

    public static GuidDatabase Instance
    {
        get
        {
            if (_instance == null)
            {   
                _instance = Resources.Load<GuidDatabase>("GuidDatabase");
            }
            return _instance;   
        }
    }

    public static void Add<T>(T guidScriptableObject) where T : GuidScriptableObject
    {
        foreach (var item in Instance.items)
        {
            if (item == guidScriptableObject)
            {
                return;
            }
        }
        Instance.items.Add(guidScriptableObject);

        foreach(var item in Instance.items)
        {
            if(item == null)
            {
                Instance.items.Remove(item);
                break;
            }
        }
    }

    [SerializeReference]
    public List<GuidScriptableObject> items;

    public static T Find<T>(string guid) where T : GuidScriptableObject
    {
        foreach (var item in Instance.items)
        {
            if (item.guid == guid)
            {
                return item as T;
            }
        }
        return null;
    }

    public static List<T> FindAll<T>() where T : GuidScriptableObject
    {
        return Instance.items.Where(i => i is T).Select(i => i as T).ToList();
    }

    public static void Remove<T>(T guidScriptableObject) where T : GuidScriptableObject
    {
        Instance.items.Remove(guidScriptableObject);
    }
}
