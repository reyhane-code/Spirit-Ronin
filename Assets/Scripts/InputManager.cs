using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Tap vs Swipe")]
    public float maxTapTime = 0.2f;
    public float minSwipeDistance = 50f; // pixels

    [Header("References")]
    public Blade blade;
    public Camera mainCamera;

    private Vector2 startPos;
    private float startTime;
    private bool isSwiping;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
#if UNITY_EDITOR
        HandleMouse();
#else
        HandleTouch();
#endif
    }

    // ===================== MOUSE (EDITOR) =====================

    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
            TouchStart(Input.mousePosition);

        if (Input.GetMouseButton(0))
            TouchMove(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
            TouchEnd(Input.mousePosition);
    }

    // ===================== TOUCH (MOBILE) =====================

    void HandleTouch()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                TouchStart(touch.position);
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                TouchMove(touch.position);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                TouchEnd(touch.position);
                break;
        }
    }

    // ===================== CORE LOGIC =====================

    void TouchStart(Vector2 screenPos)
    {
        startPos = screenPos;
        startTime = Time.time;
        isSwiping = false;
    }

    void TouchMove(Vector2 screenPos)
    {
        float distance = Vector2.Distance(screenPos, startPos);

        if (!isSwiping && distance >= minSwipeDistance)
        {
            isSwiping = true;
            blade.StartSlice(ScreenToWorld(screenPos));
        }

        if (isSwiping)
        {
            blade.UpdateSlice(ScreenToWorld(screenPos), Time.deltaTime);
        }
    }

    void TouchEnd(Vector2 screenPos)
    {
        float duration = Time.time - startTime;
        float distance = Vector2.Distance(screenPos, startPos);

        if (!isSwiping && duration <= maxTapTime && distance < minSwipeDistance)
        {
            HandleTap(screenPos);
        }

        blade.EndSlice();
    }

    // ===================== TAP =====================

    void HandleTap(Vector2 screenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null && enemy.CanBeTapped())
            {
                enemy.OnTapped();
            }
        }
    }

    Vector3 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 world = mainCamera.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, mainCamera.nearClipPlane + 5f)
        );
        world.z = 0f;
        return world;
    }
}
