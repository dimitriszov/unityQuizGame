using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int currentLesson = 0;

    public string nextLevel = null;
    public int levelToUnlock = 2;
    public TextToLearn[] lessons;
    private int levelReached;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name.Equals("Tutorial"))
        {
            int currentLevel = PlayerPrefs.GetInt("CurrentQuizLevel");
            levelToUnlock = currentLevel + 1;
        }
        levelReached = PlayerPrefs.GetInt("levelReached");
        if (levelToUnlock > levelReached)
            levelReached = levelToUnlock;
    }
    public void Start()
    {
        if(SceneManager.GetActiveScene().name.Equals("Main"))
        {
            FindObjectOfType<AudioManager>().Play("MainTheme");
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("LevelTheme");
        }
        currentLesson = 0;
        if(nextLevel == null)
        {
            Debug.LogWarning("Set Next level to load");
        }
    }

    private void Update()
    {
        if (!SceneManager.GetActiveScene().name.Equals("Main"))
        {
            FindObjectOfType<AudioManager>().Stop("MainTheme");
        }
    }

    public void WinLevel()
    {
        Debug.Log("Level Won!");
        Debug.Log("Level Reached: " + levelReached);
        PlayerPrefs.SetInt("levelReached", levelReached);
        FindObjectOfType<AudioManager>().Play("Win");
    }

    public void GoToNextLesson()
    {
        if(currentLesson + 1 < lessons.Length)
        {
            lessons[currentLesson].gameObject.SetActive(false);
            currentLesson++;
            lessons[currentLesson].gameObject.SetActive(true);
        }
        else if(SceneManager.GetActiveScene().name == "Tutorial")
        {
            int currentLevel = PlayerPrefs.GetInt("CurrentQuizLevel");
            if(currentLevel == 25)
            {
                FindObjectOfType<SceneFader>().FadeTo("Main");
                FindObjectOfType<AudioManager>().Stop("LevelTheme");
                return;
            }
            WinLevel();

            FindObjectOfType<AudioManager>().Stop("LevelTheme");
            FindObjectOfType<SceneFader>().FadeTo("Level" + (currentLevel+1).ToString("D2"));
        }
        else
        {
            WinLevel();
            FindObjectOfType<AudioManager>().Stop("LevelTheme");
            PlayerPrefs.SetInt("CurrentQuizLevel", levelToUnlock);
            FindObjectOfType<SceneFader>().FadeTo(nextLevel);
        }
    }

    public void GoTOPrevLesson()
    {
        if (currentLesson > 0)
        {
            lessons[currentLesson].gameObject.SetActive(false);
            currentLesson--;
            lessons[currentLesson].gameObject.SetActive(true);
        }
    }

    public void loadLevel(string name)
    {
        if (SceneManager.GetActiveScene().name == name)
            return;
        FindObjectOfType<AudioManager>().Stop("LevelTheme");
        StartCoroutine(LoadAsychronously(name));
    }

    IEnumerator LoadAsychronously(string name)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    public void closeApp()
    {
        Debug.Log("Closing App");
        Application.Quit();
    }
}
