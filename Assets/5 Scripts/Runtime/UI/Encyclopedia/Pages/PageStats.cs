using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageStats : Page
{
    public TMPro.TextMeshProUGUI heroName;

    protected override void OnReloaded()
    {
        if(heroName != null)
            heroName.text = SaveManager.CurrentSaveGame?.playerName;
    }
}
