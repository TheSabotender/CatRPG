using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PageStats : Page
{
    public TMPro.TextMeshProUGUI heroName;
    public Variable nameVariable;

    public void SetData()
    {
        if(this.heroName != null)
        {
            if(SaveManager.CurrentSaveGame != null)
            {
                var vars = SaveManager.CurrentSaveGame.variables.Where(v => v.guid == nameVariable.guid);
                var var = vars.FirstOrDefault();
                this.heroName.text = var.stringValue;
            } else
            {
                this.heroName.text = string.Empty;
            }
        }
    }

    public override void ReloadData() => SetData();
}
