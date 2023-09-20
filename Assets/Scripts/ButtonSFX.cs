using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSFX : MonoBehaviour
{
    [SerializeField] public Button button;
    // Start is called before the first frame update
    void Start()
    {
        button.AddComponent<EventTrigger>();
        button.onClick.AddListener(() => { FindObjectOfType<AudioManager>().Play("click"); });
        if (button.GetComponent<EventTrigger>() != null)
        {
            EventTrigger trigger = button.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((myFunction) => { FindObjectOfType<AudioManager>().Play("hover"); });
            trigger.triggers.Add(entry);
        }
    }
}
