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
    // public bool IsPlayerTwo;
    [SerializeField] private LayerMask _hitLayerMask;
    public Int64 userID;
    private Player _player;
    public bool IsNetworkBlade = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
        // sliceCollider = GetComponent<Collider>();
        sliceTrail = GetComponentInChildren<TrailRenderer>();
        _player = GetComponentInParent<Player>();
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

    private void Update()
    {
        if (!IsNetworkBlade)
        {
            Int64 userID = KinectHandPositionManager.Instance.GetUserIDBySide(_player.playerSide);
            if (IsRightHand)
            {
                screenNormalPos = KinectHandPositionManager.Instance.GetRightHandScreenPos(userID);
            }
            else
            {
                screenNormalPos = KinectHandPositionManager.Instance.GetLeftHandScreenPos(userID);
            }

            if (_player.playerSide == Player.PlayerSide.RIGHT)
            {
                screenNormalPos = new Vector3(screenNormalPos.x / 2 + 0.5f, screenNormalPos.y, screenNormalPos.z);
            }
            else
            {
                screenNormalPos = new Vector3(screenNormalPos.x / 2, screenNormalPos.y, screenNormalPos.z);
            }

            _bladePos.x = _startPos.x + screenNormalPos.x * _boundary.x;
            _bladePos.y = _startPos.y + screenNormalPos.y * _boundary.y;
            // screenPixelPos.z = 0;
        }

        // _bladePos.x = _startPos.x + screenNormalPos.x * _boundary.x;
        // _bladePos.y = _startPos.y + screenNormalPos.y * _boundary.y;

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
        ContinueSlice();
        if (slicing)
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
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }

    private void StartSlice()
    {
        transform.position = _startPos;
        slicing = true;
        sliceTrail.enabled = true;
        sliceTrail.Clear();
    }

    private void StopSlice()
    {
        slicing = false;
        sliceTrail.enabled = false;
    }

    private void ContinueSlice()
    {
        Vector3 newPosition = _bladePos;
        direction = newPosition - transform.position;
        float velocity = direction.magnitude / Time.deltaTime;
        slicing = velocity > minSliceVelocity;
        _rigidbody.MovePosition(newPosition);
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

    public void SetBladePos(Vector2 pos)
    {
        _bladePos.x = _startPos.x + pos.x * _boundary.x;
        _bladePos.y = _startPos.y + pos.y * _boundary.y;
    }
}