using UnityEngine;
using Mirror;

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
    
    private Rigidbody _rb;
    private Vector3 _moveDirection;
    private float _mouseMoveDirection;

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
        }

    }

    public void FixedUpdate()
    {
        if (isServer)
        {

        }
        if (isClient && isLocalPlayer)
        {
            CmdMovePlayer(_moveDirection);
            MovePlayer(_moveDirection);
        }
    }

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
}
