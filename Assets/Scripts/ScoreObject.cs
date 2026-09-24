using UnityEngine;

public class ScoreObject : MonoBehaviour
{
    public int scoreAmount = 0;
    private ScoreObjectSpawner scoreObjectSpawner;
    private bool isCollected = false;

    public void Setup(int amount, ScoreObjectSpawner scoreObjectSpawner)
    {
        scoreAmount = amount;
        this.scoreObjectSpawner = scoreObjectSpawner;
    }

    public void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            GamePlayManager.Instance.PlayerScoreIncrease(scoreAmount);
            scoreObjectSpawner.ReturnScore(this);
        }
    }
}
