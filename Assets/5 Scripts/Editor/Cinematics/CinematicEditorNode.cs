using Codice.Client.BaseCommands.CheckIn;
using PlasticPipe.PlasticProtocol.Messages;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CinematicEditorNode
{
    private const float LABELWIDTH = 50;

    public static void DrawNode(CinematicBaseNode node, Vector2 offset, GUIStyle style)
    {
        GUILayout.BeginArea(GetSize(node, offset), style);
        {
            GUILayout.Label("======================", EditorStyles.label);
            node.name = EditorGUILayout.TextField(node.name, EditorStyles.boldLabel);
            GUILayout.Label(node.guid, EditorStyles.miniLabel);
            GUILayout.Space(5);
            node.EditorDraw(LABELWIDTH);
        }
        GUILayout.EndArea();
    }

    static Rect GetSize(CinematicBaseNode node, Vector2 offset)
    {
        return new Rect(node.position + offset, node.EditorSize);
    }

    public static Rect Connection(CinematicBaseNode node, Vector2 offset, int index, bool isInput, bool extraSpace)
    {
        Rect body = GetSize(node, offset);
        Rect con = new Rect(body.position, new Vector2(10, 10));

        con.y += 15;

        if (isInput)
        {
            con.x -= 3;
        }
        else
        {
            con.x += body.width - 7;

            if(node is CinematicBranchNode)
                con.y += 80 + (extraSpace ? 18 : 0) + index * 45;
        }
        
        return con;
    }

    static int ConnectionCount(CinematicBaseNode node)
    {
        if (node is CinematicBranchNode)
            return ((CinematicBranchNode)node).branches.Count;

        return 1;
    }

    static void DrawContent(CinematicBaseNode node)
    {        
    /*
        else if (node is CinematicPlaySoundNode)
        {
            var playSoundNode = (CinematicPlaySoundNode)node;
            playSoundNode.audioClip = EditorGUILayout.ObjectField("Audio Clip", playSoundNode.audioClip, typeof(AudioClip), false) as AudioClip;
        }
    
        else if (node is CinematicPlayMusicNode)
        {
            var playMusicNode = (CinematicPlayMusicNode)node;
            playMusicNode.audioClip = EditorGUILayout.ObjectField("Audio Clip", playMusicNode.audioClip, typeof(AudioClip), false) as AudioClip;
        }
    
        else if (node is CinematicPlayEffectNode)
        {
            var playEffectNode = (CinematicPlayEffectNode)node;
            playEffectNode.effect = EditorGUILayout.ObjectField("Effect", playEffectNode.effect, typeof(GameObject), false) as GameObject;

            var playEffectAtPositionNode = (CinematicPlayEffectAtPositionNode)node;
            playEffectAtPositionNode.effect = EditorGUILayout.ObjectField("Effect", playEffectAtPositionNode.effect, typeof(GameObject), false) as GameObject;
            playEffectAtPositionNode.position = EditorGUILayout.Vector3Field("Position", playEffectAtPositionNode.position);
        }
    */        
    }

    public static void DrawConnectionPoints(CinematicBaseNode node, Vector2 offset, GUIStyle inStyle, GUIStyle outStyle)
    {
        //Draw input first
        Rect input = Connection(node, offset, 0, true, false);
        GUI.Box(input, "", inStyle);

        //Draw outputs
        for (int i = 0; i <= ConnectionCount(node); i++)
        {
            Rect output = Connection(node, offset, i, false, i == ConnectionCount(node));
            GUI.Box(output, "", outStyle);
        }
    }

    public static void Drag(CinematicBaseNode node, Vector2 delta)
    {
        node.position += delta;
    }

    public enum NodeInteractionType
    {
        None,
        Drag,
        Select,
        Context,
        Disconnect,
        ConnectStart,
        ConnectEnd,
        Ignore
    }
    public static (NodeInteractionType, int) ProcessEvents(Cinematic cinematic, CinematicBaseNode node, Vector2 offset, Event e, bool isConnecting)
    {
        //check connection points
        if(Connection(node, offset,0, true, false).Contains(e.mousePosition))
        {
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                return (NodeInteractionType.ConnectStart, -1);
            }
            if (e.type == EventType.MouseUp && e.button == 0)
            {
                return (NodeInteractionType.ConnectEnd, -1);
            }
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                cinematic.nodes.ForEach(n =>
                {
                    if (n.nextNode == node.guid)
                        n.nextNode = string.Empty;
                });
                return (NodeInteractionType.Disconnect, -1);
            }
        }
        for(int i = 0; i <= ConnectionCount(node); i++)
        {
            if (Connection(node, offset, i, false, i == ConnectionCount(node)).Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    if(!string.IsNullOrEmpty(node.nextNode))
                    {
                        node.nextNode = string.Empty;
                    }
                    return (NodeInteractionType.ConnectStart, i);
                }
                if (e.type == EventType.MouseUp && e.button == 0)
                {
                    return (NodeInteractionType.ConnectEnd, -1);
                }
                if (e.type == EventType.MouseDown && e.button == 1)
                {
                    node.nextNode = string.Empty;
                    return (NodeInteractionType.Disconnect, i);
                }
            }
        }

        //check node body
        if (!GetSize(node, offset).Contains(e.mousePosition))
            return (NodeInteractionType.None, 0);

        switch (e.type)
        {
            case EventType.MouseDrag:
                if (isConnecting)
                    return (NodeInteractionType.Ignore, 0);
                else if (e.button == 0)
                {
                    node.position += (e.delta);
                    e.Use();
                }
                return (NodeInteractionType.Drag, 0);
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    return (NodeInteractionType.Select, 0);
                }
                if (e.button == 1)
                {
                    ProcessContextMenu(cinematic, node);
                    return (NodeInteractionType.Context, 0);
                }
                break;
            case EventType.MouseUp:
                if (e.button == 0)
                {
                    return (NodeInteractionType.ConnectEnd, -1);
                }
                break;

        }

        return (NodeInteractionType.None, 0);
    }

    private static void ProcessContextMenu(Cinematic cinematic, CinematicBaseNode node)
    {
        GenericMenu genericMenu = new GenericMenu();
        if(cinematic.startNodeGuid != node.guid)
            genericMenu.AddItem(new GUIContent("Set as Start"), false, () => { SetAsStart(cinematic, node); });
        genericMenu.AddItem(new GUIContent("Remove node"), false, () => { OnClickRemoveNode(cinematic, node); });
        genericMenu.ShowAsContext();
    }

    private static void SetAsStart(Cinematic cinematic, CinematicBaseNode node)
    {
        cinematic.startNodeGuid = node.guid;
    }

    private static void OnClickRemoveNode(Cinematic cinematic, CinematicBaseNode node)
    {
        cinematic.nodes.Remove(node);

        cinematic.nodes.ForEach(node =>
        {
            if (node.nextNode == node.guid)
            {
                node.nextNode = string.Empty;
            }
        });
    }
}
