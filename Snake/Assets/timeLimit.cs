using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeLimit : MonoBehaviour
{

    public float timeLeft { get; private set; }

    // Use this for initialization
    void Start()
    {
        timeLeft = Random.Range(4, 9);

    }
	
	// Update is called once per frame
	void Update()
    {
        if (GameLogic.gamePaused || GameLogic.gameOver) return;
        timeLeft -= Time.deltaTime;
        var timer = gameObject.transform.GetChild(0);
        var mesh = timer.GetComponent<TextMesh>();
        mesh.text = ((int)timeLeft).ToString();
        if (timeLeft <= 0)
            Destroy(gameObject);
    }

}
