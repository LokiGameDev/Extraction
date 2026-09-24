using UnityEngine;

public class EnemyObject : MonoBehaviour
{
    private EnemyObjectSpawner enemyObjectSpawner;

    private Transform currentTarget;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 5f;

    private bool isAttacked = false;

    public void Setup(EnemyObjectSpawner enemyObjectSpawner)
    {
        this.enemyObjectSpawner = enemyObjectSpawner;
    }

    void Start()
    {
        currentTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.position - rb.position).normalized;

            rb.MovePosition(
                rb.position + direction * speed * Time.fixedDeltaTime
            );

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                rb.MoveRotation(targetRotation);
            }
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player") && !isAttacked)
        {
            isAttacked = true;
            GamePlayManager.Instance.PlayerGotHit(2, transform.position);
            enemyObjectSpawner.ReturnEnemyObject(this);
        }
    }
}
