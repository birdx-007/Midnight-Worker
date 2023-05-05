using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuControl : MonoBehaviour
{
    private Animator _animator;
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }
    public void ShowPauseMenu()
    {
        _animator.SetBool("isPausing", true);
    }
    public void HidePauseMenu()
    {
        _animator.SetBool("isPausing",false);
    }
    public void RecoverTimeScale()
    {
        Time.timeScale = 1f;
    }
}
