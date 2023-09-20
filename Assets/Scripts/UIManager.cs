using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

[Serializable()]
public struct UIManagerParameters
{
    [Header("Answers Options")]
    [SerializeField] private float margins;
    public float Margins { get { return margins; } }

    [Header("REsolution Screen Options")]
    [SerializeField] private Color correctBGColor;
    public Color CorrectBGColor { get  { return correctBGColor; } }
    [SerializeField] Color incorrectBGColor;
    public Color IncorrectBGColor { get { return incorrectBGColor; } }
    [SerializeField] Color finalBGColor;
    public Color FinalBGColor { get { return finalBGColor; } }

}

[Serializable()]
public struct UIElements
{
    [SerializeField] private RectTransform answerContentArea;
    public RectTransform AnswerContentArea { get { return answerContentArea; } }

    [SerializeField] private TextMeshProUGUI questionInfoTextObject;
    public TextMeshProUGUI QuestionInfoTextObject { get { return questionInfoTextObject; } }

    [SerializeField] private TextMeshProUGUI scoreText;
    public TextMeshProUGUI ScoreText { get { return scoreText; } }

    [Space]
    [SerializeField] public Animator ScreenAnimator;
    [SerializeField] private Image resolutionBG;
    public Image ResolutionBG { get { return resolutionBG; } }
    [SerializeField] private TextMeshProUGUI resolutionStateText;
    public TextMeshProUGUI ResolutionStateText { get { return resolutionStateText; } }
    [SerializeField] private TextMeshProUGUI resolutionScoreText;
    public TextMeshProUGUI ResolutionScoreText { get { return resolutionScoreText; } }

    [Space]
    [SerializeField] private TextMeshProUGUI highScoreText;
    public TextMeshProUGUI HighScoreText { get { return highScoreText; } }
    [SerializeField] private CanvasGroup mainCanvasGroup;
    public CanvasGroup MainCanvasGroup { get { return mainCanvasGroup; } }
    [SerializeField] private RectTransform finishUIElements;
    public RectTransform FinishUIElements { get { return finishUIElements; } }
}

public class UIManager : MonoBehaviour
{
    public enum ResolutionScreenType { Correct, Incorrect, Finish }

    [Header("References")]
    [SerializeField] private GameEvents events;

    [Header("UI Elements (Prefab)")]
    [SerializeField] private QuizData quizDataPrefab;

    [SerializeField] private UIElements uiElements;

    [Space]
    [SerializeField] private UIManagerParameters parameters;
    List<QuizData> currentQuiz = new List<QuizData>();
    private int resStateParameterHash = 0;

    private IEnumerator IE_DisplayTimedResolution = null;

    private void Start()
    {
        UpdateScoreUI();
        resStateParameterHash = Animator.StringToHash("ScreenState");
    }

    private void OnEnable()
    {
        events.UpdateQuizUI += UpdateQuestionUI;
        events.DisplayResolutionScreen += DisplayResolution;
        events.ScoreUpdated += UpdateScoreUI;
    }

    private void OnDisable()
    {
        events.UpdateQuizUI -= UpdateQuestionUI;
        events.DisplayResolutionScreen -= DisplayResolution;
        events.ScoreUpdated -= UpdateScoreUI;
    }

    private void UpdateQuestionUI(Quiz quiz)
    {
        uiElements.QuestionInfoTextObject.text = quiz.Info;
        CreateAnswers(quiz);
    }

    void DisplayResolution(ResolutionScreenType type, int score)
    {
        UpdateResUi(type, score);
        uiElements.ScreenAnimator.SetInteger(resStateParameterHash, 2);
        uiElements.MainCanvasGroup.blocksRaycasts = false;


        if(type != ResolutionScreenType.Finish)
        {
            if(IE_DisplayTimedResolution != null)
            {
                StopCoroutine(IE_DisplayTimedResolution);
            }
            IE_DisplayTimedResolution = DisplayTimedResolution();
            StartCoroutine(IE_DisplayTimedResolution);
        }
    }

    IEnumerator DisplayTimedResolution()
    {
        yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
        uiElements.ScreenAnimator.SetInteger(resStateParameterHash, 1);
        uiElements.MainCanvasGroup.blocksRaycasts = true;
    }

    void UpdateResUi(ResolutionScreenType type, int score)
    {
        var highscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);
        switch (type)
        {
            case ResolutionScreenType.Correct:
                uiElements.ResolutionBG.color = parameters.CorrectBGColor;
                uiElements.ResolutionStateText.text = "CORRECT!";
                uiElements.ResolutionScoreText.text = "+" + score;
                break;
            case ResolutionScreenType.Incorrect:
                uiElements.ResolutionBG.color = parameters.IncorrectBGColor;
                uiElements.ResolutionStateText.text = "WRONG!";
                uiElements.ResolutionScoreText.text = "-" + score;
                break;
            case ResolutionScreenType.Finish:
                uiElements.ResolutionBG.color = parameters.FinalBGColor;
                uiElements.ResolutionStateText.text = "FINAL SCORE";

                StartCoroutine(CalculateScore());
                uiElements.FinishUIElements.gameObject.SetActive(true);
                uiElements.HighScoreText.gameObject.SetActive(true);
                uiElements.HighScoreText.text = ((highscore > events.StartUpHighScore) ? "<color=yellow>new </color>" : string.Empty) + "Highscore: " + highscore;
                break;
        }
    }

    IEnumerator CalculateScore()
    {
        if(events.CurrentFinalScore == 0)
        {
            uiElements.ResolutionScoreText.text = 0.ToString();
            yield break;
        }

        var scoreValue = 0;
        var ScoreMoreThanZero = events.CurrentFinalScore > 0;
        while (ScoreMoreThanZero ? scoreValue < events.CurrentFinalScore: scoreValue > events.CurrentFinalScore)
        {
            scoreValue += ScoreMoreThanZero ? 1 : -1;
            uiElements.ResolutionScoreText.text = scoreValue.ToString();

            yield return null;
        }
    }

    void CreateAnswers(Quiz quiz)
    {
        EraseAnswers();

        float offset = 0 - parameters.Margins;
        for (int i = 0; i < quiz.Answers.Length; i++)
        {
            QuizData newAnswer = (QuizData)Instantiate(quizDataPrefab, uiElements.AnswerContentArea);
            newAnswer.UpdateData(quiz.Answers[i].Info, i);

            newAnswer.Rect.anchoredPosition = new Vector2(0, offset);

            offset -= (newAnswer.Rect.sizeDelta.y + parameters.Margins);
            uiElements.AnswerContentArea.sizeDelta = new Vector2(uiElements.AnswerContentArea.sizeDelta.x, offset * -1);

            currentQuiz.Add(newAnswer);
        }
    }

    void EraseAnswers()
    {
        foreach (var quiz in currentQuiz)
        {
            Destroy(quiz.gameObject);
        }
        currentQuiz.Clear();
    }

    void UpdateScoreUI()
    {
        uiElements.ScoreText.text = "Score: " + events.CurrentFinalScore;
    }
}
