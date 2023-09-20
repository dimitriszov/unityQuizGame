using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine;

public class TextToLearn : MonoBehaviour
{
    [SerializeField] public Image panel;
    [SerializeField] private TextMeshProUGUI titleTextGui;
    [SerializeField] private TextMeshProUGUI contentTextGui;
    [SerializeField] private Color backGroundColor = Color.white;
    [SerializeField] public string titleText = string.Empty;
    [SerializeField] public string contentText = string.Empty;
    private GameManager gameManager = null;


    private void Awake()
    {
        titleTextGui.text = titleText;
        //contentTextGui.text = contentText;
        //panel.color = backGroundColor;
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Next()
    {
        gameManager.GoToNextLesson();
    }
    public void Back()
    {
        gameManager.GoTOPrevLesson();
    }
}
