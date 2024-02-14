using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMenu_Data : Page
{
    public SaveDisplay display;
    public GameObject empty;

    public void Setup(PlayerData data)
    {
        display.gameObject.SetActive(data != null);
        empty.SetActive(data == null);

        if(data != null)
            display.Setup(data);
    }
}
