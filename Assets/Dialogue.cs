using System.Collections;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public CharacterAnimator character;
    public TMPro.TextMeshProUGUI messageHeader;
    public TMPro.TextMeshProUGUI messageBody;
    public float typingSpeed = 1f;

    private Coroutine currentDialogue;
    

    // Start is called before the first frame update
    void Start()
    {
        ShowDialogue("First Speaker of the realm of Testers", "This is a test dialogue, to see how well basic lip sync works.", character);
    }

    // Update is called once per frame
    public void ShowDialogue(string header, string body, CharacterAnimator character)
    {
        if(currentDialogue != null)
            StopCoroutine(currentDialogue);

        currentDialogue = StartCoroutine(RunDialogue(header, body, character));
    }

    private IEnumerator RunDialogue(string header, string body, CharacterAnimator character)
    {
        messageHeader.text = header;
        messageBody.text = string.Empty;
        
        int i = 0;
        while (i < body.Length)
        {
            char letter = body[i++];
            messageBody.text += letter;
            character.LipSync(letter.ToString());

            yield return new WaitForSeconds(0.1f / typingSpeed);
        }
        messageBody.text = body;
    }

    
}
