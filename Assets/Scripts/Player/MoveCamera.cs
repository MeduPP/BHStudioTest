using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveCamera : NetworkBehaviour
{
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private Vector3 _offset= new Vector3(0f, 1f, -3f);
    [SerializeField] private float SensivityY;
    [SerializeField] private float _limitY = 30;
    
    private Vector3 _localPosition;
    private Camera _camera;
    private float _currentRotation;
    private float mouseY;

    private Vector3 _cameraPosition
    {
        get { return _camera.transform.position; }
        set { _camera.transform.position = value; }
    }

    private void Awake()
    {     
        _camera = Camera.main;
    }

    public override void OnStartLocalPlayer()
    {
        if (_camera != null)
        {
            _camera.transform.SetParent(transform);
            _camera.transform.localPosition = _offset;
        }

        _localPosition = _targetPoint.InverseTransformPoint(_cameraPosition);
    }

    public override void OnStopLocalPlayer()
    {
        if (_camera != null)
        {
            _camera.transform.SetParent(null);
            _camera.transform.localPosition = new Vector3(26f, 21f, 0f);
            _camera.transform.localEulerAngles = new Vector3(50f, -90f, 0f);
        }
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        mouseY = Input.GetAxis("Mouse Y");
        CameraRatation(mouseY);
    }

    private void CameraRatation(float mouseY)
    {
        _cameraPosition = _targetPoint.TransformPoint(_localPosition);

        if(mouseY != 0)
        {
            float roatationY = Mathf.Clamp(_currentRotation + mouseY * SensivityY * Time.deltaTime, -_limitY, _limitY);

            if(roatationY != _currentRotation)
            {
                float rotation = roatationY - _currentRotation;
                _camera.transform.RotateAround(_targetPoint.position, _camera.transform.right, rotation);
                _currentRotation = roatationY;
            }
        }
        _camera.transform.LookAt(_targetPoint.position);
        _localPosition = _targetPoint.InverseTransformPoint(_cameraPosition);
    }
}
