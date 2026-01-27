using UnityEngine;

public class EnemyArmored : Enemy
{
    [Header("Directional Slash Settings")]
    public Vector3 requiredSlashDirection = Vector3.down; // Default: slash downward
    [Range(0, 90)]
    public float allowedAngleError = 45f; // Degrees - adjustable in Inspector
    public float blockEffectDuration = 0.3f;

    [Header("Visuals")]
    public GameObject weakPointArrow; // Drag arrow child object here
    public Material correctSlashMaterial;
    public Material blockedSlashMaterial;

    private Renderer enemyRenderer;
    private Material originalMaterial;
    private bool isBlocked = false;

    protected override void Start()
    {
        base.Start();
        enemyType = EnemyType.Armored;

        // Get renderer for material changes
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            originalMaterial = enemyRenderer.material;
        }

        // Randomize required direction (down, up, left, right)
        RandomizeSlashDirection();

        // Setup arrow visual
        SetupArrowVisual();
    }

    void RandomizeSlashDirection()
    {
        int randomDir = Random.Range(0, 4);
        switch (randomDir)
        {
            case 0: requiredSlashDirection = Vector3.down; break;    // ↓
            case 1: requiredSlashDirection = Vector3.up; break;      // ↑
            case 2: requiredSlashDirection = Vector3.left; break;    // ←
            case 3: requiredSlashDirection = Vector3.right; break;   // →
        }
    }

    void SetupArrowVisual()
    {
        if (weakPointArrow != null)
        {
            // Make arrow point in the required slash direction
            weakPointArrow.transform.rotation = Quaternion.LookRotation(requiredSlashDirection);

            // Color code based on direction (optional)
            SetArrowColor();
        }
    }

    void SetArrowColor()
    {
        if (weakPointArrow == null) return;

        Renderer arrowRenderer = weakPointArrow.GetComponent<Renderer>();
        if (arrowRenderer != null)
        {
            Color arrowColor = Color.white;

            // Color code directions
            if (requiredSlashDirection == Vector3.down) arrowColor = Color.red;
            else if (requiredSlashDirection == Vector3.up) arrowColor = Color.green;
            else if (requiredSlashDirection == Vector3.left) arrowColor = Color.blue;
            else if (requiredSlashDirection == Vector3.right) arrowColor = Color.yellow;

            arrowRenderer.material.color = arrowColor;
        }
    }

    public override void OnSliced(Vector3 sliceDirection)
    {
        if (isBlocked) return;

        Vector3 sliceDir = sliceDirection.normalized;
        Vector3 requiredDir = requiredSlashDirection.normalized;

        float dot = Vector3.Dot(sliceDir, requiredDir);

        Debug.Log($"DOT = {dot}");

        // مثلا حداقل 0.7 یعنی حدود 45 درجه خطا
        if (dot >= 0.7f)
        {
            DieCorrectly(sliceDir);
        }
        else
        {
            BlockAttack();
        }
    }
    void DieCorrectly(Vector3 sliceDir)
    {
        Debug.Log("ARMORED KILLED");

        // افکت
        GetComponent<EnemyEffects>()?.PlaySliceEffect(transform.position, sliceDir);

        // صدا
        // AudioSource.PlayClipAtPoint(killSound, transform.position);

        GameManager.Instance?.AddScore(scoreValue);

        Destroy(gameObject);
    }
    void BlockAttack()
    {
        isBlocked = true;

        Debug.Log("BLOCKED!");

        // تغییر متریال
        if (enemyRenderer && blockedSlashMaterial)
            enemyRenderer.material = blockedSlashMaterial;

        // TODO: metal sound + spark particle

        Invoke(nameof(ResetBlock), blockEffectDuration);
    }

    void ResetBlock()
    {
        if (enemyRenderer && originalMaterial)
            enemyRenderer.material = originalMaterial;

        isBlocked = false;
    }

    void HandleCorrectSlash(Vector3 sliceDirection)
    {
        Debug.Log("Correct directional slash! +" + scoreValue + " points");

        // Visual feedback for correct slash
        if (enemyRenderer != null && correctSlashMaterial != null)
        {
            enemyRenderer.material = correctSlashMaterial;
        }

        // Add score and destroy
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        // TODO: Play correct slash VFX and sound
        Destroy(gameObject, 0.1f); // Small delay for visual feedback
    }

    void HandleBlockedSlash()
    {
        Debug.Log("Wrong direction! Attack blocked.");
        isBlocked = true;

        // Visual feedback for blocked attack
        if (enemyRenderer != null && blockedSlashMaterial != null)
        {
            enemyRenderer.material = blockedSlashMaterial;
        }

        // TODO: Play block/deflect VFX and metal clash sound

        // Revert material after delay
        Invoke("RevertMaterial", blockEffectDuration);

        // No score, enemy survives
    }

    void RevertMaterial()
    {
        if (enemyRenderer != null && originalMaterial != null)
        {
            enemyRenderer.material = originalMaterial;
        }
        isBlocked = false;
    }

    // Visual debug in editor
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // Draw required slash direction
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, requiredSlashDirection * 2f);

        // Draw allowed angle cone
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        DrawAngleCone(requiredSlashDirection, allowedAngleError);
    }

    void DrawAngleCone(Vector3 direction, float angle)
    {
        // Simplified cone visualization
        Vector3 leftBoundary = Quaternion.Euler(0, -angle, 0) * direction * 2f;
        Vector3 rightBoundary = Quaternion.Euler(0, angle, 0) * direction * 2f;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawLine(transform.position + leftBoundary, transform.position + rightBoundary);
    }
}