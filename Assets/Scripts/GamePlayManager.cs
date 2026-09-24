using System.Collections;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayManager : MonoBehaviour
{
    private const string PlayerBestTime = "PlayerBestTime";
    private const string PlayerBestScore = "PlayerBestScore";

    private int playerCurrentGameScore = 0;
    private int playerCurrentGameTime = 0;

    public float currentDifficulty = 5;

    public bool IsGameOn { get; private set; } = true;
    private bool isInsideArea = false;

    private PlayerMovement playerMovement;

    [SerializeField] private ScoreObjectSpawner scoreObjectSpawner;
    [SerializeField] private EnemyObjectSpawner enemyObjectSpawner;
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private TMP_Text currentTimeText;
    [SerializeField] private TMP_Text currentScoreText;

    [SerializeField] private CanvasGroup controlsPanel;
    [SerializeField] private TMP_Text introText;
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private GameObject extractedPanel;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private GameObject ectractionText;

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

        restartPanel.SetActive(false);
        extractedPanel.SetActive(false);
        ectractionText.gameObject.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(InitiateTheGame());
    }

    void OnEnable()
    {
        inputReader.OnPlayerExtract += PlayerExtracted;
    }

    void OnDisable()
    {
        inputReader.OnPlayerExtract -= PlayerExtracted;
    }

    private void PlayerExtracted()
    {
        if(isInsideArea)
        {
            GameEndedByExtract();
        }
    }

    private IEnumerator InitiateTheGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        float time = 0;
        while(time < 2)
        {
            time += Time.deltaTime;

            controlsPanel.alpha = 1 - time/2;

            yield return null;
        }
        controlsPanel.alpha = 0;
        introText.gameObject.SetActive(true);

        while (introText.fontSize < 150)
        {
            introText.fontSize += 200f * Time.deltaTime;
            yield return null;
        }

        // Decrease
        while (introText.fontSize > 50)
        {
            introText.fontSize -= 200f * Time.deltaTime;
            yield return null;
        }
        introText.gameObject.SetActive(false);

        StartTheGame();
    }

    public void StartTheGame()
    {
        playerCurrentGameScore = 0;
        playerCurrentGameTime = 0;

        playerMovement = Instantiate(playerPrefab).GetComponent<PlayerMovement>();
        
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        restartPanel.SetActive(true);
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        extractedPanel.SetActive(true);
    }

    public void RestartTheGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void PlayerScoreIncrease(int amount)
    {
        playerCurrentGameScore += amount;
        currentScoreText.text = $"{playerCurrentGameScore}";
    }

    public void PlayerGotHit(int amount, Vector3 position)
    {
        playerMovement.TakeDamage(amount, position);
    }

    private void OnApplicationQuit()
    {
        CheckForNewBest();
    }

    public void PlayerAreaState(bool state)
    {
        isInsideArea = state;
        ectractionText.SetActive(state);
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
