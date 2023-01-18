using System;
using UnityEngine;

public class Blade : MonoBehaviour
{
    public Vector3 direction { get; private set; }

    private Camera mainCamera;

    private Collider sliceCollider;
    private TrailRenderer sliceTrail;

    public float sliceForce = 5f;
    public float minSliceVelocity = 0.01f;

    private Rigidbody _rigidbody;

    // Kinect Field
    // normalized and pixel position of the cursor
    private Vector3 screenNormalPos = Vector3.zero;
    private Vector3 screenPixelPos = Vector3.zero;
    private InteractionManager interactionManager;
    private KinectManager _kinectManager;
    public Camera screenCamera;
    public int playerIndex = 0;
    public bool leftHandInteraction = true;
    public bool rightHandInteraction = true;
    private Vector3 _bladePos;
    [SerializeField] private Vector2 _boundary;
    private Vector3 _startPos;
    private bool slicing;
    public bool IsRightHand;
    public float playerDetctionThreshold = 2f;
    private float detectionCountDown;
    [SerializeField] private bool _inactve;
    [SerializeField] private LayerMask _hitLayerMask;

    private void Awake()
    {
        mainCamera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
        // sliceCollider = GetComponent<Collider>();
        sliceTrail = GetComponentInChildren<TrailRenderer>();
        detectionCountDown = playerDetctionThreshold;
    }

    private void Start()
    {
        screenCamera = Camera.main;
        _kinectManager = FindObjectOfType<KinectManager>();
        interactionManager = GetInteractionManager();
        _boundary = GameManager.Instance.GetBoundary();
        _startPos = GameManager.Instance.GetBladeStartPosition();
        StartSlice();
    }

    private void OnEnable()
    {
        StopSlice();
    }

    private void OnDisable()
    {
        StopSlice();
    }

    private void Update()
    {
        // screenNormalPos = interactionManager.IsLeftHandPrimary() ? interactionManager.GetLeftHandScreenPos() : interactionManager.GetRightHandScreenPos();
        if (IsRightHand)
            screenNormalPos = interactionManager.GetRightHandScreenPos();
        else
        {
            screenNormalPos = interactionManager.GetLeftHandScreenPos();
        }

        // screenPixelPos.x = (int)(screenNormalPos.x * (screenCamera ? screenCamera.pixelWidth : Screen.width));
        // screenPixelPos.y = (int)(screenNormalPos.y * (screenCamera ? screenCamera.pixelHeight : Screen.height));
        // screenPixelPos.z = 0;
        _bladePos.x = _startPos.x + screenNormalPos.x * _boundary.x;
        _bladePos.y = _startPos.y + screenNormalPos.y * _boundary.y;
        if (_bladePos == transform.position && GameManager.Instance.Playing)
        {
            detectionCountDown -= Time.unscaledDeltaTime;
            if (detectionCountDown < 0 && !_inactve)
            {
                _inactve = true;
                GameManager.Instance.InactiveBlades++;
            }

            // GameManager.Instance.EndGame();
        }
        else
        {
            detectionCountDown = playerDetctionThreshold;
            GameManager.Instance.InactiveBlades--;
            _inactve = false;
        }

        ContinueSlice();
        // if (Input.GetMouseButtonDown(0))
        // {
        //     StartSlice();
        // }
        // else if (Input.GetMouseButtonUp(0))
        // {
        //     StopSlice();
        // }
        // else if (slicing)
        // {
        //     ContinueSlice();
        // }
    }

    private void FixedUpdate()
    {
        RaycastHit[] raycastHit = Physics.SphereCastAll(transform.position, 2f, transform.forward, .2f, _hitLayerMask);
        foreach (var hit in raycastHit)
        {
            var collider = hit.collider;
            var fruit = collider.GetComponent<Fruit>();
            if (fruit != null)
            {
                fruit.Slice(direction, transform.position, sliceForce);
            }
            else
            {
                collider.GetComponent<Bomb>().Explode();
            }
            
        }
        // Physics.SphereCast(transform.position, 2f,out raycastHit);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }

    private void StartSlice()
    {
        // Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // position.z = 0f;
        // transform.position = position;

        screenNormalPos = interactionManager.GetRightHandScreenPos();
        // screenPixelPos.x = (int)(screenNormalPos.x * (screenCamera ? screenCamera.pixelWidth : Screen.width));
        // screenPixelPos.y = (int)(screenNormalPos.y * (screenCamera ? screenCamera.pixelHeight : Screen.height));
        // screenPixelPos.z = 0;
        // transform.position = screenPixelPos;
        transform.position = _startPos;
        slicing = true;
        // sliceCollider.enabled = true;
        sliceTrail.enabled = true;
        sliceTrail.Clear();
    }

    private void StopSlice()
    {
        slicing = false;
        // sliceCollider.enabled = false;
        sliceTrail.enabled = false;
    }

    private void ContinueSlice()
    {
        // Vector3 newPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // newPosition.z = 0f;
        Vector3 newPosition = _bladePos;
        direction = newPosition - transform.position;
        float velocity = direction.magnitude / Time.deltaTime;
        // sliceCollider.enabled = velocity > minSliceVelocity;
        // _rigidbody.position = newPosition;
        _rigidbody.MovePosition(newPosition);
        // transform.position = newPosition;
    }

    private InteractionManager GetInteractionManager()
    {
        // find the proper interaction manager
        MonoBehaviour[] monoScripts = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];

        foreach (MonoBehaviour monoScript in monoScripts)
        {
            if ((monoScript is InteractionManager) && monoScript.enabled)
            {
                InteractionManager manager = (InteractionManager)monoScript;

                if (manager.playerIndex == playerIndex && manager.leftHandInteraction == leftHandInteraction && manager.rightHandInteraction == rightHandInteraction)
                {
                    return manager;
                }
            }
        }

        // not found
        return null;
    }
}