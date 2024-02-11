using System.Collections;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public CharacterData characterData;
    public SkinnedMeshRenderer head;

    [Header("Blinking")]
    public string blinkBlendshape = "e_eyeBlink";
    public AnimationCurve blinkCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public Vector2 blinkSpacing = new Vector2(1, 5);

    [Header("Lip Sync")]    
    public float blendshapeSpeed = 1f;
    public LipsyncProfile profile;

    private Coroutine blinkRoutine;
    private Coroutine blendshapeRoutine;

    private void OnEnable()
    {
        if (blinkRoutine == null)
            blinkRoutine = StartCoroutine(Blink());
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
        int blinkIndex = head.sharedMesh.GetBlendShapeIndex("e_eyeBlink");
        var lastKey = blinkCurve.keys[blinkCurve.length - 1];
        float blinkTime = 0;
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(blinkSpacing.x, blinkSpacing.y));      
            
            while (blinkTime < lastKey.time)
            {
                blinkTime += Time.deltaTime;
                head.SetBlendShapeWeight(blinkIndex, blinkCurve.Evaluate(blinkTime) * 100);
                yield return null;
            }

            head.SetBlendShapeWeight(blinkIndex, 0);
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
                for (int i = 0; i < head.sharedMesh.blendShapeCount; i++)
                {
                    string shape = head.sharedMesh.GetBlendShapeName(i);
                    if (shape != null && shape == phoneme.blendshape)
                    {
                        float current = head.GetBlendShapeWeight(i);
                        head.SetBlendShapeWeight(i, Mathf.Lerp(current, phoneme.weight, t));
                    }
                }
                yield return null;
            }
        }
        

        // reset all viseme blendshapes
        while (t >= 0)
        {
            t -= Time.deltaTime * blendshapeSpeed;
            for (int i = 0; i < head.sharedMesh.blendShapeCount; i++)
            {
                string shape = head.sharedMesh.GetBlendShapeName(i);
                if (shape != null && shape.StartsWith("v_"))
                {
                    float current = head.GetBlendShapeWeight(i);
                    head.SetBlendShapeWeight(i, Mathf.Lerp(current, 0, t));
                }
            }
            yield return null;
        }

        // Ensure all visemes are reset
        for (int i = 0; i < head.sharedMesh.blendShapeCount; i++)
        {
            string shape = head.sharedMesh.GetBlendShapeName(i);
            if (shape != null && shape.StartsWith("v_"))
            {
                head.SetBlendShapeWeight(i, 0);
            }
        }
    }
}
