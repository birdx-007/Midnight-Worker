using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearMenuControl : MonoBehaviour
{
    private Animator _animator;
    void Start()
    {
        _animator = GetComponent<Animator>();
    }
    void Update()
    {

    }
    public void ShowClearMenu()
    {
        _animator.SetBool("isClear", true);
    }
    public void HideClearMenu()
    {
        _animator.SetBool("isClear", false);
    }
}
