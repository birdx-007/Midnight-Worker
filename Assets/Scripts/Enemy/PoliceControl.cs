using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class PoliceControl : EnemyControl
{
    // enemyAI will be set later, in builder's CreateEnemy function.
    void Awake()
    {
        Initiate();
        enemyAI = new EnemyAI_ChasePlayer();
        standardSpeed = speed = 2.5f;
        maxSpeed = 3.2f;
        UpdateMoveable();
        _animator.SetBool("isPoliceman", true);
    }
    public override void UpdateOnIntPoint()
    {
        enemyAI.GetNextIntPoint(ref nextIntPoint, curIntPoint);
    }
}
