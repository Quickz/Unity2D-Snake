using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoresMenu : MonoBehaviour
{
    // scores
    GameObject canvas;
    GameObject resetBtn;
    GameObject backBtn;

    // warning
    GameObject warning;
    GameObject yesBtn;

    List<Text> scoreTxt;

	void Start()
    {
        scoreTxt = new List<Text>();
        GetScoreContainers();
        UpdateScoreDisplay();
        canvas = GameObject.Find("Canvas");
        backBtn = GameObject.Find("back");
        resetBtn = GameObject.Find("reset");
        warning = GameObject.Find("resetWarning");
        yesBtn = GameObject.Find("yesBtn");
        HideWarning();
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (canvas.activeSelf)
                ChangeScene("Main Menu");
            else
                HideWarning();
        }
        Menu.RestoreFocus(canvas.activeSelf ? backBtn : yesBtn);
    }

    public void ResetScores()
    {
        PlayerPrefs.DeleteAll();
        UpdateScoreDisplay();
    }

    public static void SaveScore()
    {
        List<int> score = LoadScores();
        score.Add(GameLogic.score);

        // sorting scores in descending order
        score.Sort((x, y) => y - x);

        // removing the lowest score
        score.Remove(score.Last());

        // saving the scores
        for (int i = 0; i < score.Count; i++)
            PlayerPrefs.SetInt("score" + i, score[i]);
    }

    static List<int> LoadScores()
    {
        List<int> score = new List<int>();
        for (int i = 0; i < 5; i++)
            score.Add(PlayerPrefs.GetInt("score" + i));
        return score;
    }

    void GetScoreContainers()
    {
        var scores = GameObject.Find("scores").transform;
        for (int i = 1; i < scores.childCount; i += 2)
        {
            var currScore = scores.GetChild(i);
            scoreTxt.Add(currScore.GetComponent<Text>());
        }
    }

    void UpdateScoreDisplay()
    {
        List<int> score = LoadScores();
        for (int i = 0; i < scoreTxt.Count; i++)
            scoreTxt[i].text = score[i].ToString();
    }

    public void ShowWarning()
    {
        canvas.SetActive(false);
        warning.SetActive(true);
        Menu.SwitchFocus(yesBtn);
    }

    public void HideWarning()
    {
        canvas.SetActive(true);
        warning.SetActive(false);
        Menu.SwitchFocus(resetBtn);
    }

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

}
