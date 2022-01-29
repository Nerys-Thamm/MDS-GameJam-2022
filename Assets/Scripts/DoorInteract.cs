using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorInteract : MonoBehaviour
{
    public InputAction Interact;
    Transform playerTransform;
    public DialogueScript[] dialogueScripts;
    public DialogueScript[] dialogueScriptVisited;
    int RandomInt;
    public bool Visited = false;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        RandomInt = Random.Range(0, dialogueScripts.Length);
        Interact.Enable();
        Interact.performed += InteractCallback;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InteractCallback(InputAction.CallbackContext context)
    {
        if (Vector3.Distance(playerTransform.position, transform.position) < 2.5f)
        {
            FindObjectOfType<DialogueBox>().StartDialogue((Visited ? dialogueScriptVisited[RandomInt] : dialogueScripts[RandomInt]));
        }
    }
}
