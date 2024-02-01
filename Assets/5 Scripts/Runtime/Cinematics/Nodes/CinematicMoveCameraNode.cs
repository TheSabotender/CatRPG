using System.Collections;
using System.Linq;
using UnityEngine;

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
}
