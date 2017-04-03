using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{

    GameObject noBtn;
    GameObject yesBtn;

	void Start()
    {
        noBtn = gameObject.transform.GetChild(1).gameObject;
	}
	
	void Update()
    {
        if (Input.GetKeyDown("escape"))
            Destroy(gameObject);
        Menu.RestoreFocus(noBtn);
    }

    public static GameObject CreateWarning(string message)
    {
        var warning = Object.Instantiate(
            Resources.Load("overlay/warning")
            ) as GameObject;

        var txtObj = warning.transform.GetChild(2);
        txtObj.GetComponent<Text>().text = message;
        GetYesBtn(warning).onClick.AddListener(() => Destroy(warning));
        GetNoBtn(warning).onClick.AddListener(() => Destroy(warning));

        return warning;
    }

    public static Button GetYesBtn(GameObject warning)
    {
        return warning.transform.GetChild(0).GetComponent<Button>();
    }

    public static Button GetNoBtn(GameObject warning)
    {
        return warning.transform.GetChild(1).GetComponent<Button>();
    }

}
