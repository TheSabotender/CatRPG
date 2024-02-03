using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class CharacterData : GuidScriptableObject
{
    public string displayName;    

    public Sprite portrait_small;
    public Sprite portrait_medium;

    [Header("Encyclopedia")]
    public int order;
    public Condition condition;
    [TextArea] public string brief;
    [TextArea] public string baseLore;
    public Page.ConditionalText[] additionalLore;

    public static CharacterData Find(string guid)
    {
        return GuidDatabase.Find<CharacterData>(guid);
    }
}