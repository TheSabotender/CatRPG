using System.Collections;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    private static Dialogue instance;
    private Coroutine currentDialogue;
    private bool isTyping;
    private bool isDialogueActive;
    private bool isOptionsActive;
    private System.Action onDialogueComplete;
    private System.Action<int> onClickOptions;

    public UIAnimator messageAnimator;
    public AnimationClip showDialogue;
    public AnimationClip hideDialogue;
    public RectTransform headerBox;
    public TMPro.TextMeshProUGUI messageHeader;
    public TMPro.TextMeshProUGUI messageBody;
    public float typingSpeed = 1f;

    public UIAnimator optionsAnimator;
    public AnimationClip showOptions;
    public AnimationClip hideOptions;
    public GameObject dialogueHistory;
    public RectTransform optionsHeaderBox;
    public TMPro.TextMeshProUGUI optionsHeader;
    public TMPro.TextMeshProUGUI optionsBody;
    public TMPro.TextMeshProUGUI option1_text;
    public TMPro.TextMeshProUGUI option2_text;
    public TMPro.TextMeshProUGUI option3_text;
    public TMPro.TextMeshProUGUI option4_text;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        messageAnimator.SetToLastFrame(hideDialogue);
        optionsAnimator.SetToLastFrame(hideOptions);
    }

    // Update is called once per frame
    public static void ShowDialogue(string header, string body, CharacterAnimator character, bool waitForClick, System.Action onComplete)
    {
        if(instance.currentDialogue != null)
            instance.StopCoroutine(instance.currentDialogue);

        if(!instance.isDialogueActive)
            instance.messageAnimator.Play(instance.showDialogue, null, 1, true);

        instance.isDialogueActive = true;
        instance.onDialogueComplete = null;
        instance.onDialogueComplete += onComplete;        
        instance.currentDialogue = instance.StartCoroutine(instance.RunDialogue(header, body, waitForClick, character));
    }

    public static void HideDialogue()
    {
        instance.isDialogueActive = false;
        if (instance.currentDialogue != null)
            instance.StopCoroutine(instance.currentDialogue);

        instance.messageAnimator.Play(instance.hideDialogue, null, 1, true);
    }

    private IEnumerator RunDialogue(string header, string body, bool doWait, CharacterAnimator character)
    {
        isTyping = true;
        headerBox.gameObject.SetActive(!string.IsNullOrEmpty(header));
        headerBox.sizeDelta = new Vector2((header.Length * 20) + 30, headerBox.sizeDelta.y);
        messageHeader.text = header;        
        messageBody.text = string.Empty;        

        int i = 0;
        while (i < body.Length && isTyping)
        {
            char letter = body[i++];
            messageBody.text += letter;
            if(character != null)
                character.LipSync(letter.ToString());

            yield return new WaitForSeconds(0.1f / typingSpeed);
        }
        if (character != null)
            character.LipSync();
        messageBody.text = body;
        isTyping = false;

        if (!doWait)
        {
            onDialogueComplete?.Invoke();
            onDialogueComplete = null;
        }
    }

    public void OnClickDialogue()
    {
        if(isTyping)
        {
            isTyping = false;
        }
        else if(onDialogueComplete != null)
        {
            onDialogueComplete?.Invoke();
            onDialogueComplete = null;
        }
        
    }

    /// <summary>
    /// Must have 4 options
    /// </summary>
    /// <param name="options"></param>
    /// <param name="onSelect"></param>
    public static void ShowOptions(string[] options, System.Action<int> onSelect, string lastSpeaker = "", string lastText = "")
    {
        if (options.Length != 4)
            return;

        instance.isOptionsActive = true;

        instance.dialogueHistory.SetActive(!string.IsNullOrEmpty(lastText));
        if (!string.IsNullOrEmpty(lastText))
        {
            instance.optionsHeaderBox.gameObject.SetActive(!string.IsNullOrEmpty(lastSpeaker));
            instance.optionsHeaderBox.sizeDelta = new Vector2((lastSpeaker.Length * 20) + 30, instance.optionsHeaderBox.sizeDelta.y);
            instance.optionsHeader.text = lastSpeaker;
            instance.optionsBody.text = lastText;
        }

        instance.option1_text.text = options[0];
        instance.option2_text.text = options[1];
        instance.option3_text.text = options[2];
        instance.option4_text.text = options[3];

        instance.onClickOptions = onSelect;
        instance.optionsAnimator.Play(instance.showOptions, null, 1, true);
    }

    public void OnClickOption(int index)
    {
        optionsAnimator.Play(hideOptions, () =>
        {
            instance.isOptionsActive = false;
            onClickOptions?.Invoke(index);
        }, 1, true);        
    }
}
