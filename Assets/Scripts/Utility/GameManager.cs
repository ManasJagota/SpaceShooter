using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MjCreates.Events;

/// <summary>
/// Class which manages the game
/// </summary>
public class GameManager : MonoBehaviour
{
    // The script that manages all others
    public static GameManager instance = null;

    [Tooltip("The UIManager component which manages the current scene's UI")]
    public UIManager uiManager = null;

    [Tooltip("The player gameobject")]
    public GameObject player = null;

    [Header("Scores")]
    // The current player score in the game
    [Tooltip("The player's score")]
    [SerializeField] private int gameManagerScore = 0;

    // Static getter/setter for player score (for convenience)
    public static int score
    {
        get
        {
            return instance.gameManagerScore;
        }
        set
        {
            instance.gameManagerScore = value;
        }
    }

    // The highest score obtained by this player
    [Tooltip("The highest score acheived on this device")]
    public int highScore = 0;

    [Header("Game Progress / Victory Settings")]
    [Tooltip("Whether the game is winnable or not \nDefault: true")]
    public bool gameIsWinnable = true;
    [Tooltip("The number of enemies that must be defeated to win the game")]
    public int enemiesToDefeat = 10;
    
    // The number of enemies defeated in game
    private int enemiesDefeated = 0;

    [Tooltip("Whether or not to print debug statements about whether the game can be won or not according to the game manager's" +
        " search at start up")]
    public bool printDebugOfWinnableStatus = true;
    [Tooltip("Page index in the UIManager to go to on winning the game")]
    public int gameVictoryPageIndex = 0;
    [Tooltip("The effect to create upon winning the game")]
    public GameObject victoryEffect;

    //The number of enemies observed by the game manager in this scene at start up"
    private int numberOfEnemiesFoundAtStart;

    /// <summary>
    /// Description:
    /// Standard Unity function called when the script is loaded, called before start
    /// 
    /// When this component is first added or activated, setup the global reference
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    /// <summary>
    /// Description:
    /// Subscribes to the gameplay events this manager reacts to. Paired with OnDisable so the
    /// handlers never outlive the manager.
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void OnEnable()
    {
        EventManager.Subscribe<EnemyDefeatedEvent>(OnEnemyDefeated);
        EventManager.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    /// <summary>
    /// Description:
    /// Unsubscribes from the gameplay events.
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void OnDisable()
    {
        EventManager.Unsubscribe<EnemyDefeatedEvent>(OnEnemyDefeated);
        EventManager.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    /// <summary>
    /// Description:
    /// Reacts to an enemy being defeated: awards its score and counts it toward the level's
    /// victory condition. Ignored once the game is over (mirrors the old in-enemy guard).
    /// Input:
    /// EnemyDefeatedEvent a_event (carries the defeated enemy's score value)
    /// Return:
    /// void (no return)
    /// </summary>
    private void OnEnemyDefeated(EnemyDefeatedEvent a_event)
    {
        if (gameIsOver)
        {
            return;
        }
        AddScore(a_event.ScoreValue);
        IncrementEnemiesDefeated();
    }

    /// <summary>
    /// Description:
    /// Reacts to the player dying by ending the game.
    /// Input:
    /// PlayerDiedEvent a_event
    /// Return:
    /// void (no return)
    /// </summary>
    private void OnPlayerDied(PlayerDiedEvent a_event)
    {
        GameOver();
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once before the first Update
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void Start()
    {
        HandleStartUp();
    }

    /// <summary>
    /// Description:
    /// Handles necessary activities on start up such as getting the highscore and score, updating UI elements, 
    /// and checking the number of enemies
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void HandleStartUp()
    {
        if (PlayerPrefs.HasKey(GameConstants.c_HighScoreKey))
        {
            highScore = PlayerPrefs.GetInt(GameConstants.c_HighScoreKey);
        }
        if (PlayerPrefs.HasKey(GameConstants.c_ScoreKey))
        {
            score = PlayerPrefs.GetInt(GameConstants.c_ScoreKey);
        }
        EventManager.Publish(new HighScoreChangedEvent(highScore));
        EventManager.Publish(new ScoreChangedEvent(score));
        if (printDebugOfWinnableStatus)
        {
            FigureOutHowManyEnemiesExist();
        }
    }

    /// <summary>
    /// Description:
    /// Searches the level for all spawners and static enemies.
    /// Only produces debug messages / warnings if the game is set to be winnable
    /// If there are any infinite spawners a debug message will say so,
    /// If there are more enemies than the number of enemies to defeat to win
    /// then a debug message will say so
    /// If there are too few enemies to defeat to win then a debug warning will say so
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void FigureOutHowManyEnemiesExist()
    {
        List<EnemySpawner> enemySpawners = FindObjectsOfType<EnemySpawner>().ToList();
        List<Enemy> staticEnemies = FindObjectsOfType<Enemy>().ToList();

        int numberOfInfiniteSpawners = 0;
        int enemiesFromSpawners = 0;
        int enemiesFromStatic = staticEnemies.Count;
        foreach(EnemySpawner enemySpawner in enemySpawners)
        {
            if (enemySpawner.spawnInfinite)
            {
                numberOfInfiniteSpawners += 1;
            }
            else
            {
                enemiesFromSpawners += enemySpawner.maxSpawn;
            }
        }
        numberOfEnemiesFoundAtStart = enemiesFromSpawners + enemiesFromStatic;

        if (gameIsWinnable)
        {
            if (numberOfInfiniteSpawners > 0)
            {
                Debug.Log("There are " + numberOfInfiniteSpawners + " infinite spawners " + " so the level will always be winnable, "
                    + "\nhowever you sshould still playtest for timely completion");
            }
            else if (enemiesToDefeat > numberOfEnemiesFoundAtStart)
            {
                Debug.LogWarning("There are " + enemiesToDefeat + " enemies to defeat but only " + numberOfEnemiesFoundAtStart + 
                    " enemies found at start \nThe level can not be completed!");
            }
            else
            {
                Debug.Log("There are " + enemiesToDefeat + " enemies to defeat and " + numberOfEnemiesFoundAtStart +
                    " enemies found at start \nThe level can completed");
            }
        }
    }

    /// <summary>
    /// Description:
    /// Increments the number of enemies defeated by 1
    /// Input:
    /// none
    /// Return:
    /// void (no returned value)
    /// </summary>
    public void IncrementEnemiesDefeated()
    {
        enemiesDefeated++;
        if (enemiesDefeated >= enemiesToDefeat && gameIsWinnable)
        {
            LevelCleared();
        }
    }

    /// <summary>
    /// Description:
    /// Configures the shared GameManager for a freshly loaded level prefab.
    /// Copies the level-specific values from the Level component, resets the
    /// per-level state (enemies defeated, game over flag), re-enables the player,
    /// and clears any open UI pages so gameplay can resume. Called by LevelManager
    /// each time a level prefab is instantiated.
    /// Input:
    /// Level level (the Level component on the instantiated level prefab; may be null)
    /// Return:
    /// void (no return)
    /// </summary>
    /// <param name="a_level">The Level config component from the loaded level prefab</param>
    public void ConfigureForLevel(Level a_level)
    {
        if (a_level != null)
        {
            enemiesToDefeat = a_level.EnemiesToDefeat;
            victoryEffect = a_level.VictoryEffect;
        }

        enemiesDefeated = 0;
        gameIsOver = false;
        // Loading a level means we are in a playable, winnable state. Set this here so
        // it does not depend on the (persistent) GameManager's authored menu value.
        gameIsWinnable = true;

        if (player != null)
        {
            player.SetActive(true);
        }

        if (uiManager != null)
        {
            uiManager.allowPause = true;
            uiManager.SetActiveAllPages(false);
        }

        if (printDebugOfWinnableStatus)
        {
            FigureOutHowManyEnemiesExist();
        }

        EventManager.Publish(new ScoreChangedEvent(score));
        EventManager.Publish(new HighScoreChangedEvent(highScore));
        EventManager.Publish(new LevelLoadedEvent());
    }

    /// <summary>
    /// Description:
    /// Standard Unity function that gets called when the application (or playmode) ends
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveHighScore();
        ResetScore();
    }

    /// <summary>
    /// Description:
    /// Adds a number to the player's score stored in the gameManager
    /// Input: 
    /// int scoreAmount
    /// Returns: 
    /// void (no return)
    /// </summary>
    /// <param name="scoreAmount">The amount to add to the score</param>
    public static void AddScore(int scoreAmount)
    {
        score += scoreAmount;
        if (score > instance.highScore)
        {
            SaveHighScore();
        }
        EventManager.Publish(new ScoreChangedEvent(score));
    }
    
    /// <summary>
    /// Description:
    /// Resets the current player score
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public static void ResetScore()
    {
        PlayerPrefs.SetInt(GameConstants.c_ScoreKey, 0);
        score = 0;
        EventManager.Publish(new ScoreChangedEvent(0));
    }

    /// <summary>
    /// Description:
    /// Saves the player's highscore
    /// Input: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public static void SaveHighScore()
    {
        if (score > instance.highScore)
        {
            PlayerPrefs.SetInt(GameConstants.c_HighScoreKey, score);
            instance.highScore = score;
            EventManager.Publish(new HighScoreChangedEvent(instance.highScore));
        }
    }

    /// <summary>
    /// Description:
    /// Resets the high score in player preferences
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public static void ResetHighScore()
    {
        PlayerPrefs.SetInt(GameConstants.c_HighScoreKey, 0);
        if (instance != null)
        {
            instance.highScore = 0;
        }
        EventManager.Publish(new HighScoreChangedEvent(0));
    }

    /// <summary>
    /// Description:
    /// Ends the level, meant to be called when the level is complete (enough enemies have been defeated)
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    public void LevelCleared()
    {
        PlayerPrefs.SetInt(GameConstants.c_ScoreKey, score);
        if (uiManager != null)
        {
            player.SetActive(false);
            uiManager.allowPause = false;
            uiManager.GoToPage(gameVictoryPageIndex);
            if (victoryEffect != null)
            {
                Instantiate(victoryEffect, transform.position, transform.rotation, null);
            }
        }
        EventManager.Publish(new LevelClearedEvent());
    }

    [Header("Game Over Settings:")]
    [Tooltip("The index in the UI manager of the game over page")]
    public int gameOverPageIndex = 0;
    [Tooltip("The game over effect to create when the game is lost")]
    public GameObject gameOverEffect;

    // Whether or not the game is over
    [HideInInspector]
    public bool gameIsOver = false;

    /// <summary>
    /// Description:
    /// Displays game over screen
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    public void GameOver()
    {
        gameIsOver = true;
        if (gameOverEffect != null)
        {
            Instantiate(gameOverEffect, transform.position, transform.rotation, null);
        }
        if (uiManager != null)
        {
            uiManager.allowPause = false;
            uiManager.GoToPage(gameOverPageIndex);
        }
        EventManager.Publish(new GameOverEvent());
    }
}
