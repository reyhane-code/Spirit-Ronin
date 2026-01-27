using UnityEngine;

public class EnemySplitter : Enemy
{
    public GameObject splitPrefab;
    public int splitCount = 2;
    public bool canSplit = true;

    public override void OnSliced(Vector3 sliceDirection)
    {
        if (canSplit)
        {
            Split();
        }
        else
        {
            // Small ones die normally
            GameManager.Instance?.AddScore(scoreValue);
        }

        Destroy(gameObject);
    }

    void Split()
    {
        for (int i = 0; i < splitCount; i++)
        {
            Vector3 offset = Random.insideUnitSphere * 0.5f;
            offset.y = 0;

            GameObject small = Instantiate(splitPrefab, transform.position + offset, Quaternion.identity);

            EnemySplitter splitter = small.GetComponent<EnemySplitter>();
            splitter.canSplit = false;
            splitter.moveSpeed = moveSpeed * 1.5f;
            splitter.scoreValue = Mathf.RoundToInt(scoreValue * 0.5f);
        }
    }
}
