using System.Collections;
using TMPro;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    private const string PlayerBestTime = "PlayerBestTime";
    private const string PlayerBestScore = "PlayerBestScore";

    private int playerCurrentGameScore = 0;
    private int playerCurrentGameTime = 0;

    public bool IsGameOn { get; private set; } = true;

    [SerializeField] private ScoreObjectSpawner scoreObjectSpawner;
    [SerializeField] private EnemyObjectSpawner enemyObjectSpawner;
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private TMP_Text currentTimeText;
    [SerializeField] private TMP_Text currentScoreText;

    private static GamePlayManager instance;
    public static GamePlayManager Instance
    {
        get
        {
            if(instance==null)
            {
                Debug.LogError("Gameplaymanager is null");
            }
            return instance;
        }
    }

    void Awake()
    {
        if(instance != null && instance != this) Destroy(this);
        if(instance == null) instance = this;
    }

    public void StartTheGame()
    {
        playerCurrentGameScore = 0;
        playerCurrentGameTime = 0;

        Instantiate(playerPrefab);
        
        scoreObjectSpawner.StartSpawning();
        enemyObjectSpawner.StartSpawning();

        currentScoreText.text = $"{playerCurrentGameScore}";
        currentTimeText.text = $"{playerCurrentGameTime}";
        
        StartCoroutine(GameTimeCounter());
    }

    public void GameEndedByDeath()
    {
        IsGameOn = false;
        if(playerCurrentGameTime > PlayerPrefs.GetFloat(PlayerBestTime))
        {
            PlayerPrefs.SetFloat(PlayerBestTime, playerCurrentGameTime);
        }
        if(playerCurrentGameScore > PlayerPrefs.GetFloat(PlayerBestScore))
        {
            PlayerPrefs.SetFloat(PlayerBestScore, playerCurrentGameScore);
        }
    }

    public void GameEndedByExtract()
    {
        IsGameOn = false;
        if(playerCurrentGameTime > PlayerPrefs.GetFloat(PlayerBestTime))
        {
            PlayerPrefs.SetFloat(PlayerBestTime, playerCurrentGameTime);
        }
        if(playerCurrentGameScore > PlayerPrefs.GetFloat(PlayerBestScore))
        {
            PlayerPrefs.SetFloat(PlayerBestScore, playerCurrentGameScore);
        }
    }

    public void PlayerScoreIncrease(int amount)
    {
        playerCurrentGameScore += amount;
        currentScoreText.text = $"{playerCurrentGameScore}";
    }

    private void OnApplicationQuit()
    {
        CheckForNewBest();
    }

    private void CheckForNewBest()
    {
        PlayerData currentBestData = PlayerDataSaver.LoadPlayerData();

        int bestScore = currentBestData.playerBestScore;
        PlayerTime bestTime = currentBestData.playerBestTime;

        if(playerCurrentGameScore > bestScore)
        {
            bestScore = playerCurrentGameScore;
        }
        if(playerCurrentGameTime > PlayerDataSaver.ConvertTimeToSeconds(currentBestData.playerBestTime))
        {
            bestTime = PlayerDataSaver.ConvertSecondsToTime(playerCurrentGameTime);
        }

        PlayerDataSaver.SavePlayerData(bestTime, bestScore);
    }

    private IEnumerator GameTimeCounter()
    {
        while(IsGameOn)
        {
            playerCurrentGameTime += 1;
            PlayerTime player = PlayerDataSaver.ConvertSecondsToTime(playerCurrentGameTime);
            currentTimeText.text = $"{player.bestTimeHour:D2}:{player.bestTimeMinute:D2}:{player.bestTimeSecond:D2}";
            yield return new WaitForSeconds(1);
        }
    }
}
