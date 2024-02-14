using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static CinematicSetVariableNode;

public class CinematicLoadSceneNode : CinematicBaseNode
{
    public string locationGuid;

    public override IEnumerator Run()
    {
        var location = GuidDatabase.Find<LocationData>(locationGuid);
        if (location != null)
            Loader.LoadScene(location, null);

        if (false)
            yield return null;
    }

#if UNITY_EDITOR
    public override int Connections => 0;

    public override Vector2 EditorSize => new Vector2(200, 100);

    public override void EditorDraw(float LABELWIDTH)
    {
        var loadSceneNode = (CinematicLoadSceneNode)this;

        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Location", GUILayout.Width(LABELWIDTH));
            List<LocationData> allVars = GuidDatabase.FindAll<LocationData>();
            if (allVars.Count > 0)
            {
                int selected = 0;
                var selectedList = allVars.Where(c => c.guid == loadSceneNode.locationGuid);
                if (selectedList.Count() == 1)
                    selected = allVars.IndexOf(selectedList.First());

                int varIndex = EditorGUILayout.Popup(selected, allVars.Select(c => c.name).ToArray());
                loadSceneNode.locationGuid = allVars[varIndex].guid;
            }
            else
            {
                loadSceneNode.locationGuid = EditorGUILayout.TextField(loadSceneNode.locationGuid);
            }
        }
    }
#endif
}
