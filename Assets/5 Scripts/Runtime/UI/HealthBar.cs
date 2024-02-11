using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public RectTransform container;

    public UIAnimator heartAnimator;
    public AnimationClip heartAnimNormal;    
    public AnimationClip heartAnimCritical;
    public AnimationClip heartAnimBreak;

    public Slider armorSlider;

    //Debug
    public bool life = true;
    public int armor = 4;
    public int maxArmor = 4;

    private bool isPlaying;

    // Start is called before the first frame update
    void Start()
    {
        StatsChanged();
        isPlaying = true;
        Play();
    }

    void StatsChanged()
    {
        container.sizeDelta = new Vector2(100 + (maxArmor * 100), 100);
        armorSlider.maxValue = armor;
        armorSlider.value = armor;
    }

    // Update is called once per frame
    void Update()
    {
        armorSlider.value = armor;     
        
        if(!life && isPlaying)
        {
            isPlaying = false;
            heartAnimator.Play(heartAnimBreak, () =>
            {

            }, 1, true);
        }
    }

    private void Play()
    {
        if (!isPlaying)
            return;

        heartAnimator.Play(armor > 0 ? heartAnimNormal : heartAnimCritical, () =>
        {
            Play();
        });
    }
}
