using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
    private Data data = new Data();

    [SerializeField] GameEvents events = null;
    [SerializeField] Animator timerAnimator = null;
    [SerializeField] TextMeshProUGUI timerText = null;
    private int timerStateParaHash = 0;

    private List<QuizData> pickedAnswers = new List<QuizData>();
    private List<int> FinishedQuestions = new List<int>();
    private int currentQuiz = 0;

    private IEnumerator IE_WaitTillNextQuiz = null;
    private IEnumerator IE_StartTimer = null;
    [SerializeField] Color timerHalfWayOutColor = Color.yellow;
    [SerializeField] Color timerAlmostOutColor = Color.red;
    private Color timerDefaultColor = Color.white;


    private bool IsFinished
    {
        get 
        { 
            return (FinishedQuestions.Count < data.Quizzes.Length) ? false : true; 
        }
    }

    private void OnEnable()
    {
        events.UpdateQuizData += UpdateQuiz;
    }

    private void OnDisable()
    {
        events.UpdateQuizData -= UpdateQuiz;
    }

    void Awake()
    {
        events.CurrentFinalScore = 0;
    }

    void Start()
    {
        events.StartUpHighScore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);

        timerDefaultColor = timerText.color;
        LoadData();
        timerStateParaHash = Animator.StringToHash("TimerState");
        /*
        foreach (var quiz in data.Quizzes)
        {
            Debug.Log(quiz.Info);
        }
        */

        Display();
    }

    public void UpdateQuiz(QuizData quizData)
    {
        if (data.Quizzes[currentQuiz].Type == AnswerType.Single)
        {
            foreach (var answer in pickedAnswers)
            {
                if (answer != quizData)
                {
                    answer.Reset();
                }
            }
            pickedAnswers.Clear();
            pickedAnswers.Add(quizData);
        }
        else
        {
            bool alreadyPicked = pickedAnswers.Exists(x => x == quizData);
            if (alreadyPicked)
            {
                pickedAnswers.Remove(quizData);
            }
            else
            {
               pickedAnswers.Add(quizData);
            }
        }
    }

    public void EraseAnswers()
    {
        pickedAnswers.Clear();
    }

    private void UpdateTimer(bool state)
    {
        switch (state)
        {
            case true:
                IE_StartTimer = StartTimer();
                StartCoroutine(IE_StartTimer);

                timerAnimator.SetInteger(timerStateParaHash, 2);
                break;
            case false:
                if (IE_StartTimer != null)
                {
                    StopCoroutine(IE_StartTimer);
                }

                timerAnimator.SetInteger(timerStateParaHash, 1);
                break;
        }
    }

    IEnumerator StartTimer()
    {
        var totalTime = data.Quizzes[currentQuiz].Timer;
        var timeLeft = totalTime;

        timerText.color = timerDefaultColor;
        while (timeLeft > 0)
        {
            timeLeft--;

            FindObjectOfType<AudioManager>().Play("CountdownSFX");

            if (timeLeft < totalTime / 2 && timeLeft > totalTime / 4)
            {
                timerText.color = timerHalfWayOutColor;
            }
            if (timeLeft < totalTime / 4)
            {
                timerText.color = timerAlmostOutColor;
            }

            timerText.text = timeLeft.ToString();
            yield return new WaitForSeconds(1.0f);
        }
        Accept();
    }


    void Display()
    {
        EraseAnswers();
        var quiz = getRandomQuiz();

        if (events.UpdateQuizUI !=  null)
        {
            events.UpdateQuizUI(quiz);
        } else
        {
            Debug.LogWarning("Error");
        }

        if(quiz.UseTimer)
        {
            UpdateTimer(quiz.UseTimer);
        }
    }

    public void Accept()
    {
        UpdateTimer(false);
        bool isCorrect = CheckQuiz();
        FinishedQuestions.Add(currentQuiz);

        UpdateScore((isCorrect) ? data.Quizzes[currentQuiz].AddScore : -data.Quizzes[currentQuiz].AddScore);
        if (IsFinished)
        {
            events.level++;
            if (events.level >= GameEvents.maxLevel)
            {
                events.level = 0;
            }
            SetHighScore();
        }

        var type = (IsFinished) ? UIManager.ResolutionScreenType.Finish : (isCorrect) ? UIManager.ResolutionScreenType.Correct : UIManager.ResolutionScreenType.Incorrect;
        if(events.DisplayResolutionScreen != null)
        {
            events.DisplayResolutionScreen(type, data.Quizzes[currentQuiz].AddScore);
        }

        FindObjectOfType<AudioManager>().Play((isCorrect) ? "correct": "wrong");

        if(type != UIManager.ResolutionScreenType.Finish)
        {
            if (IE_WaitTillNextQuiz != null)
            {
                StopCoroutine(IE_WaitTillNextQuiz);
            }
            IE_WaitTillNextQuiz = WaitTillNextQuiz();
            StartCoroutine(IE_WaitTillNextQuiz);
        }
    }

    IEnumerator WaitTillNextQuiz()
    {
        yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
        Display();
    }

    private bool CheckQuiz()
    {
        if(!CompareAnswers())
        {
            return false;
        }
        return true;
    }

    private bool CompareAnswers()
    {
        if(pickedAnswers.Count > 0)
        {
            List<int> correct = data.Quizzes[currentQuiz].GetCorrectAnswers();
            List<int> picked = pickedAnswers.Select(x=>x.AnswerIndex).ToList();
            var first = correct.Except(picked).ToList();
            var second = picked.Except(correct).ToList();

            return !first.Any() && !second.Any();
        }
        return false;
    }

    Quiz getRandomQuiz()
    {
        var randomIndex = getrandomQuestionIndex();
        currentQuiz = randomIndex;

        return data.Quizzes[randomIndex];
    }

    int getrandomQuestionIndex() {
        int random = 0;
        if (FinishedQuestions.Count < data.Quizzes.Length)
        {
            do 
            { 
                random = UnityEngine.Random.Range(0, data.Quizzes.Length);
            } while(FinishedQuestions.Contains(random) || random == currentQuiz);
        }
        return random;
    }

    void LoadData()
    {
        Debug.Log("Current level indicator: " + GameUtility.FileName + PlayerPrefs.GetInt("CurrentQuizLevel").ToString("D2"));
        data = Data.Fetch(Path.Combine(GameUtility.fileDir, GameUtility.FileName + PlayerPrefs.GetInt("CurrentQuizLevel").ToString("D2") + ".xml"));
    }

    private void SetHighScore()
    {
        var highScore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);
        if (highScore < events.CurrentFinalScore)
        {
            PlayerPrefs.SetInt(GameUtility.SavePrefKey, events.CurrentFinalScore);
        }
    }

    private void UpdateScore(int a)
    {
        events.CurrentFinalScore += a;
        if(events.CurrentFinalScore < 0)
        {
            events.CurrentFinalScore = 0;
        }

        if(events.ScoreUpdated != null)
        {
            events.ScoreUpdated();
        }    
    }
}
