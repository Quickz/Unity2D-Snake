using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{

    GameObject start;
    GameObject scores;
    static EventSystem eventSystem;
    static bool scoreFocused;

    void Start()
    {
        start = GameObject.Find("startGame");
        scores = GameObject.Find("scores");
        InitMenuControls(start);
        if (scoreFocused) SwitchFocus(scores);
        scoreFocused = false;
    }

    void Update()
    {
        RestoreFocus(start);

    }

    public static void RestoreFocus(GameObject button)
    {
        if (eventSystem.currentSelectedGameObject == null)
            eventSystem.SetSelectedGameObject(button);
    }

    // hides cursor and moves focus to a button
    public static void InitMenuControls(GameObject button)
    {
        SwitchFocus(button);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void SwitchFocus(GameObject button)
    {
        eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(button);
    }

    public void ChangeScene(string scene)
    {
        if (scene == "Scores") scoreFocused = true;
        SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
