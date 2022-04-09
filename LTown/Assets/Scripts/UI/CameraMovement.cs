using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    enum CameraDirection
    {
        Up,
        Right,
        Left,
        Down,
        None
    }
    
    private int _screenHeight;
    private int _screenWidth;
    private float _screenUpperBound;
    private float _screenRightBound;
    private float _screenLeftBound;
    private float _screenBottonBound;
    [SerializeField] public float cameraSpeed = 5;
    private Camera _camera;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateScreenSize();
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (IsScreenSizeChanged())
        {
            UpdateScreenSize();
        }
        CheckCameraMovement();
    }

    private void UpdateScreenSize()
    {
        _screenHeight = Screen.height;
        _screenWidth = Screen.width;
        _screenUpperBound = _screenHeight * 0.9f;
        _screenBottonBound = _screenHeight * 0.1f;
        _screenRightBound = _screenWidth * 0.9f;
        _screenLeftBound = _screenWidth * 0.1f;
    }

    private bool IsScreenSizeChanged()
    {
        return !(_screenHeight == Screen.height && _screenWidth == Screen.width);
    }

    private CameraDirection CameraBound()
    {
        Vector2 mousePos = Input.mousePosition;
        if (mousePos.x >= _screenRightBound)
        {
            return CameraDirection.Right;
        }
        
        if (mousePos.x <= _screenLeftBound)
        {
            return CameraDirection.Left;
        }

        if (mousePos.y >= _screenUpperBound)
        {
            return CameraDirection.Up;
        }

        if (mousePos.y <= _screenBottonBound)
        {
            return CameraDirection.Down;
        }

        return CameraDirection.None;
    }

    //TODO scroll, diagonal movement
    private void CheckCameraMovement()
    {
        float speed = cameraSpeed * Time.deltaTime;
        switch (CameraBound())
        {
            case CameraDirection.Up:
                _camera.transform.Translate(new Vector3(0, speed, 0));
                break;
            case CameraDirection.Right:
                _camera.transform.Translate(new Vector3(speed, 0, 0));
                break;
            case CameraDirection.Left:
                _camera.transform.Translate(new Vector3(-speed, 0, 0));
                break;
            case CameraDirection.Down:
                _camera.transform.Translate(new Vector3(0, -speed, 0));
                break;
            case CameraDirection.None:
                break;
        }
    }
}
