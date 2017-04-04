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

    public static Transform allObstacles;

    public static bool gameOver { get; private set; }
    public static bool gamePaused { get; private set; }
    
    public static float time { get; private set; }
    public static float gameSpeed { get; private set; }
    public static int score { get; private set; }

    public static float mapX { get; private set; }
    public static float mapY { get; private set; }

    public static Transform game;
    public static Transform snake { get; private set; }
    public static Transform food { get; private set; }

    public static TextMesh scoreObj { get; private set; }
    static GameObject gameOverNote;
    GameObject gamePausedNote;

    Menu menu;
    GameObject currWarning;
    static GameObject darkBackground;

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

        enemy = null;

        allFood = GameObject.Find("allFood").transform;
        allObstacles = GameObject.Find("allObstacles").transform;
        RespawnFood(food);

        menu = gameObject.GetComponent<Menu>();

    }

    // Update is called once per frame
    void Update()
    {

        // exit to main menu
        if (Input.GetKeyDown("escape"))
            ToggleWarning(false);

        // restart the game
        if (Input.GetKeyDown("r"))
            ToggleWarning(true);

        if (gameOver) return;

        if (Input.GetKeyDown("p"))
        {
            if (!gamePaused) PauseGame();
            else
            {
                if (currWarning != null)
                    Destroy(currWarning);
                ResumeGame();

            }
        }
        else if (gamePaused)
            return;
        else
            player.CheckForInput();

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
    void ToggleWarning(bool toggleRestart)
    {
        if (currWarning == null)
        {
            if (toggleRestart)
                CreateRestartWarning();
            else
                CreateReturnWarning();
            if (!gamePaused && !gameOver) PauseGame();
        }
        else
        {
            if (!toggleRestart)
            {
                Destroy(currWarning);
                if (!gameOver) ResumeGame();
            }
        }
    }

    public void CreateReturnWarning()
    {
        if (gameOver)
        {
            menu.ChangeScene("Main Menu");
            return;
        }

        currWarning = Warning.CreateWarning("Are you sure you want to return to main menu?");

        var warning = currWarning.transform;

        // obtaining button component
        var yesBtn = Warning.GetYesBtn(currWarning);
        var noBtn = Warning.GetNoBtn(currWarning);

        // adding button events
        yesBtn.onClick.AddListener(() => menu.ChangeScene("Main Menu"));
        noBtn.onClick.AddListener(() => ToggleWarning(false));

    }

    public static GameObject CreateFood(string name)
    {
        var food = Object.Instantiate(
            Resources.Load("food/" + name)
        ) as GameObject;

        food.transform.parent = allFood;
        food.name = name;
        return food;
    }

    // creates a red enemy snake
    public static void CreateEnemy()
    {
        if (enemy != null) return;

        var enemyObj = Object.Instantiate(
            Resources.Load("enemy/enemy")
        ) as GameObject;

        enemyObj.transform.parent = game.transform;
        enemy = enemyObj.GetComponent<Enemy>();

        // adding a random amount of extra length to the enemy tail
        int extraLength = Random.Range(0, 3);
        for (int i = 0; i < extraLength; i++)
            Snake.GrowSnake(enemyObj.transform, "enemy/enemyTail");

    }

    // moves the food to a random position
    public static void RespawnFood(Transform food)
    {

        Vector2 pos = food.position;

        // (int) rounds up the random value
        // the f makes sure the result is float
        pos.x = (int)Random.Range(-mapX * 2, mapX * 2) / 2f;
        pos.y = (int)Random.Range(-mapY * 2, mapY * 2) / 2f;

        bool inSnake = !GameLogic.CheckContainerCoord(snake, pos);
        bool inObstacle = !GameLogic.CheckContainerCoord(allObstacles, pos);
        bool inEnemy = false;

        if (enemy != null)
            inEnemy = !GameLogic.CheckContainerCoord(enemy.snake, pos);
        
        if (inSnake || inEnemy || inObstacle)
        {
            RespawnFood(food);
            return;
        }

        food.position = pos;

    }

    // checks if coordinate is for the specified transform
    public static bool CheckContainerCoord(Transform container, Vector2 objPos)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            Vector2 tailPos = container.GetChild(i).position;
            if (tailPos == objPos)
                return false;
        }
        return true;
    }

    public static void UpScore(int addition = 10)
    {
        score += addition;
        scoreObj.text = "Score: " + score;
    }

    // checks if the head is inside the tail
    public static void GameOver()
    {
        darkBackground = Instantiate(
            Resources.Load("overlay/darkBackground")
            ) as GameObject;

        GameOverNotification();
        SetHeadColor(Color.red);
        gameOver = true;

        time = 0;
        ScoresMenu.SaveScore();
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
            Resources.Load("overlay/gameOver")
            ) as GameObject;
        gameOverNote.name = "gameOver";
    }

    void CreateRestartWarning()
    {
        if (gameOver)
        {
            RestartGame();
            return;
        }

        currWarning = Warning.CreateWarning("Are you sure you want to restart the game?");

        var warning = currWarning.transform;

        // obtaining button component
        var yesBtn = Warning.GetYesBtn(currWarning);
        var noBtn = Warning.GetNoBtn(currWarning);

        // adding button events
        yesBtn.onClick.AddListener(() => RestartGame());
        noBtn.onClick.AddListener(() => ResumeGame());
    }

    public static void PlaySound(string dir)
    {
        Transform game = GameObject.Find("game").transform;
        AudioSource audio = game.GetComponent<AudioSource>();
        audio.clip = Resources.Load("sounds/" + dir) as AudioClip;
        audio.Play();
    }

    void RestartGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    
    void PauseGame()
    {
        gamePausedNote = Instantiate(
            Resources.Load("overlay/gamePaused")
            ) as GameObject;

        darkBackground = Instantiate(
            Resources.Load("overlay/darkBackground")
            ) as GameObject;

        gamePausedNote.name = "gamePaused";
        gamePaused = true;
    }

    void ResumeGame()
    {
        Destroy(darkBackground);
        Destroy(gamePausedNote);
        gamePaused = false;
    }

}
