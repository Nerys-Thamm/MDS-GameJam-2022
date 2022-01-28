using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public Queue<DialogueScript.Phrase> phraseQueue;
    public InputAction input;
    DialogueScript.Phrase currentPhrase;

    public TMP_Text text;
    public Image charsprite;
    public Animator animator;
    AudioSource blipSource;

    string dialogueString;
    string nextDialogueString;
    int dialogueIndex;
    int currDialogCount;
    float currDialogSpeed;
    public GameObject dialogueBox;
    float timer = 0;
    bool isTyping = false;

    // Start is called before the first frame update
    void Start()
    {
        blipSource = GetComponentInChildren<AudioSource>();
        
        input.Enable();
        input.performed += SkipDialogue;
    }

    void SkipDialogue(InputAction.CallbackContext context)
    {
        
            if (isTyping)
            {
                text.text = dialogueString;
                isTyping = false;
                blipSource.Stop();
            }
            else
            {
                text.text = "";
                NextPhrase();
            }
    }



    // Update is called once per frame
    void Update()
    {
        animator.SetBool("IsTyping", isTyping);
        if (isTyping)
        {
            
                timer += Time.deltaTime;
                if (timer >= currDialogSpeed)
                {
                    timer = 0;
                    dialogueIndex++;
                    text.text = dialogueString.Substring(0, dialogueIndex);
                    if (dialogueIndex >= dialogueString.Length)
                    {
                        isTyping = false;
                        dialogueIndex = 0;
                        text.text = dialogueString;
                    }
                    if(blipSource == null) {
                        blipSource = GetComponentInChildren<AudioSource>();
                    }
                    blipSource.clip = currentPhrase.m_blip;
                    blipSource.Play();
                }
            
        }
        
    }

    public void SetDialogue(DialogueScript.Phrase phrase)
    {
        currentPhrase = phrase;
        dialogueString = phrase.m_message;
        dialogueIndex = 0;
        currDialogCount = phrase.m_message.Length;
        currDialogSpeed = phrase.m_speed;
        charsprite.sprite = phrase.m_characterSprite;
        isTyping = true;
        
        
    }

    public void NextPhrase()
    {
        if (phraseQueue != null && phraseQueue.Count > 0)
        {
            dialogueBox.SetActive(true);
            SetDialogue(phraseQueue.Dequeue());
            
        }
        else
        {
            dialogueBox.SetActive(false);
        }
    }

    public void StartDialogue(DialogueScript dialogue)
    {
        
        phraseQueue = new Queue<DialogueScript.Phrase>(dialogue.m_Phrases);
        NextPhrase();
    }
    
}
