using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Image actorImage;
    public TextMeshProUGUI actorName;
    public TextMeshProUGUI messageText;
    public RectTransform backRoundBox;

    Message[] currentMessages = null;
    Actor[] currentActors = null;
    int activeMessage = 0;
    public static bool isActive = false;

    private void Start()
    {
        backRoundBox.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isActive == true)
        {
            NextMessage();
        }
    }

    public void openDialogue(Message[] messages, Actor[] actors)
    {
        currentActors = actors;
        currentMessages = messages;
        activeMessage = 0;
        isActive = true;

        DisplayMessage();
        backRoundBox.LeanScale(Vector3.one, 0.5f).setEaseInOutExpo();
    }
    
    public void DisplayMessage()
    {
        Message messageToDisplay = currentMessages[activeMessage];
        messageText.text = messageToDisplay.message;

        Actor actorTodisplay = currentActors[messageToDisplay.actorId];
        actorName.text = actorTodisplay.actorName;
        actorImage.sprite = actorTodisplay.sprite;
    }

    public void NextMessage()
    {
        activeMessage++;
        if(activeMessage < currentMessages.Length)
        {
            DisplayMessage();
        }
        else
        {
            backRoundBox.LeanScale(Vector3.zero, 0.5f).setEaseInOutExpo();
            isActive = false;
        }
    }
}
