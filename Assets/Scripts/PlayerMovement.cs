using UnityEngine;
using Mirror;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    [SyncVar]
    [SerializeField]
    private float _moveSpeed;
    [SyncVar]
    [SerializeField]
    public float _healthPoint;
    [SerializeField]
    public float _rotationSpeed;
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
    private Coroutine _dashCoroutine;

    public void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if (isServer)
        {

        }
        if (isClient && isLocalPlayer)
        {
            _moveDirection = GetMoveDirection();
            _mouseMoveDirection = GetMouseMove();

            if (GetDashMouse() && !_duringDash)
            {
                Debug.Log("StartDash");
                CmdStartDash();

            }
        }
    }

    public void FixedUpdate()
    {
        if (isServer)
        {

        }
        if (isClient && isLocalPlayer)
        {
            if (_duringDash)
            {

            }
            else
            {
                CmdMovePlayer(_moveDirection);
                MovePlayer(_moveDirection);
            }
        }
    }
    #region Move Player
    [Command]
    private void CmdMovePlayer(Vector3 moveDirection)
    {
        _rb.velocity = transform.forward * moveDirection.z * _moveSpeed * Time.deltaTime;

        if (_mouseMoveDirection != 0)
        {
            transform.Rotate(new Vector3(0, _mouseMoveDirection * _moveSpeed * Time.deltaTime, 0));
        }
    }

    private void MovePlayer(Vector3 moveDirection)
    {
        _rb.velocity = transform.forward * moveDirection.z * _moveSpeed * Time.deltaTime;

        if (_mouseMoveDirection != 0)
        {
            transform.Rotate(new Vector3(0, _mouseMoveDirection * _moveSpeed * Time.deltaTime, 0));
        }
    }
    #endregion

    #region Do dash
    [Command]
    public void CmdStartDash()
    {
        _dashCoroutine = StartCoroutine(DoDash());
    }

    public void StartDash()
    {
        _dashCoroutine = StartCoroutine(DoDash());
    }

    [Command]
    public void CmdStopDash()
    {
        StopCoroutine(_dashCoroutine);
        _duringDash = false;
    }

    public void StopDash()
    {
        StopCoroutine(_dashCoroutine);
        _duringDash = false;
    }

    IEnumerator DoDash()
    {
        Debug.Log("Inside Dash");
        _duringDash = true;
        Vector3 startpoint = transform.position;

        while (Vector3.Distance(startpoint, transform.position) < _dashDistance)
        {
            _rb.velocity = transform.forward * _dashSpeed * Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        _duringDash = false;
    }
    #endregion

    private void OnCollisionStay(Collision collision)
    {
        if (_dashCoroutine != null && collision.gameObject.CompareTag("Obstacle"))
        {
            CmdStopDash();
            StopDash();
        }
    }

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
