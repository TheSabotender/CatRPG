using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string fileName;

    //Customizing
    public string playerName;

    //Stats
    public int playerLevel;
    public int playerExp;

    //Progression    
    public string playerLocation;
    public Vector3 playerPosition;
    public float playerRotation;
    public string currentQuest;
    public Reputation[] reputations;



    [System.Serializable]
    public class Reputation
    {
        public string character;
        public int value;
    }
}
