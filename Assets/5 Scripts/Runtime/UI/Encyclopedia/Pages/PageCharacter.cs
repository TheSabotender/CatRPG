using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageCharacter : Page
{
    public TMPro.TextMeshProUGUI characterName;
    public Image portrait;
    public Image image;

    public TMPro.TextMeshProUGUI brief;
    public TMPro.TextMeshProUGUI baseLore;
    public TMPro.TextMeshProUGUI additionalLore;

    private CharacterData c;

    public void SetData(CharacterData character)
    {
        c = character;

        if(characterName != null)
            characterName.text = c.displayName;

        if(portrait != null)
            portrait.sprite = c.portrait_small;

        if(image != null)
            image.sprite = c.portrait_medium;

        if(brief != null)
            brief.text = c.brief;

        if(baseLore != null)
            baseLore.text = c.baseLore;

        if(additionalLore != null)
        {
            additionalLore.text = string.Empty;
            foreach (var lore in c.additionalLore)
            {
                if (lore.condition.Validate())
                    {
                    additionalLore.text += lore.text + "\n\n";
                }
            }
        }
    }

    public override void ReloadData() => SetData(c);

    public override bool Search(string text)
    {
        return c.displayName.ToLower().Contains(text.ToLower());
    }
}
