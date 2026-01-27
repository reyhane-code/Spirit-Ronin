using UnityEngine;

public class EnemyFriendly : Enemy
{
    [Header("Friendly Spirit Settings")]
    public Color savedColor = Color.green; // Color when saved
    public int tapScoreBonus = 50; // Extra points for tapping instead of slicing

    private bool wasTapped = false;

    protected override void Start()
    {
        base.Start();
        enemyType = EnemyType.Friendly;

        // Friendly moves slower
        moveSpeed = 3f;
    }

    public override void OnSliced(Vector3 sliceDirection)
    {
        // Friendly spirit was sliced (WRONG!)
        Debug.Log("OH NO! You sliced a friendly spirit! -1 Life");

        // TODO: Play sad VFX
        // TODO: Deduct life
        GameManager.Instance?.LoseLife();

        // Still destroy it
        Destroy(gameObject);
    }

    public override void OnTapped()
    {
        if (wasTapped) return; // Prevent multiple taps

        wasTapped = true;
        Debug.Log("Friendly spirit saved! +" + (scoreValue + tapScoreBonus) + " points");

        // TODO: Play happy VFX, change color
        // TODO: Add bonus score
        GameManager.Instance?.AddScore(scoreValue + tapScoreBonus);

        // TODO: Maybe float away with animation instead of immediate destroy
        Destroy(gameObject, 0.5f); // Delay destruction for effect
    }

    private void OnMouseDown() // This works for testing in Editor
    {
        if (enemyType == EnemyType.Friendly)
        {
            OnTapped();
        }
    }
}