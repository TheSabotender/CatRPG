using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageStats : Page
{
    public TMPro.TextMeshProUGUI heroName;

    public void SetData()
    {
        if(this.heroName != null)
            this.heroName.text = SaveManager.CurrentSaveGame?.playerName;
    }

    public override void ReloadData() => SetData();
}
