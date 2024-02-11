using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVisuals : MonoBehaviour
{
    public Dictionary<string, float> blendShapes;

    [Header("References")]
    public SkinnedMeshRenderer head;

    [Header("Assets")]
    public Texture2D[] furPatterns;
    public Color[] furColors;

    // Start is called before the first frame update
    void Start()
    {
        SetBlendshapes();   
    }

    void SetBlendshapes()
    {
        if (blendShapes == null) return;

        foreach((string key, float value) in blendShapes)
        {
            foreach (SkinnedMeshRenderer smr in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                int index = smr.sharedMesh.GetBlendShapeIndex(key);
                if(index >= 0)
                    smr.SetBlendShapeWeight(index, value);
            }
        }
    }

    public void Randomize()
    {
        // Fur pattern
        if (furPatterns.Length > 0)
        {
            var pattern = furPatterns[Random.Range(0, furPatterns.Length)];
            head.material.SetTexture("_Pattern", pattern);
        }
        head.material.SetColor("_PrimaryColor", furColors[Random.Range(0, furColors.Length)]);
        head.material.SetColor("_SecondaryColor", furColors[Random.Range(0, furColors.Length)]);
        head.material.SetColor("_TertiaryColor", furColors[Random.Range(0, furColors.Length)]);

        // Blendshapes
        blendShapes = new Dictionary<string, float>();
        foreach (SkinnedMeshRenderer smr in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            for (int i = 0; i < smr.sharedMesh.blendShapeCount; i++)
            {
                string shape = smr.sharedMesh.GetBlendShapeName(i);
                // only randomize face shapes
                if(shape.StartsWith("c_") && !blendShapes.ContainsKey(shape))
                {
                    int minimum = shape.EndsWith("_n") ? -100 : 100;
                    blendShapes.Add(shape, Random.Range(minimum, 100));
                }
            }
        }
        SetBlendshapes();
    }

#if UNITY_EDITOR
    public void SaveCustomization()
    {
        string path = UnityEditor.EditorUtility.SaveFilePanel("Save Customization", "", "customization", ".cat");
        if (path.Length != 0)
        {
            System.IO.File.WriteAllText(path, JsonUtility.ToJson(blendShapes));
        }
    }

    public void LoadCustomization()
    {
        string path = UnityEditor.EditorUtility.OpenFilePanel("Load Customization", "", "cat");
        if (path.Length != 0)
        {
            blendShapes = JsonUtility.FromJson<Dictionary<string, float>>(System.IO.File.ReadAllText(path));
            SetBlendshapes();
        }
    }
#endif
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(CharacterVisuals))]
public class CharacterVisualsEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Randomize"))
        {
            ((CharacterVisuals)target).Randomize();
        }

        if (GUILayout.Button("Save Customization"))
        {
            ((CharacterVisuals)target).SaveCustomization();
        }

        if (GUILayout.Button("Load Customization"))
        {
            ((CharacterVisuals)target).LoadCustomization();
        }
    }
}
#endif
