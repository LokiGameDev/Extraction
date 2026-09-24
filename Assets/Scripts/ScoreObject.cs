using UnityEngine;

public class ScoreObject : MonoBehaviour
{
    public int scoreAmount = 0;

    public void SetScore(int amount)
    {
        scoreAmount = amount;
    }

    public void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            GamePlayManager.Instance.PlayerScoreIncrease(scoreAmount);
            Destroy(gameObject);
        }
    }
}
