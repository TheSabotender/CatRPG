using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SaveDisplay : MonoBehaviour
{
    public Image thumbnail;
    public TMPro.TextMeshProUGUI saveName;
    public TMPro.TextMeshProUGUI saveDate;

    public TMPro.TextMeshProUGUI playerName;
    public TMPro.TextMeshProUGUI playerLevel;

    public Action onClick;
    public Action onHover;

    public Variable nameVariable;

    public void Setup(PlayerData data)
    {
        if(data == null)
        {
            if (saveName != null)
                saveName.text = "New file";
            if (saveDate != null)
                saveDate.text = string.Empty;

            if (playerName != null)
                playerName.text = string.Empty;
            if (playerLevel != null)
                playerLevel.text = string.Empty;

            return;
        }

        if (saveName != null)
            saveName.text = data.fileName;
        if (saveDate != null)
            saveDate.text = data.saveDate;

        if (playerName != null && nameVariable != null)
        {
            var vars = data.variables.Where(v => v.guid == nameVariable.guid);
            var var = vars.FirstOrDefault();
            playerName.text = var.stringValue;
        }
        if (playerLevel != null)
            playerLevel.text = $"Level {data.playerLevel}";
    }

    public void Btn_Click()
    {
        onClick?.Invoke();
    }

    public void Btn_Hover()
    {
        onHover?.Invoke();
    }
}
