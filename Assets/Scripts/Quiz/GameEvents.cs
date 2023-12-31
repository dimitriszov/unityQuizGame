using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvents", menuName = "Quiz/new GameEvents")]
public class GameEvents : ScriptableObject
{
    public delegate void UpdateQuizUICallback(Quiz quiz);
    public UpdateQuizUICallback UpdateQuizUI;

    public delegate void UpdateQuizDataCallback(QuizData quizData);
    public UpdateQuizDataCallback UpdateQuizData;

    public delegate void DisplayResolutionScreenCallback(UIManager.ResolutionScreenType type, int score);
    public DisplayResolutionScreenCallback DisplayResolutionScreen;

    public delegate void ScoreUpdatedCallback();
    public ScoreUpdatedCallback ScoreUpdated;

    public int level = 1;
    public const int maxLevel = 4;

    [SerializeField] public int CurrentFinalScore, StartUpHighScore;
}
