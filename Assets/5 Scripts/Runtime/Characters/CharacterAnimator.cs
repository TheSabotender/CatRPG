using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public CharacterData characterData;

    [Header("Blinking")]
    public string blinkBlendshape = "e_eyeBlink";
    public AnimationCurve blinkCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public Vector2 blinkSpacing = new Vector2(1, 5);

    [Header("Lip Sync")]    
    public float blendshapeSpeed = 1f;
    public LipsyncProfile profile;

    [Header("Jaw")]
    public string jawBlendshape = "e_jawOpen";
    [Range(-50,100)]public float jawClosed;
    [Range(-50,100)]public float jawOpen;

    private Dictionary<string, float> currentLipsync;
    private SkinnedMeshRenderer[] renderers;
    private Coroutine blinkRoutine;
    private Coroutine blendshapeRoutine;

    private void OnEnable()
    {
        if (currentLipsync == null)
            currentLipsync = new Dictionary<string, float>();
        if (blinkRoutine == null)
            blinkRoutine = StartCoroutine(Blink());

        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void OnDisable()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
        if (blendshapeRoutine != null)
            StopCoroutine(blendshapeRoutine);
    }

    private IEnumerator Blink()
    {
        var lastKey = blinkCurve.keys[blinkCurve.length - 1];
        float blinkTime = 0;

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(blinkSpacing.x, blinkSpacing.y));

            while (blinkTime < lastKey.time)
            {
                blinkTime += Time.deltaTime;
                foreach (var renderer in renderers)
                {
                    int blinkIndex = renderer.sharedMesh.GetBlendShapeIndex(blinkBlendshape);
                    if(blinkIndex >= 0)
                        renderer.SetBlendShapeWeight(blinkIndex, blinkCurve.Evaluate(blinkTime) * 100);
                }
                yield return null;
            }

            foreach (var renderer in renderers)        
            {
                int blinkIndex = renderer.sharedMesh.GetBlendShapeIndex(blinkBlendshape);
                if(blinkIndex >= 0)
                    renderer.SetBlendShapeWeight(blinkIndex, 0);
            }
            blinkTime = 0;
        }                
    }

    // TODO - Emotions

    // TODO - Play animations, etc.

    public void LipSync(string phoneme)
    {
        LipsyncProfile.Phoneme p = GetPhoneme(phoneme);

        // if phoneme is not found, use the phoneme as the blendshape
        if (p != null)
        {
            if (blendshapeRoutine != null)
                StopCoroutine(blendshapeRoutine);

            StartCoroutine(BlendshapeRoutine(p));
        }

        // if phoneme is not found, and it is the end of a sentence, reset all blendshapes
        else if (phoneme == null || phoneme == "" || phoneme == " " || phoneme == "!" || phoneme == "?")
        {
            if (blendshapeRoutine != null)
                StopCoroutine(blendshapeRoutine);

            StartCoroutine(BlendshapeRoutine(null));
        }
    }

    private LipsyncProfile.Phoneme GetPhoneme(string phoneme)
    {
        foreach (var p in profile.phonemes)
        {
            foreach (var trigger in p.triggers)
            {
                if (trigger.ToLower() == phoneme.ToLower())
                    return p;
            }
        }
        return null;
    }

    private IEnumerator BlendshapeRoutine(LipsyncProfile.Phoneme phoneme)
    {
        
        float t = 0;
        if(phoneme == null)
        {
            t = 1;
        }

        // apply viseme blendshapes
        else
        {
            while (t < 1)
            {
                t += Time.deltaTime * blendshapeSpeed;                

                SetBlendshape(phoneme.blendshape, phoneme.weight, t);
                SetBlendshape(jawBlendshape, jawOpen, t);

                yield return null;
            }
        }
        

        // reset all viseme blendshapes
        while (t >= 0)
        {
            t -= Time.deltaTime * blendshapeSpeed;
            
            foreach (var renderer in renderers)
            {
                for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
                {
                    string shape = renderer.sharedMesh.GetBlendShapeName(i);
                    if (shape != null && shape.StartsWith("v_"))
                    {
                        float current = renderer.GetBlendShapeWeight(i);
                        renderer.SetBlendShapeWeight(i, Mathf.Lerp(current, 0, t));
                    }
                    else if(shape != null && shape == jawBlendshape)
                    {
                        float current = renderer.GetBlendShapeWeight(i);
                        renderer.SetBlendShapeWeight(i, Mathf.Lerp(current, jawClosed, t));
                    }
                }
            }
            yield return null;
        }

        // Ensure all visemes are reset
        foreach (var renderer in renderers)
        {
            for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
            {
                string shape = renderer.sharedMesh.GetBlendShapeName(i);
                if (shape != null && shape.StartsWith("v_"))
                {
                    renderer.SetBlendShapeWeight(i, 0);
                }
                else if (shape != null && shape == jawBlendshape)
                {
                    float current = renderer.GetBlendShapeWeight(i);
                    renderer.SetBlendShapeWeight(i, jawClosed);
                }
            }
        }
    }

    void SetBlendshape(string name, float weight, float lerp = 1)
    {
        foreach (var renderer in renderers)
        {
            int index = renderer.sharedMesh.GetBlendShapeIndex(name);
            if (index != -1)
            {
                float current = renderer.GetBlendShapeWeight(index);
                renderer.SetBlendShapeWeight(index, Mathf.Lerp(current, weight, lerp));
            }                
        }
    }
}
