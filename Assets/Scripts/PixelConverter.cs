using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PixelConverter : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnPreCapture = new();
    [HideInInspector] public UnityEvent OnCapture = new();
    [HideInInspector] public UnityEvent OnPostCapture = new();
    public bool IsOnProgress;
    public bool IsCaptureCompleted;
    public bool IsConvertCompleted;
    private void Update()
    {
        HandleInput();
        if (IsCaptureCompleted)
        {
            OnPostCapture.Invoke();
            IsOnProgress = false;
            IsCaptureCompleted = false;
        }
        else if (IsOnProgress)
        {
            OnCapture.Invoke();
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.P) && !IsOnProgress)
        {
            OnPreCapture.Invoke();
            IsOnProgress = true;
        }
    }

    private void OnDestroy()
    {
        OnPreCapture.RemoveAllListeners();
        OnCapture.RemoveAllListeners();
        OnPostCapture.RemoveAllListeners();
    }
}