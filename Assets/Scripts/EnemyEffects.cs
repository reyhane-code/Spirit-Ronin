using UnityEngine;

public class EnemyEffects : MonoBehaviour
{
    public GameObject sliceEffectPrefab;
    
    public void PlaySliceEffect(Vector3 position, Vector3 direction)
    {
        if (sliceEffectPrefab != null)
        {
            GameObject effect = Instantiate(sliceEffectPrefab, position, Quaternion.identity);
            
            // Rotate effect to face slice direction
            if (direction != Vector3.zero)
            {
                effect.transform.rotation = Quaternion.LookRotation(direction);
            }
            
            // Auto-destroy after 2 seconds
            Destroy(effect, 2f);
        }
    }
}