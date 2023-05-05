using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class SkyPoliceControl : EnemyControl
{
    // enemyAI will be set later, in builder's CreateEnemy function.
    void Awake()
    {
        Initiate();
        enemyAI = new EnemyAI_ChasePlayerInSky();
        standardSpeed = speed = 1.5f;
        UpdateMoveable();
        _animator.SetBool("isSkyPoliceman", true);
        GetComponent<SpriteRenderer>().sortingOrder = 1;
    }
    public override void UpdateOnIntPoint()
    {
        enemyAI.GetNextIntPoint(ref nextIntPoint, curIntPoint);
    }
}
