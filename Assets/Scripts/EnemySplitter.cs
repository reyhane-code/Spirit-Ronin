using UnityEngine;

public class SplitterEnemy : Enemy
{
    public GameObject splitChildPrefab;
    public int splitCount = 2;
    public float splitForce = 2f;
    private bool hasSplit = false;
    public AudioClip hitSound;


    public override void OnSliced(Vector3 sliceDirection)
    {
        if (hasSplit)
        {
            base.OnSliced(sliceDirection);
            return;
        }

        hasSplit = true;
        AudioSource.PlayClipAtPoint(hitSound, transform.position);
        SpawnChildren(sliceDirection);
        Destroy(gameObject);
    }

    void SpawnChildren(Vector3 sliceDir)
    {
        for (int i = 0; i < splitCount; i++)
        {
            Vector3 offset = Vector3.right * (i == 0 ? -0.4f : 0.4f);

            GameObject child = Instantiate(
                splitChildPrefab,
                transform.position + offset,
                Quaternion.identity,
                transform.parent
            );

            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce((offset + sliceDir.normalized) * splitForce, ForceMode.Impulse);
                rb.isKinematic = true;
            }
        }
    }
}
