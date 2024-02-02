using System.Collections;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class CinematicMoveCameraNode : CinematicBaseNode
{
    public enum MoveType
    {
        Instant,
        Curve
    }
    public enum LookAtType
    {
        Vector,
        Transform
    }

    public MoveType moveType;
    public LookAtType lookAtType;

    public Transform lookAtTransform;
    public Vector3 lookAtVector;
    
    public AnimationCurve curve;

    public bool waidForEnd;

    public override IEnumerator Run()
    {
        /*
        CameraManager.Instance.MoveCamera(
            lookAtType == LookAtType.Transform ? lookAtTransform : lookAtVector,
            moveType == MoveType.Instant ? AnimationCurve.Linear : curve,
            moveType == MoveType.Instant ? 0 : duration
        );
        */

        if (waidForEnd && moveType == MoveType.Curve)
        {
            yield return new WaitForSeconds(curve.keys.Last().time);
        }
    }

#if UNITY_EDITOR
    public override Vector2 EditorSize => new Vector2(200, 160 + (moveType == MoveType.Curve ? 20 : 0));

    public override void EditorDraw(float LABELWIDTH)
    {
        var moveCameraNode = (CinematicMoveCameraNode)this;
        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Move Type", GUILayout.Width(LABELWIDTH));
            moveCameraNode.moveType = (CinematicMoveCameraNode.MoveType)EditorGUILayout.EnumPopup(moveCameraNode.moveType);
        }
        if (moveCameraNode.moveType == CinematicMoveCameraNode.MoveType.Curve)
            moveCameraNode.curve = EditorGUILayout.CurveField(moveCameraNode.curve);

        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Target", GUILayout.Width(LABELWIDTH));
            moveCameraNode.lookAtType = (CinematicMoveCameraNode.LookAtType)EditorGUILayout.EnumPopup(moveCameraNode.lookAtType);
        }
        if (moveCameraNode.lookAtType == CinematicMoveCameraNode.LookAtType.Transform)
            moveCameraNode.lookAtTransform = EditorGUILayout.ObjectField(moveCameraNode.lookAtTransform, typeof(Transform), true) as Transform;
        else
            moveCameraNode.lookAtVector = EditorGUILayout.Vector3Field("", moveCameraNode.lookAtVector);

        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Wait", GUILayout.Width(LABELWIDTH));
            moveCameraNode.waidForEnd = EditorGUILayout.Toggle(moveCameraNode.waidForEnd);
        }
    }
    #endif
}
