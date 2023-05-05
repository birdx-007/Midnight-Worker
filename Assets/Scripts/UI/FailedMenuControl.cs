using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailedMenuControl : MonoBehaviour
{
    private Animator _animator;
    void Start()
    {
        _animator = GetComponent<Animator>();
    }
    void Update()
    {
        
    }
    public void ShowFailedMenu()
    {
        _animator.SetBool("isFailed", true);
    }
    public void HideFailedMenu()
    {
        _animator.SetBool("isFailed", false);
    }
}
