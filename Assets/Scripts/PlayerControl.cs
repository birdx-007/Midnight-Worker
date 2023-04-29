using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerControl : MonoBehaviour
{
    public bool noStopMode = false;
    public bool isCaught = false;
    public bool hasWon = false;
    public float speed = 4;
    public bool isInputingMotion;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    public Vector2 _lookDirection;
    private Vector2 _currentInput;
    private float _inputX;
    private float _inputY;
    public bool isOnIntPoint = true;
    private float _roundIntX;
    private float _roundIntY;
    private float _nextIntX;
    private float _nextIntY;
    public BankControl bankFacing = null;
    public BankControl bankVisiting = null;
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        isInputingMotion = false;
        _lookDirection = Vector2.zero;
    }
    void Update()
    {
        // input
        if (!isCaught && !hasWon)
        {
            _inputX = Input.GetAxisRaw("Horizontal");
            _inputY = Input.GetAxisRaw("Vertical");
        }
        else
        {
            _inputX = 0f;
            _inputY = 0f;
        }
        Vector2 inputVector = new Vector2(_inputX, _inputY);
        inputVector.Normalize();
        _currentInput = inputVector;
        _animator.SetFloat("speed", _currentInput.magnitude);
        if (Mathf.Approximately(_inputX, 0.0f) && Mathf.Approximately(_inputY, 0.0f))
        {
            isInputingMotion = false;
        }
        else
        {
            isInputingMotion = true;
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = _rigidbody2D.position;
        // ʹ��Approximately�жϵĻ�x=0��y=0�����ߵ��ж���֪Ϊ�λ�������
        if (Mathf.Abs(position.x - Mathf.Round(position.x)) <= 0.05 && Mathf.Abs(position.y - Mathf.Round(position.y)) <= 0.05)
        {
            // mark int point
            //Debug.Log("Int point" + position);
            isOnIntPoint = true;
            _roundIntX = Mathf.Round(position.x);
            _roundIntY = Mathf.Round(position.y);
            if (isInputingMotion)
            {
                // lookDirection
                if(_currentInput.y>0.0f)
                {
                    _lookDirection.Set(0, 1);
                }
                if(_currentInput.y<0.0f)
                {
                    _lookDirection.Set(0, -1);
                }
                if(_currentInput.x>0.0f)
                {
                    _lookDirection.Set(1, 0);
                }
                if(_currentInput.x<0.0f)
                {
                    _lookDirection.Set(-1, 0);
                }
                // calculate next int point
                _nextIntX = _roundIntX + _lookDirection.x;
                _nextIntY = _roundIntY + _lookDirection.y;
            }
            else
            {
                position.x = _roundIntX;
                position.y = _roundIntY;
                _rigidbody2D.MovePosition(position);// ����λ��
                return;
            }
        }
        else
        {
            isOnIntPoint = false;
        }
        // move
        if (isInputingMotion || noStopMode)
        {
            position += _lookDirection * speed * Time.deltaTime;
            if(Mathf.Approximately(_lookDirection.x, 0.0f))
            {
                position.x = Mathf.Round(position.x);
            }
            if (Mathf.Approximately(_lookDirection.y, 0.0f))
            {
                position.y = Mathf.Round(position.y);
            }
            _rigidbody2D.MovePosition(position);
        }
        else // stop
        {
            float distance=0.0f;
            if (!Mathf.Approximately(position.x, _nextIntX))
            {
                distance = Mathf.Abs(_nextIntX - position.x);
            }
            if (!Mathf.Approximately(position.y, _nextIntY))
            {
                distance = Mathf.Abs(_nextIntY - position.y);
            }
            position.x = Mathf.MoveTowards(position.x, _nextIntX, Mathf.Min(speed * Time.deltaTime, distance));
            position.y = Mathf.MoveTowards(position.y, _nextIntY, Mathf.Min(speed * Time.deltaTime, distance));
            _rigidbody2D.MovePosition(position);
        }
        // animation
        _animator.SetFloat("lookX", _lookDirection.x);
        _animator.SetFloat("lookY", _lookDirection.y);
    }
    public void FaceBank(BankControl bank)
    {
        bankFacing = bank;
    }
    public void UnfaceBank()
    {
        bankFacing = null;
    }
    public void GetIntoBank(BankControl bank)
    {
        bankVisiting = bank;
    }
    public void GetOutofBank()
    {
        bankVisiting = null;
    }
    public void GetCaught()
    {
        if (!isCaught)
        {
            isCaught = true;
            _animator.SetTrigger("caught");
            GetOutofBank();
        }
    }
    public void Win()
    {
        if(!hasWon)
        {
            hasWon = true;
        }
    }
}
