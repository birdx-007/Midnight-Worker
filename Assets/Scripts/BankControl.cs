using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankControl : MonoBehaviour
{
    public bool isFaced = false;
    public bool isBroken = false;
    public bool isBeingVisited = false;
    public bool isBeingDamaged = false;
    public bool isLocked = false;
    static public float maxLockedTime = 3f;
    private float _leftLockedTime;
    public int totalCoins = 3;
    private int currentCoins;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private Collider2D _trigger;
    void Start()
    {
        _leftLockedTime = maxLockedTime;
        currentCoins = totalCoins;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _trigger = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        if (isLocked)
        {
            _leftLockedTime -= Time.deltaTime;
            if(_leftLockedTime <= 0)
            {
                Unlock();
            }
        }
    }
    public void OnTriggerStay2D(Collider2D other)
    {
        if (isBroken || isLocked)
        {
            return;
        }
        PlayerControl playerControl = other.GetComponent<PlayerControl>();
        if (playerControl != null)
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            Vector2 delta = _rigidbody2D.position - rb.position;
            if (!isBeingVisited)
            {
                if (playerControl._lookDirection == delta.normalized)
                {
                    isFaced = true;
                    playerControl.FaceBank(this);
                }
            }
            else
            {
                if (playerControl._lookDirection != delta.normalized)
                {
                    isFaced = false;
                    isBeingVisited = false;
                    playerControl.UnfaceBank();
                    playerControl.GetOutofBank();
                }
            }
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        PlayerControl playerControl = other.GetComponent<PlayerControl>();
        if (playerControl != null)
        {
            isFaced = false;
            isBeingVisited = false;
            playerControl.UnfaceBank();
            playerControl.GetOutofBank();
        }
    }
    public void Lock()
    {
        isLocked = true;
        isFaced = false;
        isBeingVisited = false;
        _leftLockedTime = maxLockedTime;
        _animator.SetBool("isLocked", true);
        _trigger.enabled = false;
    }
    public void Unlock()
    {
        isLocked = false;
        _animator.SetBool("isLocked", false);
        _trigger.enabled = true;
    }
    public void Visited()
    {
        isBeingVisited = true;
        Debug.Log("Thief visit bank!");
    }
    public void Damage()
    {
        if(!isBroken && !isBeingDamaged)
        {
            Debug.Log("Bank damage!");
            isBeingDamaged = true;
            _animator.SetBool("isDamaging", true);
            currentCoins = (short)Math.Clamp(currentCoins - 1, 0, totalCoins);
        }
    }
    public void StopDamage()
    {
        isBeingDamaged = false;
        _animator.SetBool("isDamaging", false);
        if (currentCoins == 0)
        {
            Break();
        }
        else
        {
            Lock();
        }
    }
    public void Break()
    {
        Debug.Log("Bank break!");
        isBroken = true;
        _animator.SetBool("isBroken", true);
        _trigger.enabled = false;
    }
}
