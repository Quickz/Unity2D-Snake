using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{

    GameObject start;
    static EventSystem eventSystem;

    void Start()
    {
        start = GameObject.Find("startGame");
        InitMenuControls(start);

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
        SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
