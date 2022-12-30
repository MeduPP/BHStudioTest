using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CursorBehaviour : MonoBehaviour
{
    private bool _lockCursor = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.Confined;
        if (Input.GetMouseButtonDown(0))
            _lockCursor = true;
    }
    private void OnDrawGizmos()
    {
        if (_lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            _lockCursor = false;
        }
    }

}
