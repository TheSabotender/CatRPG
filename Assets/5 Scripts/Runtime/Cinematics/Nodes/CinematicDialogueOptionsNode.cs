using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class CinematicDialogueOptionsNode : CinematicBaseNode
{
    [System.Serializable]
    public class Option
    {
        public string text;
        public string node;
    }
    public Option[] options;
    public string lastSpeaker, lastText;
    private bool isOptionsActive;

    public override IEnumerator Run()
    {
        isOptionsActive = true;

        // Find the speaker character from guid
        string speaker = string.Empty;
        CharacterData character = GuidDatabase.Find<CharacterData>(lastSpeaker);
        if (character != null)
            speaker = character.displayName;

        // Turn the options into strings
        string[] optionStrings = new string[4];
        for (int i = 0; i < 4; i++)
            optionStrings[i] = this.options[i].text;

        Dialogue.ShowOptions(optionStrings, (index) =>
        {
            nextNode = options[index].node;
            isOptionsActive = false;
        }, speaker, lastText);

        while (isOptionsActive)
            yield return null;
    }

#if UNITY_EDITOR
    public override int Connections => 4;

    public override Vector2 EditorSize => new Vector2(200, 160);

    public override void EditorDraw(float LABELWIDTH)
    {
        var optionNode = (CinematicDialogueOptionsNode)this;
        if(optionNode.options == null || optionNode.options.Length != 4) {
            optionNode.options = new Option[4];
        }

        if (optionNode.options[0] == null) optionNode.options[0] = new Option();
        if (optionNode.options[1] == null) optionNode.options[1] = new Option();
        if (optionNode.options[2] == null) optionNode.options[2] = new Option();
        if (optionNode.options[3] == null) optionNode.options[3] = new Option();

        using (new GUILayout.VerticalScope())
        {
            optionNode.options[0].text = EditorGUILayout.TextField(optionNode.options[0].text);
            optionNode.options[1].text = EditorGUILayout.TextField(optionNode.options[1].text);
            optionNode.options[2].text = EditorGUILayout.TextField(optionNode.options[2].text);
            optionNode.options[3].text = EditorGUILayout.TextField(optionNode.options[3].text);
        }
    }
#endif
}
