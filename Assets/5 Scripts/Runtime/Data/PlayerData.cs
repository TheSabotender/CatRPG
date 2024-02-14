using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // Save
    public string fileName;
    public string saveDate;

    // Stats
    public int playerLevel;
    public int playerExp;
    //equiped items

    // Progression    
    public Location location;
    public string currentQuest;
    public Reputation[] reputations;
    public Variable[] variables;
    //Unlocks
    //titles



    [System.Serializable]
    public class Location
    {
        public string scene;
        public Vector3 position;
        public float rotation;
    }

    [System.Serializable]
    public class Reputation
    {
        public string character;
        public int value;
    }

    [System.Serializable]
    public class Variable
    {
        public string guid;
        public string stringValue;
        public bool boolValue;
        public int intValue;
    }

    public void OnPreSave()
    {
        saveDate = DateTime.Now.ToString();
    }

    public void OnPostLoad()
    {

    }
}
