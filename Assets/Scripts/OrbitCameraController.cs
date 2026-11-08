using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCameraController : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The point the camera orbits around. Leave empty to auto-create one at the arena center.")]
    public Transform target;

    [Header("Orbit")]
    public float orbitSpeed = 150f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    [Header("Pan")]
    public float panSpeed = 0.5f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;
    public float minDistance = 3f;
    public float maxDistance = 30f;

    [Header("Cursor (optional)")]
    [Tooltip("Open-hand cursor shown on hover. Leave empty to skip custom cursor.")]
    public Texture2D handCursor;
    [Tooltip("Closed-hand cursor shown while dragging. Leave empty to reuse handCursor.")]
    public Texture2D grabCursor;
    public Vector2 cursorHotspot = new Vector2(16, 16);

    private float _yaw;
    private float _pitch;
    private float _distance;

    private Vector3 _lastMousePosition;
    private bool _isOrbiting;
    private bool _isPanning;

    // touch state
    private float _lastPinchDistance;
    private Vector2 _lastTouchMidpoint;

    void Start()
    {
        if (target == null)
        {
            GameObject pivot = new GameObject("CameraPivot (auto)");
            pivot.transform.position = Vector3.zero; // arena center
            target = pivot.transform;
        }

        
        Vector3 offset = transform.position - target.position;
        _distance = Mathf.Clamp(offset.magnitude, minDistance, maxDistance);
        Vector3 dir = offset.normalized;
        _pitch = Mathf.Asin(dir.y) * Mathf.Rad2Deg;
        _yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        UpdateCameraTransform();
        SetCursor(handCursor);
    }

    void Update()
    {
        HandleMouse();
        HandleTouch();
        UpdateCameraTransform();
    }

    private void HandleMouse()
    {
        // Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            _distance -= scroll * zoomSpeed;
            _distance = Mathf.Clamp(_distance, minDistance, maxDistance);
        }

        // Start orbit
        if (Input.GetMouseButtonDown(0))
        {
            _isOrbiting = true;
            _lastMousePosition = Input.mousePosition;
            SetCursor(grabCursor != null ? grabCursor : handCursor);
        }
        if (Input.GetMouseButtonUp(0))
        {
            _isOrbiting = false;
            SetCursor(handCursor);
        }

        // Start pan
        if (Input.GetMouseButtonDown(1))
        {
            _isPanning = true;
            _lastMousePosition = Input.mousePosition;
            SetCursor(grabCursor != null ? grabCursor : handCursor);
        }
        if (Input.GetMouseButtonUp(1))
        {
            _isPanning = false;
            SetCursor(handCursor);
        }

        Vector3 delta = Input.mousePosition - _lastMousePosition;

        if (_isOrbiting)
        {
            _yaw += delta.x * orbitSpeed * 0.02f;
            _pitch -= delta.y * orbitSpeed * 0.02f;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
            _lastMousePosition = Input.mousePosition;
        }
        else if (_isPanning)
        {
            Vector3 right = transform.right;
            Vector3 up = transform.up;
            Vector3 panOffset = (-right * delta.x - up * delta.y) * panSpeed * 0.01f * _distance;
            target.position += panOffset;
            _lastMousePosition = Input.mousePosition;
        }
    }

    private void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Moved)
            {
                _yaw += t.deltaPosition.x * orbitSpeed * 0.02f;
                _pitch -= t.deltaPosition.y * orbitSpeed * 0.02f;
                _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            // Pinch to zoom
            float currentPinchDistance = Vector2.Distance(t0.position, t1.position);
            if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
            {
                _lastPinchDistance = currentPinchDistance;
                _lastTouchMidpoint = (t0.position + t1.position) * 0.5f;
            }
            else
            {
                float pinchDelta = currentPinchDistance - _lastPinchDistance;
                _distance -= pinchDelta * zoomSpeed * 0.01f;
                _distance = Mathf.Clamp(_distance, minDistance, maxDistance);
                _lastPinchDistance = currentPinchDistance;

                // Two-finger drag to pan
                Vector2 midpoint = (t0.position + t1.position) * 0.5f;
                Vector2 midDelta = midpoint - _lastTouchMidpoint;
                Vector3 right = transform.right;
                Vector3 up = transform.up;
                Vector3 panOffset = (-right * midDelta.x - up * midDelta.y) * panSpeed * 0.01f * _distance;
                target.position += panOffset;
                _lastTouchMidpoint = midpoint;
            }
        }
    }

    private void UpdateCameraTransform()
    {
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 position = target.position - (rotation * Vector3.forward * _distance);
        transform.position = position;
        transform.rotation = rotation;
    }

    private void SetCursor(Texture2D cursorTexture)
    {
        if (cursorTexture == null) return;
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }
}
