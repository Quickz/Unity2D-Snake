using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour
{
    public static Player player { get; private set; }
    public static Enemy enemy { get; private set; }

    public static Transform allFood;

    public static bool gameOver { get; private set; }
    public static bool gamePaused { get; private set; }
    
    public static float time { get; private set; }
    public static float gameSpeed { get; private set; }
    public static int score { get; private set; }

    public static float mapX { get; private set; }
    public static float mapY { get; private set; }

    Transform game;
    public static Transform snake { get; private set; }
    public static Transform food { get; private set; }

    public static TextMesh scoreObj { get; private set; }
    static GameObject gameOverNote;
    GameObject gamePausedNote;

    Menu menu;
    GameObject returnWarning;

    // awake is used to assign variables
    // before all the other objects Start()
    void Awake()
    {
        gameOver = false;
        gamePaused = false;

        gameSpeed = 0.085f;
        time = 0;
        score = 0;

        mapX = 10;
        mapY = 5;

        game = gameObject.transform;
        snake = game.GetChild(0);

        player = snake.GetComponent<Player>();

        food = game. GetChild(1);
        scoreObj = game.GetChild(3).GetComponent<TextMesh>();
        RespawnFood(food);

        enemy = null;

        allFood = GameObject.Find("allFood").transform;
        menu = gameObject.GetComponent<Menu>();

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("escape"))
            ToggleReturnWarning();
        if (Input.GetKeyDown("r"))
            RestartGame();
        if (gameOver)
            return;
        else if (Input.GetKeyDown("p"))
        {
            if (!gamePaused) PauseGame();
            else
            {
                if (returnWarning != null)
                    Destroy(returnWarning);
                ResumeGame();

            }
        }
        else if (gamePaused)
            return;
        else
            player.CheckForInput();

        // spawn enemy - temporary
        if (Input.GetKeyDown("h") && enemy == null)
            CreateEnemy();
        // destroy enemy - temporary
        else if (Input.GetKeyDown("j") && enemy != null && enemy.exit == null)
            enemy.CreateEnemyExit();
        else if (Input.GetKeyDown("f"))
            CreateFood("highQualityFood");
        else if (Input.GetKeyDown("g"))
            CreateFood("randomBonus");

        time += Time.deltaTime;
        if (time >= gameSpeed)
            time = 0;

    }

    public static void ChangeScoreColor(byte r, byte g, byte b)
    {
        scoreObj.color = new Color32(r, g, b, 200);
    }

    // creates/hides a warning message
    // and pauses/unpauses the game
    void ToggleReturnWarning()
    {
        if (returnWarning == null)
        {
            CreateReturnWarning();
            if (!gamePaused && !gameOver) PauseGame();
        }
        else
        {
            Destroy(returnWarning);
            if (!gameOver)
                ResumeGame();
        }
    }

    public void CreateReturnWarning()
    {
        returnWarning = Object.Instantiate(
            Resources.Load("ReturnWarning")
        ) as GameObject;

        var warning = returnWarning.transform;

        // obtaining button component
        var yesBtn = warning.GetChild(0).GetComponent<Button>();
        var noBtn = warning.GetChild(1).GetComponent<Button>();

        // adding button events
        yesBtn.onClick.AddListener(() => menu.ChangeScene("Main Menu"));
        noBtn.onClick.AddListener(() => ToggleReturnWarning());

    }

    public static GameObject CreateFood(string name)
    {
        var food = Object.Instantiate(
            Resources.Load(name)
        ) as GameObject;

        food.transform.parent = allFood;
        food.name = name;
        return food;
    }

    // creates a red enemy snake
    public void CreateEnemy()
    {
        var enemyObj = Object.Instantiate(
            Resources.Load("enemy")
        ) as GameObject;

        enemyObj.transform.parent = game.transform;
        enemy = enemyObj.GetComponent<Enemy>();

    }

    // moves the food to a random position
    public static void RespawnFood(Transform food)
    {

        Vector2 pos = food.position;

        // (int) rounds up the random value
        // the f makes sure the result is float
        pos.x = (int)Random.Range(-mapX * 2, mapX * 2) / 2f;
        pos.y = (int)Random.Range(-mapY * 2, mapY * 2) / 2f;

        // making sure the food doesn't spawn inside the snake
        for (int i = 0; i < snake.childCount; i++)
        {
            Vector2 tailPos = snake.GetChild(i).position;
            if (tailPos == pos)
            {
                RespawnFood(food);
                return;
            }
        }

        if (enemy != null)
        {
            // making sure the food doesn't spawn inside the enemy
            for (int i = 0; i < enemy.snake.childCount; i++)
            {
                Vector2 tailPos = enemy.snake.GetChild(i).position;
                if (tailPos == pos)
                {
                    RespawnFood(food);
                    return;
                }
            }
        }

        food.position = pos;

    }

    public static void UpScore(int addition = 10)
    {
        score += addition;
        scoreObj.text = "Score: " + score;
    }

    // checks if the head is inside the tail
    public static void GameOver()
    {
        GameOverNotification();
        SetHeadColor(Color.red);
        gameOver = true;

        time = 0;

    }

    static void SetHeadColor(Color color)
    {
        var head = snake.GetChild(0);
        var spr = head.transform.GetComponent<SpriteRenderer>();
        spr.color = color;
    }

    static void GameOverNotification()
    {
        gameOverNote = Instantiate(
            Resources.Load("gameOver")
            ) as GameObject;
        gameOverNote.name = "gameOver";
    }

    void RestartGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    
    void PauseGame()
    {
        gamePausedNote = Instantiate(
            Resources.Load("gamePaused")
            ) as GameObject;
        gamePausedNote.name = "gamePaused";
        gamePaused = true;
    }

    void ResumeGame()
    {
        Destroy(gamePausedNote);
        gamePaused = false;
    }

}
