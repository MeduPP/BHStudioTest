using Mirror;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private float SensivityY;
    [SerializeField] private float _limitY = 30;
    
    private Vector3 _localPosition;
    private float _currentRotation;
    private float mouseY;

    private Vector3 _cameraPosition
    {
        get { return _camera.transform.position; }
        set { _camera.transform.position = value; }
    }

    void Start()
    {
        _localPosition = _targetPoint.InverseTransformPoint(_cameraPosition);
    }

    private void Update()
    {
        mouseY = Input.GetAxis("Mouse Y");
    }

    private void FixedUpdate()
    {
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
