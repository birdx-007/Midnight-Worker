using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[Serializable]
public enum EnemyState
{
    Sleep = 0,
    FixedPatrol,
    RandomPatrol,
    ChasePlayer
}

public class PoliceControl : MonoBehaviour
{
    public bool canCatchThief = true;
    public float speed = 3;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    public Vector2 _lookDirection;
    public bool isOnIntPoint = true;
    public Vector2Int curIntPoint;
    public Vector2Int nextIntPoint;
    public Moveable2D moveable;
    public EnemyState state;
    public EnemyBehaviorControl behaviorControl;
    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        Initiate();
    }

    void Update()
    {
        switch(state)
        {
            case EnemyState.ChasePlayer:
                break;
            default:
                break;
        }
    }
    private void FixedUpdate()
    {
        Vector2 position = _rigidbody2D.position;
        curIntPoint.Set((int)position.x, (int)position.y);
        // ʹ��Approximately�жϵĻ�x=0��y=0�����ߵ��ж���֪Ϊ�λ�������
        if (Mathf.Abs(position.x - Mathf.Round(position.x)) <= 0.025 && Mathf.Abs(position.y - Mathf.Round(position.y)) <= 0.025)
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
        _animator.SetFloat("speed", _rigidbody2D.velocity.magnitude);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (canCatchThief)
        {
            PlayerControl playerControl = collider.GetComponent<PlayerControl>();
            if (playerControl != null)
            {
                Debug.Log("Policeman catchs thief: " + collider);
                playerControl.GetCaught();
            }
        }
    }
    public void Initiate()
    {
        _lookDirection = Vector2.down;
        curIntPoint = nextIntPoint = new Vector2Int((int)_rigidbody2D.position.x, (int)_rigidbody2D.position.y);
        moveable = new Moveable2D(speed);
        SetStateRandomPatrol();
    }
    public void SetState(EnemyState state, List<Vector2Int> waypoints = null)
    {
        switch (state)
        {
            case EnemyState.Sleep:
                SetStateSleep();
                break;
            case EnemyState.FixedPatrol:
                SetStateFixedPatrol(waypoints);
                break;
            case EnemyState.RandomPatrol:
                SetStateRandomPatrol();
                break;
            case EnemyState.ChasePlayer:
                SetStateChasePlayer();
                break;
            default:
                break;
        }
    }
    public void SetStateSleep()
    {
        state = EnemyState.Sleep;
        behaviorControl = new EnemySleepControl();
        speed = behaviorControl.speed;
        moveable.speed = speed;
    }
    public void SetStateFixedPatrol(List<Vector2Int> waypoints)
    {
        state = EnemyState.FixedPatrol;
        behaviorControl = new EnemyFixedPatrolControl(waypoints);
        speed = behaviorControl.speed;
        moveable.speed = speed;
    }
    public void SetStateRandomPatrol()
    {
        state = EnemyState.RandomPatrol;
        behaviorControl = new EnemyRandomPatrolControl();
        speed = behaviorControl.speed;
        moveable.speed = speed;
    }
    public void SetStateChasePlayer()
    {
        state = EnemyState.ChasePlayer;
        behaviorControl = new EnemyChasePlayerControl();
        speed = behaviorControl.speed;
        moveable.speed = speed;
    }
}
