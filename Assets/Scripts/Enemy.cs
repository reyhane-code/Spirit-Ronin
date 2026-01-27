using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Base Enemy Settings")]
    public int scoreValue = 100;
    public float moveSpeed = 5f;
    public EnemyType enemyType;

    [Header("Target X Range")]
    public float targetXRange = 3f;       // بازه X (±)

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
            // X range random
            float randomX = Random.Range(-targetXRange, targetXRange);

            // Keep Y from PlayArea
            float targetY = playArea.transform.position.y;

            targetPosition = new Vector3(randomX, targetY, playArea.transform.position.z);
        }
    }

    protected virtual void Update()
    {
        // Move toward target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Destroy if too far from target (safety)
        if (Vector3.Distance(transform.position, targetPosition) > 15f)
        {
            Destroy(gameObject);
        }
    }

    // Called when blade hits this enemy
    public virtual void OnSliced(Vector3 sliceDirection)
    {
        GameManager.Instance?.AddScore(scoreValue);
        Destroy(gameObject);
    }

    // Called when tapped (for friendly spirits)
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
