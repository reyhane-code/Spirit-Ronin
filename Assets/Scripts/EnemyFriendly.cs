using UnityEngine;

public class EnemyFriendly : Enemy
{
    private bool wasTapped = false;
    private bool isHandled = false;
    public AudioClip tapSound;
    public AudioClip slashSound;
    protected override void Start()
    {
        base.Start();
        enemyType = EnemyType.Friendly;
    }

    public override void OnSliced(Vector3 sliceDirection)
    {
        if (isHandled) return;
        isHandled = true;
        AudioSource.PlayClipAtPoint(slashSound, transform.position);
        GameManager.Instance?.LoseLife();

        Destroy(gameObject);
    }


    public override void OnTapped()
    {
        if (wasTapped) return;
        wasTapped = true;
        AudioSource.PlayClipAtPoint(tapSound, transform.position);
        GameManager.Instance?.AddScore(scoreValue);

        Destroy(gameObject, 0.05f);
    }
}