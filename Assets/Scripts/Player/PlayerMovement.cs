using UnityEngine;
using Mirror;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    [SyncVar]
    [SerializeField]
    private float _moveSpeed;

    [SyncVar]
    [SerializeField]
    public float _rotationSpeed;

    [SyncVar]
    [SerializeField]
    float _rotationSmoothTime;

    [Header("Player dash")]
    [SyncVar]
    [SerializeField] private float _dashSpeed;
    [SyncVar]
    [SerializeField] private float _dashDistance;

    private Rigidbody _rb;
    private Vector3 _moveDirection;
    private float _mouseMoveDirection;
    private bool _duringDash = false;
    public Coroutine DashCoroutine;

    public static Action<GameObject, GameObject> PlayerHit;

    public void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if (isServer)
        {

        }
        if (isLocalPlayer && isClient && isOwned)
        {
            _moveDirection = GetMoveDirection();
            _mouseMoveDirection = GetMouseMove();

            if (!_duringDash)
                RotatePlayer();

            if (GetDashMouse() && !_duringDash)
            {
                //CmdStartDash();
                StartDash();
            }
        }
    }


    public void FixedUpdate()
    {
        if (isServer)
        {

        }
        if (isLocalPlayer && isClient && isOwned)
        {
            if (_duringDash)
            {

            }
            else
            {
                //CmdMovePlayer(_moveDirection);
                MovePlayer(_moveDirection);
            }
        }
    }

    #region Move Player
    //[Command]
    //private void CmdMovePlayer(Vector3 moveDirection)
    //{
    //    _rb.velocity = transform.forward * moveDirection.z * _moveSpeed * Time.deltaTime;

    //    if (_mouseMoveDirection != 0)
    //    {
    //        transform.Rotate(new Vector3(0, _mouseMoveDirection * _moveSpeed * Time.deltaTime, 0));
    //    }
    //}

    private void MovePlayer(Vector3 moveDirection)
    {
        _rb.velocity = transform.forward * moveDirection.z * _moveSpeed * Time.deltaTime;
    }

    private void RotatePlayer()
    {
        if (_mouseMoveDirection != 0)
        {
            transform.Rotate(new Vector3(0, _mouseMoveDirection * _rotationSpeed * Time.deltaTime, 0));
        }
    }


    //[Command]
    //public void CmdStartDash()
    //{
    //    DashCoroutine = StartCoroutine(DoDash());
    //}

    public void StartDash()
    {
        DashCoroutine = StartCoroutine(DoDash());
    }

    //[Command]
    //public void CmdStopDash()
    //{
    //    StopCoroutine(DashCoroutine);
    //    _duringDash = false;
    //}

    public void StopDash()
    {
        StopCoroutine(DashCoroutine);
        _duringDash = false;
    }

    IEnumerator DoDash()
    {
        _duringDash = true;
        Vector3 startpoint = transform.position;

        while (Vector3.Distance(startpoint, transform.position) < _dashDistance)
        {
            _rb.velocity = transform.forward * _dashSpeed * Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        _duringDash = false;
        DashCoroutine = null;
    }
    #endregion


    #region Input handlers
    private float GetMouseMove()
    {
        float x = Input.GetAxis("Mouse X");
        return x;
    }

    private Vector3 GetMoveDirection()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        return new Vector3(x, 0, z);
    }

    private bool GetDashMouse()
    {
        return Input.GetMouseButtonDown(0);
    }
    #endregion
}
