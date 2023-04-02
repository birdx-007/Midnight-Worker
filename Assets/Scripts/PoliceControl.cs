using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class PoliceControl : MonoBehaviour
{
    public float speed = 3;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    public Vector2 _lookDirection;
    public bool isOnIntPoint = true;
    public Vector2Int curIntPoint;
    public Vector2Int nextIntPoint;
    public Moveable2D moveable;
    public EnemyState state;
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _lookDirection = Vector2.down;
        curIntPoint = nextIntPoint = new Vector2Int((int)_rigidbody2D.position.x, (int)_rigidbody2D.position.y);
        moveable = new Moveable2D(speed);
        state = EnemyState.Search;
    }

    void Update()
    {
        switch(state)
        {
            case EnemyState.Search:
                break;
            default:
                break;
        }
    }
    private void FixedUpdate()
    {
        Vector2 position = _rigidbody2D.position;
        curIntPoint.Set((int)position.x, (int)position.y);
        // 使用Approximately判断的话x=0和y=0两条线的判定不知为何会有问题
        if (Mathf.Abs(position.x - Mathf.Round(position.x)) <= 0.05 && Mathf.Abs(position.y - Mathf.Round(position.y)) <= 0.05)
        {
            isOnIntPoint = true;
        }
        else
        {
            isOnIntPoint = false;
            _lookDirection = nextIntPoint - position;
            _lookDirection.Normalize();
        }
        moveable.MoveTo(_rigidbody2D, nextIntPoint);
        // animation
        _animator.SetFloat("lookX", _lookDirection.x);
        _animator.SetFloat("lookY", _lookDirection.y);
        _animator.SetFloat("speed", speed);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerControl playerControl = collider.GetComponent<PlayerControl>();
        if (playerControl != null)
        {
            Debug.Log("Policeman catchs thief: " + collider);
            playerControl.GetCaught();
        }
    }
}
