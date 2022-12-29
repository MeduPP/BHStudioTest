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

    [Header("Player dash")]
    [SyncVar]
    [SerializeField] private float _dashSpeed;
    [SyncVar]
    [SerializeField] private float _dashDistance;

    private Rigidbody _rb;
    
    private float _mouseMoveDirection;
    private Vector3 _moveDirection;

    private bool _duringDash = false;
    public Coroutine DashCoroutine;

    public void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if (isLocalPlayer && isClient && isOwned)
        {
            _moveDirection = GetMoveDirection();
            _mouseMoveDirection = GetMouseMove();

            if (!_duringDash)
                RotatePlayer();

            if (GetDashMouse() && !_duringDash)
            {
                StartDash();
            }
        }
    }


    public void FixedUpdate()
    {
        if (isLocalPlayer && isClient && isOwned)
        {
            if (_duringDash)
            {
                //Do nothing
            }
            else
            {
                MovePlayer(_moveDirection);
            }
        }
    }

    #region Move Player

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

    public void StartDash()
    {
        DashCoroutine = StartCoroutine(DoDash());
    }

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
