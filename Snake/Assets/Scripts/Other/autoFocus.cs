using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class autoFocus : MonoBehaviour
{

    EventSystem eventSystem;

	void Start()
    {
        eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(gameObject);
    }
	
	void Update()
    {
        if (eventSystem.currentSelectedGameObject == null)
            eventSystem.SetSelectedGameObject(gameObject);
    }
}
