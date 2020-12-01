using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    #region Private Fields

    [Tooltip("The move speed ")]
    [SerializeField]
    private float moveSpeed = 10f;

    [Tooltip("The distance in the local x-z plane to the target")]
    [SerializeField]
    private float distance = 7.0f;

    [Tooltip("The height we want the camera to be above the target")]
    [SerializeField]
    private float height = 3.0f;

    [Tooltip("Allow the camera to be offseted vertically from the target, for example giving more view of the sceneray and less ground.")]
    [SerializeField]
    private Vector3 centerOffset = Vector3.zero;

    [Tooltip("The Smoothing for the camera to follow the target")]
    [SerializeField]
    private float smoothSpeed = 0.125f;

    private bool smoothEnabled = true;

    // cached transform of the target
    Transform cameraTransform;

    // maintain a flag internally to reconnect if target is lost or camera is switched
    bool isFollowing;

    // Cache for camera offset
    Vector3 cameraOffset = Vector3.zero;

    public int Boundary = 50;
    private int screenWidth;
    private int screenHeight;

    private bool isFollowingByMouse;
    private Vector3 lastMousePosition;

    [SerializeField]
    private float maxScrollDistance = 15;

    [SerializeField]
    private float minScrollDistance = 1;


    #endregion

    #region MonoBehaviour Callbacks

    void LateUpdate()
    {
        // The transform target may not destroy on level load, 
        // so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens
        if (cameraTransform == null && !isFollowing)
        {
            OnStartFollowing();
        }

        // only follow is explicitly declared
        if (isFollowing)
        {
            Follow();
        }
    }

    void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    void Update()
    {
        var mouseScrollDelta = Input.GetAxis("Mouse ScrollWheel");
        height -= mouseScrollDelta;
        height = Mathf.Max(height, minScrollDistance);
        height = Mathf.Min(height, maxScrollDistance);

        if (Input.GetMouseButtonUp(2))
        {
            isFollowingByMouse = false;
            smoothEnabled = true;
        }
        else if (Input.GetMouseButtonDown(2))
        {
            var mousePos = Input.mousePosition;
            lastMousePosition = mousePos;
            isFollowingByMouse = true;
            smoothEnabled = false;
        }

        if (isFollowingByMouse)
        {
            var mousePos = Input.mousePosition;
            var delta = mousePos - lastMousePosition;
            var move = new Vector3(-delta.x, 0, -delta.y);
            Move(move);
            lastMousePosition = mousePos;
            return;
        }

        var moveDirection = GetMouseMoveDirection() + GetInputMoveDirection();

        if (moveDirection != Vector3.zero)
        {
            Move(moveDirection);
        }
    }

    #endregion

    #region Private Methods

    Vector3 GetMouseMoveDirection()
    {
        var moveDirection = Vector3.zero;

        if (Input.mousePosition.y > screenHeight - Boundary)
        {
            moveDirection += Vector3.forward;
        }

        if (Input.mousePosition.y < 0 + Boundary)
        {
            moveDirection += Vector3.back;
        }

        if (Input.mousePosition.x < 0 + Boundary)
        {
            moveDirection += Vector3.left;
        }

        if (Input.mousePosition.x > screenWidth - Boundary)
        {
            moveDirection += Vector3.right;
        }

        return moveDirection;
    }

    Vector3 GetInputMoveDirection()
    {
        var moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection = Vector3.forward;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection = Vector3.back;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection = Vector3.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection = Vector3.right;
        }

        return moveDirection;
    }

    void Move(Vector3 moveDirection)
    {
        transform.position = Vector3.Lerp(
            transform.position,
            transform.position + moveDirection * moveSpeed * Time.deltaTime,
            0.75f
        );
    }

    /// <summary>
    /// Raises the start following event. 
    /// Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
    /// </summary>
    public void OnStartFollowing()
    {
        cameraTransform = Camera.main.transform;
        isFollowing = true;
        // we don't smooth anything, we go straight to the right camera shot
        Cut();
    }

    /// <summary>
    /// Follow the target smoothly
    /// </summary>
    void Follow()
    {
        cameraOffset.z = -distance;
        cameraOffset.y = height;

        var newPosition = smoothEnabled
            ? Vector3.Lerp(cameraTransform.position, transform.position + transform.TransformVector(cameraOffset),
                smoothSpeed * Time.deltaTime)
            : transform.position + transform.TransformVector(cameraOffset);
        cameraTransform.position = newPosition;

        cameraTransform.LookAt(transform.position + centerOffset);
    }


    void Cut()
    {
        cameraOffset.z = -distance;
        cameraOffset.y = height;

        cameraTransform.position = transform.position + transform.TransformVector(cameraOffset);

        cameraTransform.LookAt(transform.position + centerOffset);
    }
    #endregion
}