using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor window for displaying and editing save game data
/// </summary>
public class SaveEditorWindow : EditorWindow
{
    private Vector2 scrollPos;
    private string[] saveNames;

    [MenuItem("Dark Codex/Save Editor")]
    public static void ShowWindow()
    {
        GetWindow<SaveEditorWindow>("Save Editor");
    }

    private void OnGUI()
    {
        if(saveNames == null)
            saveNames = SaveManager.GetSaveFiles();

        using (new EditorGUILayout.HorizontalScope())
        {
            DrawSaveList();
            if(SaveManager.CurrentSaveGame == null)
                DrawNoSave();
            else
                DrawCurrentSave();
        }
    }

    private void DrawSaveList()
    {        
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(125)))
        {
            EditorGUILayout.LabelField("Save Files", EditorStyles.boldLabel);
            using (new EditorGUILayout.ScrollViewScope(scrollPos, "box"))
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    foreach (string file in saveNames)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField(file, GUILayout.Width(60));
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Load", GUILayout.Width(40)))
                            {
                                SaveManager.Load(file);
                            }
                            if (GUILayout.Button("X", GUILayout.Width(20)))
                            {
                                SaveManager.Delete(file);
                            }
                        }
                    }
                }
            }
            if (GUILayout.Button("Reload"))
            {
                saveNames = SaveManager.GetSaveFiles();
            }
        }
    }

    private void DrawNoSave()
    {
        EditorGUILayout.LabelField("No save loaded", EditorStyles.boldLabel);
    }

    private void DrawCurrentSave()
    {
        using (new EditorGUILayout.VerticalScope())
        {
            EditorGUILayout.LabelField("Current Save", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope("box"))
            {                
                SaveManager.CurrentSaveGame.fileName = EditorGUILayout.TextField("File Name", SaveManager.CurrentSaveGame.fileName);
                SaveManager.CurrentSaveGame.playerName = EditorGUILayout.TextField("Player Name", SaveManager.CurrentSaveGame.playerName);
                SaveManager.CurrentSaveGame.playerLevel = EditorGUILayout.IntField("Player Level", SaveManager.CurrentSaveGame.playerLevel);
                SaveManager.CurrentSaveGame.playerExp = EditorGUILayout.IntField("Player Exp", SaveManager.CurrentSaveGame.playerExp);
                SaveManager.CurrentSaveGame.playerLocation = EditorGUILayout.TextField("Player Location", SaveManager.CurrentSaveGame.playerLocation);
                SaveManager.CurrentSaveGame.playerPosition = EditorGUILayout.Vector3Field("Player Position", SaveManager.CurrentSaveGame.playerPosition);
                SaveManager.CurrentSaveGame.playerRotation = EditorGUILayout.FloatField("Player Rotation", SaveManager.CurrentSaveGame.playerRotation);
                SaveManager.CurrentSaveGame.currentQuest = EditorGUILayout.TextField("Current Quest", SaveManager.CurrentSaveGame.currentQuest);
                EditorGUILayout.LabelField("Reputations");
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    foreach (PlayerData.Reputation rep in SaveManager.CurrentSaveGame.reputations)
                    {
                        rep.value = EditorGUILayout.IntField(rep.character, rep.value);
                    }
                }

                GUILayout.FlexibleSpace();
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save"))
                {
                    SaveManager.Save();
                }
            }
        }
    }
}
