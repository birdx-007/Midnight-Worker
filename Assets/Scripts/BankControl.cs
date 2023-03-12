using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankControl : MonoBehaviour
{
    public bool isBroken = false;
    public bool isBeingVisited = false;
    public bool isBeingDamaged = false;
    public short totalCoins = 3;
    private short currentCoins;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    void Start()
    {
        currentCoins = totalCoins;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }
    public void OnTriggerStay2D(Collider2D other)
    {
        if (isBroken)
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
                    isBeingVisited = true;
                    playerControl.GetIntoBank(this);
                    //Damage();
                    Debug.Log("Thief visit bank!");
                }
            }
            else
            {
                if (playerControl._lookDirection != delta.normalized)
                {
                    isBeingVisited = false;
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
            isBeingVisited = false;
            playerControl.GetOutofBank();
        }
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
    }
    public void Break()
    {
        Debug.Log("Bank break!");
        isBroken = true;
        _animator.SetBool("isBroken", true);
    }
}
