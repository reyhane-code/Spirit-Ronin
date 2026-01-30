using UnityEngine;

public class EnemyArmored : Enemy
{
    [Range(0f, 1f)] public float alignmentThreshold = 0.7f;
    public AudioClip blockSound;
    public AudioClip hitSound;
    public ParticleSystem hitParticlePrefab;
    public Color blockColor = Color.red;
    private Renderer enemyRenderer;

    protected override void Start()
    {
        base.Start();
        enemyRenderer = GetComponentInChildren<Renderer>();
    }

    public override void OnSliced(Vector3 sliceDir)
    {
        if (sliceDir.sqrMagnitude < 0.0001f)
            return;

        sliceDir.z = 0f;
        sliceDir.Normalize();

        Vector3 correctDir = Vector3.right;
        float dot = Vector3.Dot(sliceDir, correctDir);
        float absDot = Mathf.Abs(dot);
        float angle = Mathf.Acos(Mathf.Clamp(dot, -1f, 1f)) * Mathf.Rad2Deg;


        if (absDot >= alignmentThreshold)
        {
            PlayHit();
            Die();
        }
        else
        {
            PlayBlock();
        }
    }


    private void PlayBlock()
    {
        if (enemyRenderer != null)
            enemyRenderer.material.color = blockColor;

        if (blockSound != null)
            AudioSource.PlayClipAtPoint(blockSound, transform.position);
    }

    private void PlayHit()
    {
        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, transform.position);

        if (hitParticlePrefab != null)
            Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
    }

    private void Die()
    {
        GameManager.Instance?.AddScore(scoreValue);
        Destroy(gameObject);
    }
}
