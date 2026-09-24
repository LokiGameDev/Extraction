using System.IO;
using UnityEngine;

public static class PlayerDataSaver
{
    public static void SavePlayerData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        
        PlayerPrefs.SetString("SaveData", json);
        PlayerPrefs.Save();
    }

    public static void SavePlayerData(PlayerTime time, int score)
    {
        PlayerData data = new()
        {
            playerBestScore = score,
            playerBestTime = time
        };

        string json = JsonUtility.ToJson(data, true);

        PlayerPrefs.SetString("SaveData", json);
        PlayerPrefs.Save();
    }

    public static PlayerData LoadPlayerData()
    {
        string json = PlayerPrefs.GetString("SaveData", "");

        if (string.IsNullOrEmpty(json))
        {
            PlayerData data = new PlayerData(0,0,0,0);
            json = JsonUtility.ToJson(data, true);;
        }

        return JsonUtility.FromJson<PlayerData>(json);
    }

    public static PlayerTime ConvertSecondsToTime(int seconds)
    {
        PlayerTime time = new();

        while(seconds >= 3600)
        {
            time.bestTimeHour += 1;
            seconds -= 3600;
        }

        while(seconds >= 60)
        {
            time.bestTimeMinute += 1;
            seconds -= 60;
        }

        time.bestTimeSecond = seconds;

        return time;
    }

    public static int ConvertTimeToSeconds(PlayerTime time)
    {
        int seconds = 0;

        seconds += time.bestTimeHour * 3600;
        seconds += time.bestTimeMinute * 60;
        seconds += time.bestTimeSecond;

        return seconds;
    }
}

[System.Serializable]
public class PlayerData
{
    public PlayerTime playerBestTime;
    public int playerBestScore;

    public PlayerData(int hr, int min, int sec, int score)
    {
        playerBestScore = score;
        
        playerBestTime = new PlayerTime
        {
            bestTimeHour = hr,
            bestTimeMinute = min,
            bestTimeSecond = sec
        };
    }

    public PlayerData()
    {    }
}

[System.Serializable]
public class PlayerTime
{
    public int bestTimeHour = 0;
    public int bestTimeMinute = 0;
    public int bestTimeSecond = 0;
}