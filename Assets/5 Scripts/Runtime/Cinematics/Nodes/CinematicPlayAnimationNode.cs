using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class CinematicPlayAnimationNode : CinematicBaseNode
{
    public Animator animator;
    public AnimationClip animationClip;

#if UNITY_EDITOR
    public override Vector2 EditorSize => new Vector2(200, 120);

    public override void EditorDraw(float LABELWIDTH)
    {
        var playAnimationNode = (CinematicPlayAnimationNode)this;
        playAnimationNode.animator = EditorGUILayout.ObjectField("Animator", playAnimationNode.animator, typeof(Animator), true) as Animator;
        playAnimationNode.animationClip = EditorGUILayout.ObjectField("Animation Clip", playAnimationNode.animationClip, typeof(AnimationClip), false) as AnimationClip;
    }
#endif
}
