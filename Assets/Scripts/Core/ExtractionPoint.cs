using UnityEngine;

public class ExtractionPoint : MonoBehaviour
{
    private bool isNotified = false;
    public void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player") && !isNotified)
        {
            isNotified = true;
            GamePlayManager.Instance.PlayerAreaState(true);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            isNotified = false;
            GamePlayManager.Instance.PlayerAreaState(false);
        }
    }
}
