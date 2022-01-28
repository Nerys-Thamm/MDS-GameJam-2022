using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueScript dialogue;
    public bool Interactible = true;
    public bool PlayerInRange;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!PlayerInRange) {
            return;
        }

        if(Keyboard.current.eKey.wasPressedThisFrame) {
            FindObjectOfType<DialogueBox>().StartDialogue(dialogue);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerInRange = true;
            //collision.gameObject.GetComponent<Player>().interactObj.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerInRange = false;
            //collision.gameObject.GetComponent<Player>().interactObj.SetActive(false);
        }
    }


}
