using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class Moveable2D
    {
        public float speed;
        public Moveable2D(float speed = 0f)
        {
            this.speed = speed;
        }
        public void MoveTo(Rigidbody2D rb, int targetX, int targetY)
        {
            Vector2 position = rb.position;
            Vector2 target = new Vector2(targetX, targetY);
            float distance = Vector2.Distance(position, target);
            position.x = Mathf.MoveTowards(position.x, targetX, Mathf.Min(speed * Time.deltaTime, distance));
            position.y = Mathf.MoveTowards(position.y, targetY, Mathf.Min(speed * Time.deltaTime, distance));
            rb.MovePosition(position);
        }
        public void MoveTo(Rigidbody2D rb, Vector2 target)
        {
            Vector2 position = rb.position;
            float distance = Vector2.Distance(position, target);
            position.x = Mathf.MoveTowards(position.x, target.x, Mathf.Min(speed * Time.deltaTime, distance));
            position.y = Mathf.MoveTowards(position.y, target.y, Mathf.Min(speed * Time.deltaTime, distance));
            rb.MovePosition(position);
        }
    }
    public abstract class EnemyAI 
    {
        public List<Vector2Int> waypoints;
        public int currentTargetWaypointIndex;
        public bool isBlocked;
        public EnemyAI()
        {
            this.waypoints = null;
            this.currentTargetWaypointIndex = 0;
            this.isBlocked = false;
        }
        public abstract void GetNextIntPoint(ref Vector2Int nextIntPoint, Vector2Int curIntPoint);
    }
    public class EnemyAI_Sleep : EnemyAI
    {
        public override void GetNextIntPoint(ref Vector2Int nextIntPoint, Vector2Int curIntPoint)
        {
            nextIntPoint = curIntPoint;
        }
    }
    public class EnemyAI_FixedPatrol : EnemyAI
    {
        private bool revertDirection = false;
        public override void GetNextIntPoint(ref Vector2Int nextIntPoint, Vector2Int curIntPoint)
        {
            if (curIntPoint == waypoints[currentTargetWaypointIndex])
            {
                if (currentTargetWaypointIndex == waypoints.Count - 1)
                {
                    revertDirection = true;
                }
                else if (currentTargetWaypointIndex == 0)
                {
                    revertDirection = false;
                }
                currentTargetWaypointIndex = (currentTargetWaypointIndex + 1 * (revertDirection ? -1 : 1)) % waypoints.Count;
            }
            var path = PathSearcher.FindWayTo(curIntPoint, waypoints[currentTargetWaypointIndex]);
            if (path != null && path.Count > 1)
            {
                nextIntPoint.Set(path[1].x, path[1].y);
            }
        }
    }
    public class EnemyAI_RandomPatrol : EnemyAI
    {
        public List<Vector2Int> directions = new List<Vector2Int>(4) { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        public Vector2Int latestChoice = Vector2Int.zero;
        public override void GetNextIntPoint(ref Vector2Int nextIntPoint, Vector2Int curIntPoint)
        {
            isBlocked = false;
            Vector2Int target = curIntPoint + latestChoice;
            // latestChoice is preferred
            if (latestChoice != Vector2Int.zero && Blackbroad.map.isReachable(target.x, target.y))
            {
                nextIntPoint.Set(target.x, target.y);
            }
            else // latestChoice is invalid
            {
                List<Vector2Int> optionalDirections = new List<Vector2Int>(directions);
                int chosenDirectionIndex = Random.Range(0, 4);
                target = curIntPoint + optionalDirections[chosenDirectionIndex];
                while (!Blackbroad.map.isReachable(target.x, target.y))
                {
                    optionalDirections.RemoveAt(chosenDirectionIndex);
                    if (optionalDirections.Count == 0)
                    {
                        isBlocked = true;
                        break;
                    }
                    chosenDirectionIndex = Random.Range(0, optionalDirections.Count);
                    target = curIntPoint + optionalDirections[chosenDirectionIndex];
                }
                if (!isBlocked)
                {
                    nextIntPoint.Set(target.x, target.y);
                    latestChoice = optionalDirections[chosenDirectionIndex];
                }
            }
        }
    }
    public class EnemyAI_ChasePlayer : EnemyAI
    {
        public override void GetNextIntPoint(ref Vector2Int nextIntPoint, Vector2Int curIntPoint)
        {
            var path = PathSearcher.FindWayTo(curIntPoint, Blackbroad.playerIntPosition);
            if (path != null && path.Count > 1)
            {
                nextIntPoint.Set(path[1].x, path[1].y);
            }
        }
    }
}
