using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AnswerType { Multi, Single }

[System.Serializable]
public class Answer
{
    public string Info = string.Empty;
    public bool IsCorrect = false;

    public Answer() { }
}
[System.Serializable]
public class Quiz
{

    public String Info = null;
    public Answer[] Answers = null;
    public Boolean UseTimer = false;
    public Int32 Timer = 0;
    public AnswerType Type = AnswerType.Single;
    public Int32 AddScore = 0;

    public Quiz() { }

    public List<int> GetCorrectAnswers()
    {
        List<int> correctAnswers = new List<int>();
        for(int i = 0;  i < Answers.Length; i++)
        {
            if (Answers[i].IsCorrect)
            {
                correctAnswers.Add(i);
            }
        }
        return correctAnswers;
    }
}
