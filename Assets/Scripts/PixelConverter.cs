using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PixelConverter : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnPreCapture;
    [HideInInspector] public UnityEvent OnCapture;
    [HideInInspector] public UnityEvent OnPostCapture;
    private bool _isOnProgress;

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.C) && !_isOnProgress)
        {
            _isOnProgress = false;
            OnPreCapture.Invoke();
            OnCapture.Invoke();
            OnPostCapture.Invoke();
            _isOnProgress = true;
        }
    }

    private void OnDestroy()
    {
        OnPreCapture.RemoveAllListeners();
        OnCapture.RemoveAllListeners();
        OnPostCapture.RemoveAllListeners();
    }
}