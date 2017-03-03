using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomFood : MonoBehaviour
{

    GameLogic gameLogic;
    public bool eatenByPlayer;

	// Use this for initialization
	void Start()
    {
        GameObject game = GameObject.FindWithTag("Game");
        gameLogic = game.GetComponent<GameLogic>();
        eatenByPlayer = false;
	}

    void OnDestroy()
    {
        if (!eatenByPlayer) return;
        int choice = Random.Range(0, 3);

        if (choice == 1)
            SpeedUp();
        else if (choice == 2)
            SpeedDown();
        else
            gameLogic.UpScore();
    }

    void SpeedUp()
    {
        if (eatenByPlayer)
            gameLogic.ChangeScoreColor(176, 255, 179);
        gameLogic.gameSpeed = gameLogic.defaultGameSpeed / 2;
    }

    void SpeedDown()
    {
        if (eatenByPlayer)
            gameLogic.ChangeScoreColor(189, 169, 255);
        gameLogic.gameSpeed = gameLogic.defaultGameSpeed * 2;
    }


}
