using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuControl : MonoBehaviour
{
    private Animator _animator;
    void Start()
    {
        _animator = GetComponent<Animator>();
    }
    void Update()
    {
        
    }
    public void ShowStartMenu()
    {
        _animator.SetBool("hasStarted", true);
    }
    public void HideStartMenu()
    {
        _animator.SetBool("hasStarted", false);
    }
}
