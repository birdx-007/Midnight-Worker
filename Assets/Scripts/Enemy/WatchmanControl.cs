using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class WatchmanControl : EnemyControl
{
    void Awake()
    {
        Initiate();
        enemyAI = new EnemyAI_FixedPatrol();
        speed = 1.5f;
        UpdateMoveable();
        _animator.SetBool("isWatchman", true);
    }
    public override void UpdateOnIntPoint()
    {
        enemyAI.GetNextIntPoint(ref nextIntPoint, curIntPoint);
    }
}
