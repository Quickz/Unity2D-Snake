using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{

    GameObject start;
    EventSystem eventSystem;

    void Start()
    {
        start = GameObject.Find("startGame");
        eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(start);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (eventSystem.currentSelectedGameObject == null)
            eventSystem.SetSelectedGameObject(start);

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
