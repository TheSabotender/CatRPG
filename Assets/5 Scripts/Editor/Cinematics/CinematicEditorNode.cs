using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CinematicEditorNode
{
    public static void DrawNode(CinematicBaseNode node, Vector2 offset, GUIStyle style)
    {
        GUILayout.BeginArea(GetSize(node, offset), style);
        {
            GUILayout.Label("======================", EditorStyles.label);
            node.name = EditorGUILayout.TextField(node.name, EditorStyles.boldLabel);
            GUILayout.Label(node.guid, EditorStyles.miniLabel);
            GUILayout.Space(5);
            DrawContent(node);
        }
        GUILayout.EndArea();
        //DrawConnectionPoints(node);    
    }

    static Rect GetSize(CinematicBaseNode node, Vector2 offset)
    {
        if(node is CinematicDialogueNode)
            return new Rect(node.position + offset, new Vector2(200, 220));
        if (node is CinematicWaitNode)
            return new Rect(node.position + offset, new Vector2(200, 100));
        if (node is CinematicLoadSceneNode)
            return new Rect(node.position + offset, new Vector2(200, 120));
        if (node is CinematicBranchNode)
            return new Rect(node.position + offset, new Vector2(200, 220));

        //if (node is CinematicPlaySoundNode)
        if (node is CinematicPlayAnimationNode)
            return new Rect(node.position, new Vector2(200, 120));

        return new Rect(node.position, new Vector2(200, 120));
    }

    static void DrawContent(CinematicBaseNode node)
    {
        if (node is CinematicDialogueNode)
        {
            var dialogueNode = (CinematicDialogueNode)node;

            List<CharacterData> allCharacters = new List<CharacterData>(Resources.LoadAll<CharacterData>("Characters"));
            if (allCharacters.Count > 0)
            {
                CharacterData selected = allCharacters.Where(c => c.guid == dialogueNode.speakerGuid).First();
                int speakerIndex = EditorGUILayout.Popup("Speaker", allCharacters.IndexOf(selected), allCharacters.Select(c => c.displayName).ToArray());
                dialogueNode.speakerGuid = allCharacters[speakerIndex].guid;
            }
            else
            {
                dialogueNode.speakerGuid = EditorGUILayout.TextField("Speaker", dialogueNode.speakerGuid);
            }

            EditorGUILayout.LabelField("Text");
            dialogueNode.text = EditorGUILayout.TextArea(dialogueNode.text, GUILayout.Height(100));
        }

        else if (node is CinematicWaitNode)
        {
            var waitNode = (CinematicWaitNode)node;
            waitNode.waitTime = EditorGUILayout.FloatField("Wait Time", waitNode.waitTime);
        }

        else if (node is CinematicLoadSceneNode)
        {
            var loadSceneNode = (CinematicLoadSceneNode)node;
            //loadSceneNode.scene = UnityEditor.EditorGUILayout.ObjectField("Scene", loadSceneNode.scene, typeof(SceneField), false) as SceneField;
        }

        else if (node is CinematicBranchNode)
        {
            var branchNode = (CinematicBranchNode)node;
            //branchNode.branches = EditorGUILayout.IntField("Branches", branchNode.branches);
        }
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
        else if (node is CinematicPlayAnimationNode)
        {
            var playAnimationNode = (CinematicPlayAnimationNode)node;
            playAnimationNode.animator = EditorGUILayout.ObjectField("Animator", playAnimationNode.animator, typeof(Animator), true) as Animator;
            playAnimationNode.animationClip = EditorGUILayout.ObjectField("Animation Clip", playAnimationNode.animationClip, typeof(AnimationClip), false) as AnimationClip;
        }
    }

    public static void Drag(CinematicBaseNode node, Vector2 delta)
    {
        node.position += delta;
    }
    public static bool ProcessEvents(Cinematic cinematic, CinematicBaseNode node, Vector2 offset, Event e)
    {
        if (!GetSize(node, offset).Contains(e.mousePosition))
            return false;

        switch (e.type)
        {
            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    node.position += (e.delta);
                    e.Use();
                }
                break;
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    ProcessContextMenu(cinematic, node);
                }
                break;
        }

        return true;
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
