using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class CinematicDialogueNode : CinematicBaseNode
{
    public string speakerGuid;
    public string text;

    public override IEnumerator Run()
    {
        Debug.Log("[Dialogue] " + speakerGuid + ": " + text);
        if (false)
            yield return null;
    }

#if UNITY_EDITOR
    public override Vector2 EditorSize => new Vector2(200, 220);

    public override void EditorDraw(float LABELWIDTH)
    {
        var dialogueNode = (CinematicDialogueNode)this;

        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Speaker", GUILayout.Width(LABELWIDTH));
            List<CharacterData> allCharacters = GuidDatabase.FindAll<CharacterData>();
            if (allCharacters.Count > 0)
            {
                int selected = 0;
                var selectedList = allCharacters.Where(c => c.guid == dialogueNode.speakerGuid);
                if (selectedList.Count() == 1)
                    selected = allCharacters.IndexOf(selectedList.First());

                int speakerIndex = EditorGUILayout.Popup(selected, allCharacters.Select(c => c.displayName).ToArray());
                dialogueNode.speakerGuid = allCharacters[speakerIndex].guid;
            }
            else
            {
                dialogueNode.speakerGuid = EditorGUILayout.TextField(dialogueNode.speakerGuid);
            }
        }

        EditorGUILayout.LabelField("Text");
        dialogueNode.text = EditorGUILayout.TextArea(dialogueNode.text, GUILayout.Height(100));
    }
    #endif
}
