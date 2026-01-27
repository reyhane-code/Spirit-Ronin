using UnityEngine;
using System.Collections;

public class Blade : MonoBehaviour
{
    [Header("Settings")]
    public float minSliceVelocity = 0.01f;

    [Header("References")]
    public TrailRenderer trail;
    public Collider bladeCollider;

    private Vector3 lastPosition;
    private bool slicing;
    private Coroutine startRoutine;

    private void Awake()
    {
        trail.emitting = false;
        bladeCollider.enabled = false;
    }

    // ===================== CALLED BY InputManager =====================

    public void StartSlice(Vector3 startWorldPos)
    {
        slicing = true;

        if (startRoutine != null)
            StopCoroutine(startRoutine);

        startRoutine = StartCoroutine(StartSliceRoutine(startWorldPos));
    }

    IEnumerator StartSliceRoutine(Vector3 startWorldPos)
    {
        // 1️⃣ قطع کامل
        trail.emitting = false;
        bladeCollider.enabled = false;

        // 2️⃣ تلپورت
        transform.position = startWorldPos;
        lastPosition = startWorldPos;

        // 3️⃣ پاکسازی
        trail.Clear();

        // 4️⃣ صبر یک فریم (کلید حل مشکل 👇)
        yield return null;

        // 5️⃣ شروع تریل جدید
        trail.emitting = true;
    }

    public void UpdateSlice(Vector3 worldPos, float deltaTime)
    {
        if (!slicing) return;

        transform.position = worldPos;

        float velocity = (worldPos - lastPosition).magnitude / deltaTime;
        bladeCollider.enabled = velocity > minSliceVelocity;

        lastPosition = worldPos;
    }

    public void EndSlice()
    {
        slicing = false;

        if (startRoutine != null)
            StopCoroutine(startRoutine);

        trail.emitting = false;
        bladeCollider.enabled = false;
    }
}
