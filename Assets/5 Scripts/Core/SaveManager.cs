using UnityEngine;

public static class SaveManager
{
    public static PlayerData _currentSaveGame;
    public static PlayerData CurrentSaveGame => _currentSaveGame;

    public static void NewGame(string playerName)
    {
        _currentSaveGame = new PlayerData();
        _currentSaveGame.fileName = "";
        _currentSaveGame.playerName = playerName;
        _currentSaveGame.reputations = new PlayerData.Reputation[0];
    }

    public static string[] GetSaveFiles()
    {
        string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath, "*.cat");
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = System.IO.Path.GetFileNameWithoutExtension(files[i]);
        }
        return files;
    }

    public static string LastSaveFile()
    {
        return PlayerPrefs.GetString("LastSaveFile", "");
    }

    public static PlayerData GetSaveInfo(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".cat";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            return JsonUtility.FromJson<PlayerData>(json);            
        }

        Debug.LogError("Save file not found!");
        return null;
    }

    public static bool Load(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".cat";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            _currentSaveGame = JsonUtility.FromJson<PlayerData>(json);
            return true;
        }

        Debug.LogError("Save file not found!");
        return false;
    }
    
    public static bool Save()
    {
        if(CurrentSaveGame == null)
        {
            Debug.LogError("No save game loaded!");
            return false;
        }

        if (string.IsNullOrEmpty(CurrentSaveGame.fileName))
        {
            Debug.LogError("Cannot quicksave!");
            return false;
        }

        Save(CurrentSaveGame.fileName);
        return true;
    }

    public static void Save(string fileName)
    {
        if (CurrentSaveGame == null)
        {
            Debug.LogError("No save game loaded!");
            return;
        }

        string json = JsonUtility.ToJson(CurrentSaveGame);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + fileName + ".cat", json);
        PlayerPrefs.SetString("LastSaveFile", fileName);
    }

    public static void Unload()
    {
        _currentSaveGame = null;
    }

    public static void Delete(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".cat";
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
    }
}
