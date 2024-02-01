using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GuidScriptableObject : ScriptableObject, IEquatable<GuidScriptableObject>
{
    protected string _guid;

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
    protected static Dictionary<string, GuidScriptableObject> _lookup = new Dictionary<string, GuidScriptableObject>();

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

        if (_lookup.TryGetValue(_guid, out GuidScriptableObject reference))
        {
            if (reference != this)
            {
                if (reference == null)
                {
                    _lookup[_guid] = this;
                }
                else
                {
                    EditorSetNewGUID();
                    _lookup.Add(_guid, this);
                }
            }
        }
        else _lookup.Add(_guid, this);
    }

    public void EditorSetNewGUID()
    {
        if (_guid != null && _lookup.ContainsKey(_guid))
        {
            Debug.Log("Generating new GUID for object, NOTE: this will break references to this object! Old GUID: " + _guid, this);
            _lookup.Remove(_guid);
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
