using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LipsyncProfile;

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
        if (blendshapeRoutine == null)
            blendshapeRoutine = StartCoroutine(BlendshapeRoutine());

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
            currentLipsync[p.blendshape] = p.weight;
        }

        // if phoneme is not found, and it is the end of a sentence, reset all blendshapes
        else
        {
            currentLipsync = new Dictionary<string, float>();
        }
    }

    public void LipSync()
    {
        currentLipsync = new Dictionary<string, float>();
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

    private IEnumerator BlendshapeRoutine()
    {
        while (true)
        {
            if (currentLipsync != null && currentLipsync.Count > 0)
            {
                foreach (var blend in currentLipsync.Keys)
                    SetBlendshape(blend, currentLipsync[blend]);

                SetBlendshape(jawBlendshape, jawOpen);
            }
            else
            {
                if (renderers != null && renderers.Length > 0)
                {
                    foreach (var renderer in renderers)
                    {
                        for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
                        {
                            string shape = renderer.sharedMesh.GetBlendShapeName(i);
                            if(shape != null)
                            {
                                float current = renderer.GetBlendShapeWeight(i);
                                if (shape.StartsWith("v_"))
                                {
                                    renderer.SetBlendShapeWeight(i, Mathf.Lerp(current, 0, Time.deltaTime * blendshapeSpeed));
                                }
                                else if (shape == jawBlendshape)
                                {
                                    renderer.SetBlendShapeWeight(i, Mathf.Lerp(current, jawClosed, Time.deltaTime * blendshapeSpeed));
                                }
                            }
                        }
                    }
                }
            }
            yield return null;
        }              
    }

    void SetBlendshape(string name, float weight)
    {
        if(renderers != null && renderers.Length > 0)
        {
            foreach (var renderer in renderers)
            {
                int index = renderer.sharedMesh.GetBlendShapeIndex(name);
                if (index != -1)
                {
                    float current = renderer.GetBlendShapeWeight(index);
                    renderer.SetBlendShapeWeight(index, Mathf.Lerp(current, weight, Time.deltaTime * blendshapeSpeed));
                }
            }
        }
    }
}
