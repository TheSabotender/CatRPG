using System;
using System.Collections;
using UnityEngine;

public class UIAnimator : MonoBehaviour
{
    public AnimationClip playOnAwake;
    public bool isPlaying { get; private set; }

    public void Play(AnimationClip clip, Action onComplete = null, float speed = 1, bool cancelExisting = false)
    {
        if (cancelExisting)
            StopAllCoroutines();

        StartCoroutine(DoPlay(clip, speed, cancelExisting, onComplete));
    }

    public void Stop()
    {
        StopAllCoroutines();
    }

    public void SetToLastFrame(AnimationClip clip)
    {
        clip.SampleAnimation(gameObject, clip.length);
    }

    #region ---PRIVATE---    

    private void Awake()
    {
        if (playOnAwake != null)
            Play(playOnAwake);
    }

    private IEnumerator DoPlay(AnimationClip clip, float speed, bool cancel, Action onComplete)
    {
        if (isPlaying && !cancel)
            yield return null;

        isPlaying = true;
        float time = 0;
        while (time < clip.length)
        {
            clip.SampleAnimation(gameObject, time);
            time += Time.deltaTime * speed;
            yield return null;
        }
        clip.SampleAnimation(gameObject, clip.length);
        isPlaying = false;
        onComplete?.Invoke();
    }

    #endregion
}
