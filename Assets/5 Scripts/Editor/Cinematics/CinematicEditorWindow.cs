using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class CinematicEditorWindow : EditorWindow
{
    private Cinematic cinematic;
    private string currentNode;
    private List<CinematicEditorNode> editorNodes;

    private GUIStyle startNodeStyle;
    private GUIStyle nodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    private CinematicConnectionPoint selectedInPoint;
    private CinematicConnectionPoint selectedOutPoint;

    private Vector2 offset;
    private Vector2 drag;

    private string draggingNode;

    [UnityEditor.MenuItem("Window/Cinematic Editor")]
    public static void OpenWindow(Cinematic cinematic = null)
    {
        var window = GetWindow<CinematicEditorWindow>();
        window.titleContent = new GUIContent("Cinematic Editor");
        window.cinematic = cinematic;
        window.Show();
    }

    private void OnEnable()
    {
        //builtin skins/darkskin/images/node0   =>   blue
        //builtin skins/darkskin/images/node1   =>   blue
        //builtin skins/darkskin/images/node2   =>   teal
        //builtin skins/darkskin/images/node3   =>   green
        //builtin skins/darkskin/images/node4   =>   yellow
        //builtin skins/darkskin/images/node5   =>   orange
        //builtin skins/darkskin/images/node6   =>   red

        startNodeStyle = new GUIStyle();
        startNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        startNodeStyle.border = new RectOffset(12, 12, 12, 12);
        startNodeStyle.padding = new RectOffset(10, 10, 10, 10);

        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node0.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
        nodeStyle.padding = new RectOffset(10, 10, 10, 10);
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        if (cinematic != null)
        {
            DrawConnections(offset);
            DrawNodes(offset);
            DrawInspector();

            DrawConnectionLine(Event.current);

            if(!ProcessNodeEvents(Event.current))
            {
                ProcessEvents(Event.current);
            }            

            if (GUI.changed) Repaint();
        }
        else
        {
            cinematic = (Cinematic)EditorGUI.ObjectField(new Rect(10, 10, position.width - 20, 20), "Cinematic", cinematic, typeof(Cinematic), false);
        }
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i <= widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height + gridSpacing, 0f) + newOffset);
        }

        for (int j = 0; j <= heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width + gridSpacing, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawConnections(Vector2 offset)
    {
        if (cinematic == null || cinematic.nodes == null)
            return;

        cinematic.nodes.ForEach(node =>
        {
            if (node is CinematicDialogueNode)
            {

            }
            switch (node.GetType().Name)
            {
                case nameof(CinematicBranchNode):
                    var branchNode = (CinematicBranchNode)node;
                    branchNode.branches.ForEach(branch =>
                    {
                        if (!string.IsNullOrEmpty(branch.node))
                        {
                            var nextNode = cinematic.nodes.Where(n => n.guid == branch.node).First();
                            UnityEditor.Handles.DrawLine(node.position, nextNode.position);
                        }
                    });
                    if (!string.IsNullOrEmpty(branchNode.nextNode))
                    {
                        var nextNode = cinematic.nodes.Where(n => n.guid == branchNode.nextNode).First();
                        UnityEditor.Handles.DrawLine(node.position, nextNode.position);
                    }
                    break;
                default:
                    if (!string.IsNullOrEmpty(node.nextNode))
                    {
                        var nextNode = cinematic.nodes.Where(n => n.guid == node.nextNode).First();
                        UnityEditor.Handles.DrawLine(node.position, nextNode.position);
                    }
                    break;
            }
        });
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void DrawInspector()
    {
        // Draw empty box
        if (currentNode == null)
        {

        }

        // Draw node details
        else
        {

        }
    }

    private void DrawNodes(Vector2 offset)
    {
        if (cinematic != null && cinematic.nodes != null)
        {
            cinematic.nodes.ForEach(node =>
            {
                CinematicEditorNode.DrawNode(node, offset, node.guid == cinematic.startNodeGuid ? startNodeStyle : nodeStyle);
            });
        }
    }

    private void ClearConnectionSelection()
    {

    }

    private void ProcessContextMenu(Vector2 position)
    {
        GenericMenu genericMenu = new GenericMenu();
                
        // if is over a node
        //genericMenu.AddItem(new GUIContent("Remove node"), false, () => { OnClickRemoveNode(); });

        // not over a node
        genericMenu.AddItem(new GUIContent("Dialogue"), false, () => { OnClickAddNode(position, typeof(CinematicDialogueNode)); });
        genericMenu.AddItem(new GUIContent("Wait"), false, () => { OnClickAddNode(position, typeof(CinematicWaitNode)); });
        genericMenu.AddItem(new GUIContent("Branch"), false, () => { OnClickAddNode(position, typeof(CinematicBranchNode)); });

        genericMenu.ShowAsContext();
    }

    private void OnClickAddNode(Vector2 mousePosition, Type type)
    {
        bool isFirst = false;
        if (cinematic.nodes == null)
        {
            cinematic.nodes = new List<CinematicBaseNode>();
            isFirst = true;
        }

        CinematicBaseNode node = null;
        switch (type.Name)
        {            
            case nameof(CinematicBaseNode):
                node = new CinematicBaseNode();                
                break;
            case nameof(CinematicDialogueNode):
                node = new CinematicDialogueNode();
                break;
            case nameof(CinematicBranchNode):
                node = new CinematicBranchNode();
                break;
            case nameof(CinematicWaitNode):
                node = new CinematicWaitNode();
                break;
        }

        node.name = "new " + type.Name.Replace("Cinematic", "").Replace("Node", " Node");
        node.position = mousePosition;
        node.guid = Guid.NewGuid().ToString();
        
        if (isFirst)
            cinematic.startNodeGuid = node.guid;

        cinematic.nodes.Add(node);
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (cinematic.nodes != null)
        {
            for (int i = 0; i < cinematic.nodes.Count; i++)
            {
                //nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }


    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    ClearConnectionSelection();
                }

                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0 || e.button == 2)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }

    private bool ProcessNodeEvents(Event e)
    {
        draggingNode = string.Empty;
        foreach (var node in cinematic.nodes)
        {
            if(CinematicEditorNode.ProcessEvents(cinematic, node, offset, e)) {
                draggingNode = node.guid;
                return true;
            }
        }

        return false;
    }
}