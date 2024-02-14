using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Variable", menuName = "Variable")]
public class Variable : GuidScriptableObject
{
    public enum VariableType
    {
        String,
        Bool,
        Int
    }
    public VariableType type;

    public string stringValue
    {
        get
        {
            var save = SaveManager.CurrentSaveGame;
            if (save != null)
            {
                if (save.variables != null && save.variables.Length > 0)
                {
                    var matching = save.variables.Where(v => v.guid == guid);
                    if (matching.Count() > 0 && matching.First() != null)
                        return matching.First().stringValue;
                }
            }
            return string.Empty;
        }
        set
        {
            var save = SaveManager.CurrentSaveGame;
            if (save != null)
            {
                if (save.variables != null && save.variables.Length > 0)
                {
                    var matching = save.variables.Where(v => v.guid == guid);
                    if (matching.Count() > 0 && matching.First() != null)
                        matching.First().stringValue = value;
                }
            }
        }
    }

    public bool boolValue
    {
        get
        {
            var save = SaveManager.CurrentSaveGame;
            if (save != null)
            {
                if (save.variables != null && save.variables.Length > 0)
                {
                    var matching = save.variables.Where(v => v.guid == guid);
                    if (matching.Count() > 0 && matching.First() != null)
                        return matching.First().boolValue;
                }
            }
            return false;
        }
        set
        {
            var save = SaveManager.CurrentSaveGame;
            if (save != null)
            {
                if (save.variables != null && save.variables.Length > 0)
                {
                    var matching = save.variables.Where(v => v.guid == guid);
                    if (matching.Count() > 0 && matching.First() != null)
                        matching.First().boolValue = value;
                }
            }
        }
    }

    public int intValue
    {
        get
        {
            var save = SaveManager.CurrentSaveGame;
            if (save != null)
            {
                if (save.variables != null && save.variables.Length > 0)
                {
                    var matching = save.variables.Where(v => v.guid == guid);
                    if (matching.Count() > 0 && matching.First() != null)
                        return matching.First().intValue;
                }
            }
            return 0;
        }
        set
        {
            var save = SaveManager.CurrentSaveGame;
            if (save != null)
            {
                if (save.variables != null && save.variables.Length > 0)
                {
                    var matching = save.variables.Where(v => v.guid == guid);
                    if (matching.Count() > 0 && matching.First() != null)
                        matching.First().intValue = value;
                }
            }
        }
    }
}
