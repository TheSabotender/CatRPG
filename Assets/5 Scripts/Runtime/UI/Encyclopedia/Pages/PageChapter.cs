using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PageChapter : Page
{
    public TMPro.TextMeshProUGUI header;
    public Image icon;

    public void SetData(string header, Sprite icon)
    {
        this.header.text = header;
        this.icon.sprite = icon;
        this.icon.enabled = icon != null;
    }

    public override void ReloadData() => SetData(header.text, icon.sprite);

    public override bool Search(string text)
    {
        return header.text.ToLower().Contains(text.ToLower());
    }
}
