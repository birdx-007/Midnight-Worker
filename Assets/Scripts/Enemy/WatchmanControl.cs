using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class WatchmanControl : EnemyControl
{
    // enemyAI will be set later, in builder's CreateEnemy function.
    void Awake()
    {
        Initiate();
        enemyAI = new EnemyAI_FixedPatrol();
        standardSpeed = speed = 2f;
        maxSpeed = 3f;
        UpdateMoveable();
        _animator.SetBool("isWatchman", true);
    }
    public override void UpdateOnIntPoint()
    {
        enemyAI.GetNextIntPoint(ref nextIntPoint, curIntPoint);
    }
}
