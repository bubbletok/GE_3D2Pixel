using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private PixelConverter pixelConverter;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        pixelConverter.OnPreCapture.AddListener(PlayAnimation);
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayAnimation();
        }
    }*/

    void PlayAnimation()
    {
        _animator.SetBool("OnAttack",true);
    }

    void StopAnimation()
    {
        _animator.SetBool("OnAttack",false);
        pixelConverter.IsCaptureCompleted = true;
    }
    
    bool AnimatorIsPlaying()
    {
        if (_animator is null) return false;
        
        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f;
    }
}
