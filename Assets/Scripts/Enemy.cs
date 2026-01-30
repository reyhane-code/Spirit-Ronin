using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int scoreValue = 100;
    public float moveSpeed = 5f;
    public EnemyType enemyType;
    public float targetXRange = 3f;
    protected Rigidbody rb;
    protected Vector3 targetPosition;

    public enum EnemyType
    {
        Armored,
        Splitter,
        Friendly
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;

        // Find PlayArea
        GameObject playArea = GameObject.Find("PlayArea");
        if (playArea != null)
        {
            float randomX = Random.Range(-targetXRange, targetXRange);

            float targetY = playArea.transform.position.y;

            targetPosition = new Vector3(randomX, targetY, playArea.transform.position.z);
        }
    }

    protected virtual void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) > 15f)
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnSliced(Vector3 sliceDirection)
    {
        GameManager.Instance?.AddScore(scoreValue);
        Destroy(gameObject);
    }

    public virtual void OnTapped()
    {
        GameManager.Instance?.AddScore(scoreValue);
        Destroy(gameObject);
    }

    public virtual bool CanBeTapped()
    {
        return enemyType == EnemyType.Friendly;
    }

    void OnDestroy()
    {
        SpawnManager sm = FindObjectOfType<SpawnManager>();
        if (sm != null)
            sm.RemoveX(transform.position.x);
    }
}
