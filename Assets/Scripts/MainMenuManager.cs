using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerBestTimeText;
    [SerializeField] private TMP_Text playerBestScoreText;

    private void Start()
    {
        PlayerData playerData = PlayerDataSaver.LoadPlayerData();
        if(playerData==null) PlayerDataSaver.SavePlayerData(new PlayerData(0,0,0,0));
        playerData = PlayerDataSaver.LoadPlayerData();
        playerBestTimeText.text = $"Best Time\n{playerData.playerBestTime.bestTimeHour:D2}:{playerData.playerBestTime.bestTimeMinute:D2}:{playerData.playerBestTime.bestTimeSecond:D2}";
        playerBestScoreText.text = $"Best Score\n{playerData.playerBestScore}";
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}
