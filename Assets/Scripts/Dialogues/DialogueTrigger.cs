using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Message[] messages = null;
    public Actor[] actors = null;


    public void StartDialogue()
    {
        FindObjectOfType<DialogueManager>().openDialogue(messages, actors);
    }
}

[Serializable]
public class Message
{
    public int actorId;
    public string message;
}

[Serializable]
public class Actor
{
    public string actorName;
    public Sprite sprite;
}