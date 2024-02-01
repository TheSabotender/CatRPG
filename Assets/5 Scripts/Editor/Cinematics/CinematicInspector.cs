using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cinematic))]
public class CinematicInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Editor"))
        {
            CinematicEditorWindow.OpenWindow((Cinematic)target);
        }

        base.OnInspectorGUI();
    }
}