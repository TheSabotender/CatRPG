using Codice.Client.BaseCommands.CheckIn;
using Codice.CM.SEIDInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// https://oguzkonya.com/creating-node-based-editor-unity/

public class CinematicEditorWindow : EditorWindow
{
    private enum CursorMode { None, Drag, Connect }
    private Cinematic cinematic;
    private string currentNode;
    private List<CinematicEditorNode> editorNodes;

    private GUIStyle startNodeStyle;
    private GUIStyle nodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    private CursorMode cursorMode = CursorMode.None;
    private int selectedConnectionPoint;

    private Vector2 offset;
    private Vector2 drag;

    private string draggingNode;

    public static void OpenWindow(Cinematic cinematic)
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

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        if (cinematic != null)
        {
            DrawConnections(offset);
            DrawNodes(offset);            
            DrawConnectionLine(Event.current);
            DrawInspector();

            if (!ProcessNodeEvents(Event.current))
            {
                ProcessEvents(Event.current);
            }            

            if (GUI.changed)
            {
                EditorUtility.SetDirty(cinematic);
                Repaint();
            }
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

        Vector2 lineOffset = new Vector2(0, 5);
        cinematic.nodes.ForEach(node =>
        {
            switch (node.GetType().Name)
            {
                case nameof(CinematicBranchNode):
                    var branchNode = (CinematicBranchNode)node;                    
                    for(int i = 0; i < branchNode.branches.Count; i++)
                    {
                        var branch = branchNode.branches[i];
                        if (!string.IsNullOrEmpty(branch.node))
                        {
                            var nextNode = cinematic.nodes.Where(n => n.guid == branch.node).First();
                            DrawLine(CinematicEditorNode.Connection(nextNode, offset, 0, true, false).position + lineOffset, CinematicEditorNode.Connection(node, offset, i, false, false).position + lineOffset);
                        }
                    }

                    if (!string.IsNullOrEmpty(branchNode.elseBranch))
                    {
                        var nextNode = cinematic.nodes.Where(n => n.guid == branchNode.elseBranch).First();
                        DrawLine(CinematicEditorNode.Connection(nextNode, offset, 0, true, false).position + lineOffset, CinematicEditorNode.Connection(node, offset, branchNode.branches.Count, false, true).position + lineOffset);
                    }
                    break;

                case nameof(CinematicDialogueOptionsNode):
                    var dialogueOptionsNode = (CinematicDialogueOptionsNode)node;
                    if(dialogueOptionsNode.options != null)
                    {
                        for (int i = 0; i < dialogueOptionsNode.options.Length; i++)
                        {
                            var option = dialogueOptionsNode.options[i];
                            if (!string.IsNullOrEmpty(option.node))
                            {
                                var nextNode = cinematic.nodes.Where(n => n.guid == option.node).First();
                                DrawLine(CinematicEditorNode.Connection(nextNode, offset, 0, true, false).position + lineOffset, CinematicEditorNode.Connection(node, offset, i, false, false).position + lineOffset);
                            }
                        }
                    }
                    break;

                default:
                    if (!string.IsNullOrEmpty(node.nextNode))
                    {
                        var connections = cinematic.nodes.Where(n => n.guid == node.nextNode);
                        if(connections != null && connections.Count() == 1)
                        {
                            var nextNode = cinematic.nodes.Where(n => n.guid == node.nextNode).First();
                            DrawLine(CinematicEditorNode.Connection(nextNode, offset, 0, true, false).position + lineOffset, CinematicEditorNode.Connection(node, offset, 0, false, false).position + lineOffset);
                        } else
                            node.nextNode = string.Empty;
                    }
                    break;
            }
        });
    }

    private void DrawConnectionLine(Event e)
    {
        if (!string.IsNullOrEmpty(currentNode) && cursorMode == CursorMode.Connect)
        {
            if(selectedConnectionPoint == -1)
            {
                Rect rect = CinematicEditorNode.Connection(cinematic.nodes.Where(n => n.guid == currentNode).First(), offset, 0, true, false);
                DrawLine(rect.center, e.mousePosition);                
            }

            else
            {
                CinematicBaseNode node = cinematic.nodes.Where(n => n.guid == currentNode).First();
                CinematicBranchNode branchNode = node as CinematicBranchNode;
                CinematicDialogueOptionsNode optionsNode = node as CinematicDialogueOptionsNode;

                Rect rect = CinematicEditorNode.Connection(node, offset, selectedConnectionPoint, false, false);
                if(branchNode != null)
                    rect = CinematicEditorNode.Connection(node, offset, selectedConnectionPoint, false, selectedConnectionPoint == branchNode.branches.Count);
                if (optionsNode != null)
                    rect = CinematicEditorNode.Connection(node, offset, selectedConnectionPoint, false, false);
                DrawLine(e.mousePosition, rect.center);
            }

            GUI.changed = true;
        }
    }

    void DrawLine(Vector2 from, Vector2 to)
    {
        Handles.DrawBezier(
                    from,
                    to,
                    from + Vector2.left * 50f,
                    to - Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );
    }

    private void DrawInspector()
    {
        // Draw empty box
        if (string.IsNullOrEmpty(currentNode))
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
                CinematicEditorNode.DrawConnectionPoints(node, offset, inPointStyle, outPointStyle);
            });
        }
    }

    private void ClearConnectionSelection()
    {
        cursorMode = CursorMode.None;
        selectedConnectionPoint = -1;
    }

    private void ProcessContextMenu(Vector2 position)
    {
        GenericMenu genericMenu = new GenericMenu();
                
        // not over a node
        genericMenu.AddItem(new GUIContent("Dialogue"), false, () => { OnClickAddNode<CinematicDialogueNode>(position); });
        genericMenu.AddItem(new GUIContent("Options"), false, () => { OnClickAddNode<CinematicDialogueOptionsNode>(position); });
        genericMenu.AddItem(new GUIContent("Wait"), false, () => { OnClickAddNode<CinematicWaitNode>(position); });
        genericMenu.AddItem(new GUIContent("Branch"), false, () => { OnClickAddNode<CinematicBranchNode>(position); });
        genericMenu.AddItem(new GUIContent("Camera"), false, () => { OnClickAddNode<CinematicMoveCameraNode>(position); });
        genericMenu.AddItem(new GUIContent("SetVar"), false, () => { OnClickAddNode<CinematicSetVariableNode>(position); });
        genericMenu.AddItem(new GUIContent("Goto Location"), false, () => { OnClickAddNode<CinematicLoadSceneNode>(position); });

        genericMenu.ShowAsContext();
    }

    private void OnClickAddNode<T>(Vector2 mousePosition) where T : CinematicBaseNode, new()
    {
        T node = new T();

        if(node is CinematicBranchNode)
            (node as CinematicBranchNode).branches = new List<CinematicBranchNode.Branch>();

        if (node is CinematicDialogueOptionsNode)
            (node as CinematicDialogueOptionsNode).options = new CinematicDialogueOptionsNode.Option[4];

        node.name = "new " + typeof(T).Name.Replace("Cinematic", "").Replace("Node", " Node");
        node.position = mousePosition - offset;
        node.guid = Guid.NewGuid().ToString();


        if (cinematic.nodes == null)
        {
            cinematic.nodes = new List<CinematicBaseNode>();
            cinematic.startNodeGuid = node.guid;
        }        
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
                if ((e.button == 0 || e.button == 2) && cursorMode != CursorMode.Connect)
                {
                    OnDrag(e.delta);
                }
                break;

            case EventType.MouseUp:
                ClearConnectionSelection();
                break;
        }
    }

    private bool ProcessNodeEvents(Event e)
    {
        draggingNode = string.Empty;
        foreach (var node in cinematic.nodes)
        {
            (CinematicEditorNode.NodeInteractionType interaction, int index) = CinematicEditorNode.ProcessEvents(cinematic, node, offset, e, cursorMode == CursorMode.Connect);
            switch(interaction) {
                case CinematicEditorNode.NodeInteractionType.Select:                    
                case CinematicEditorNode.NodeInteractionType.Context:
                    currentNode = node.guid;
                    ClearConnectionSelection();
                    GUI.changed = true;
                    return true;
                case CinematicEditorNode.NodeInteractionType.Drag:
                    cursorMode = CursorMode.Drag;
                    draggingNode = node.guid;
                    GUI.changed = true;
                    return true;
                case CinematicEditorNode.NodeInteractionType.ConnectStart:
                    ClearConnectionSelection();
                    cursorMode = CursorMode.Connect;
                    currentNode = node.guid;
                    selectedConnectionPoint = index;
                    return true;
                case CinematicEditorNode.NodeInteractionType.ConnectEnd:
                    TryConnect(node);
                    ClearConnectionSelection();
                    GUI.changed = true;
                    break;
                case CinematicEditorNode.NodeInteractionType.Disconnect:
                    ClearConnectionSelection();
                    GUI.changed = true;
                    return true;
                case CinematicEditorNode.NodeInteractionType.Ignore:
                    GUI.changed = true;
                    return true;


                case CinematicEditorNode.NodeInteractionType.None:
                default:
                    break;
            }
        }

        return false;
    }

    bool TryConnect(CinematicBaseNode node)
    {
        if (cursorMode == CursorMode.Connect && currentNode != node.guid)
        {
            //From output
            //first node is the one we are connecting from
            //node is the one we are connecting to
            if (selectedConnectionPoint >= 0)
            {                
                CinematicBaseNode firstNode = cinematic.nodes.Where(n => n.guid == currentNode).First();
                CinematicBranchNode branchNode = firstNode as CinematicBranchNode;
                CinematicDialogueOptionsNode optionsNode = firstNode as CinematicDialogueOptionsNode;
                if (branchNode != null && selectedConnectionPoint == branchNode.branches.Count)
                {
                    branchNode.elseBranch = node.guid;
                }
                else if (branchNode != null)
                {
                    branchNode.branches[selectedConnectionPoint].node = node.guid;
                }
                if (optionsNode != null)
                {
                    optionsNode.options[selectedConnectionPoint].node = node.guid;
                }

                firstNode.nextNode = node.guid;

                if (firstNode is CinematicDialogueNode)
                {
                    CinematicDialogueNode dialogue = firstNode as CinematicDialogueNode;
                    dialogue.nextIsOptions = false;

                    if (node is CinematicDialogueOptionsNode)
                    {
                        CinematicDialogueOptionsNode options = node as CinematicDialogueOptionsNode;
                        options.lastSpeaker = dialogue.speakerGuid;
                        options.lastText = dialogue.text;
                        dialogue.nextIsOptions = true;
                    }
                } else if (node is CinematicDialogueOptionsNode)
                {
                    CinematicDialogueOptionsNode options = node as CinematicDialogueOptionsNode;
                    options.lastSpeaker = string.Empty;
                    options.lastText = string.Empty;
                }


            }

            //From input
            //secondNode is the one we are connecting to
            //node is the one we are connecting to
            else
            {
                CinematicBaseNode firstNode = cinematic.nodes.Where(n => n.guid == currentNode).First();
                CinematicBaseNode secondNode = cinematic.nodes.Where(n => n.guid == node.guid).First();
                secondNode.nextNode = currentNode;

                if (secondNode is CinematicDialogueNode)
                {
                    CinematicDialogueNode dialogue = secondNode as CinematicDialogueNode;
                    dialogue.nextIsOptions = false;

                    if (firstNode is CinematicDialogueOptionsNode)
                    {
                        CinematicDialogueOptionsNode options = firstNode as CinematicDialogueOptionsNode;
                        options.lastSpeaker = dialogue.speakerGuid;
                        options.lastText = dialogue.text;
                        dialogue.nextIsOptions = true;
                    }
                } else if (firstNode is CinematicDialogueOptionsNode)
                {
                    CinematicDialogueOptionsNode options = firstNode as CinematicDialogueOptionsNode;
                    options.lastSpeaker = string.Empty;
                    options.lastText = string.Empty;
                }
            }

            ClearConnectionSelection();
            return true;
        }

        return false;
    }
}