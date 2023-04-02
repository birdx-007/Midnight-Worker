using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPointControl : MonoBehaviour
{
    public Text key;
    private Animator _animator;
    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetBool("isShowing", true);
    }

    void Update()
    {
        
    }
    public void Initiate(string k)
    {
        key.text = k;
    }
    public void GetHit()
    {
        _animator.SetBool("isShowing", false);
    }
    public void Disappear()
    {
        Destroy(gameObject);
    }
}
