using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GuidScriptableObject : ScriptableObject, IEquatable<GuidScriptableObject>
{
    [SerializeField]
    [HideInInspector]
    private string _guid;

    public string guid => _guid;

    public override bool Equals(object obj_)
    {
        return Equals(obj_ as GuidScriptableObject);
    }

    public bool Equals(GuidScriptableObject other_)
    {
        return other_ is not null && _guid == other_._guid;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_guid);
    }

    #region -------------------------- EDITOR ---------------------------
#if UNITY_EDITOR

    private void Awake()
    {
        if (Application.isPlaying) return;
        EditorCheckGUID();
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        EditorCheckGUID();
    }

    public void EditorCheckGUID()
    {
        if (string.IsNullOrEmpty(_guid))
        {
            EditorSetNewGUID();
        }

        var gso = GuidDatabase.Find<GuidScriptableObject>(guid);        
        if (gso != null)
        {
            if (gso != this)
            {
                if (gso == null)
                {
                    Debug.Log("Replacing corrupt reference");
                    GuidDatabase.Add(this);
                }
                else
                {
                    EditorSetNewGUID();
                    GuidDatabase.Add(this);                    
                }
                EditorUtility.SetDirty(GuidDatabase.Instance);
            }
        }
        else
        {
            GuidDatabase.Add(this);
            EditorUtility.SetDirty(GuidDatabase.Instance);
        }
    }

    public void EditorSetNewGUID()
    {
        var gso = GuidDatabase.Find<GuidScriptableObject>(guid);
        if (guid != null && gso != null)
        {
            Debug.Log("Generating new GUID for object, NOTE: this will break references to this object! Old GUID: " + guid, this);
            GuidDatabase.Remove(this);
        }

        _guid = Guid.NewGuid().ToString();
        EditorUtility.SetDirty(this);
    }
#endif
    #endregion
}

#if UNITY_EDITOR
// we make a custom inspector for GuidScriptableObjects, so that _guid is read-only, and has a button to generate a new one
[CustomEditor(typeof(GuidScriptableObject), true)]
public class GuidScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {        
        GuidScriptableObject script = (GuidScriptableObject)target;

        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.TextField("GUID", script.guid);

            if (GUILayout.Button("Regenerate", GUILayout.Width(100)))
                if(EditorUtility.DisplayDialog("Generate new GUID", "Are you sure you want to generate a new GUID? You might have references to this in your project.", "Generate", "Abort"))
                    script.EditorSetNewGUID();
        }

        GuidDatabase.Add(script);
        EditorUtility.SetDirty(GuidDatabase.Instance);

        base.OnInspectorGUI();
    }
}


#endif
