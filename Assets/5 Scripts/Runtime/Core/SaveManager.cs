using PlasticPipe.PlasticProtocol.Messages;
using UnityEngine;

public static class SaveManager
{
    private static PlayerData _currentSaveGame;
    public static PlayerData CurrentSaveGame => _currentSaveGame;

    public static void NewGame(string playerName)
    {
        _currentSaveGame = new PlayerData();
        _currentSaveGame.fileName = "";
        _currentSaveGame.reputations = new PlayerData.Reputation[0];
        _currentSaveGame.location = new PlayerData.Location();

        var variables = GuidDatabase.FindAll<Variable>();
        _currentSaveGame.variables = new PlayerData.Variable[variables.Count];
        for(int i =  0; i < variables.Count; i++)
        {
            _currentSaveGame.variables[i] = new PlayerData.Variable() {
                guid = variables[i].guid,
                stringValue = string.Empty,
                boolValue = false,
                intValue = 0
            };
        }
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

    public static void Load(PlayerData file)
    {
        //Make sure to hide all overlays
        MainMenu.Hide();

        //Apply the save game
        _currentSaveGame = file;
        CurrentSaveGame.OnPostLoad();

        //Load the scene
        LocationData location = GuidDatabase.Find<LocationData>(CurrentSaveGame.location.scene);
        Loader.LoadScene(location.scene.BuildIndex, (gs) =>
        {

        });
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

        CurrentSaveGame.fileName = fileName;
        CurrentSaveGame.OnPreSave();

        string json = JsonUtility.ToJson(CurrentSaveGame);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + fileName + ".cat", json);
        PlayerPrefs.SetString("LastSaveFile", fileName);

        MainMenu.Refresh();
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
