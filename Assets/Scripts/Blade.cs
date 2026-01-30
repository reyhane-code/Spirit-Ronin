using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Blade : MonoBehaviour
{
    public float minSliceVelocity = 0.5f;
    public TrailRenderer trail;
    private Vector3 lastPosition;
    private bool slicing;
    private Collider bladeCollider;
    private Vector3 _lastPos;
    private Vector3 _lastValidSliceDir;

    private void Awake()
    {
        bladeCollider = GetComponent<Collider>();
        bladeCollider.isTrigger = true;
        bladeCollider.enabled = false;

        if (trail != null)
            trail.emitting = false;
    }

    public void StartSlice(Vector3 startWorldPos)
    {
        slicing = true;
        lastPosition = startWorldPos;
        _lastPos = startWorldPos;
        _lastValidSliceDir = Vector3.zero;
        transform.position = startWorldPos;

        if (trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }
    }

    public void UpdateSlice(Vector3 worldPos, float deltaTime)
    {
        if (!slicing) return;

        Vector3 delta = worldPos - _lastPos;
        if (delta.sqrMagnitude > 0.0001f)
            _lastValidSliceDir = delta.normalized;

        _lastPos = worldPos;

        float velocity = (worldPos - lastPosition).magnitude / deltaTime;
        bladeCollider.enabled = velocity > minSliceVelocity;

        transform.position = worldPos;
        lastPosition = worldPos;
    }

    public void EndSlice()
    {
        slicing = false;
        if (trail != null)
            trail.emitting = false;
        bladeCollider.enabled = false;
        _lastValidSliceDir = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            Vector3 sliceDir = _lastValidSliceDir;
            sliceDir.z = 0f;
            if (sliceDir.sqrMagnitude < 0.0001f)
                sliceDir = (transform.position - lastPosition);
            sliceDir.z = 0f;
            sliceDir.Normalize();

            enemy.OnSliced(sliceDir);
        }
    }

    public Vector3 GetLastSliceDirection()
    {
        return _lastValidSliceDir;
    }
}
