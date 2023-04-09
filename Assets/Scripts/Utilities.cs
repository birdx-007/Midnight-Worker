using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class Moveable2D
    {
        public float speed;
        public Moveable2D(float speed)
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
    public abstract class EnemyBehaviorControl
    {
        public float speed = 0f;
        public abstract void UpdateBehavior(ref Vector2Int nextIntPoint,Vector2Int curIntPoint);
    }
    public class EnemySleepControl : EnemyBehaviorControl
    {
        public EnemySleepControl()
        {
            speed = 0f;
        }
        public override void UpdateBehavior(ref Vector2Int nextIntPoint, Vector2Int curIntPoint)
        {
            nextIntPoint = curIntPoint;
        }
    }
    public class EnemyFixedPatrolControl : EnemyBehaviorControl
    {
        public List<Vector2Int> waypoints;
        public int currentTargetWaypointIndex;
        private bool revertDirection = false;
        public EnemyFixedPatrolControl(List<Vector2Int> waypoints)
        {
            speed = 2f;
            this.waypoints = waypoints;
            currentTargetWaypointIndex = 0;
        }
        public override void UpdateBehavior(ref Vector2Int nextIntPoint, Vector2Int curIntPoint)
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
    public class EnemyRandomPatrolControl : EnemyBehaviorControl
    {
        public List<Vector2Int> directions;
        public Vector2Int latestChoice;
        public bool isBlocked;
        public EnemyRandomPatrolControl()
        {
            speed = 2f;
            directions = new List<Vector2Int>(4) { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            latestChoice = Vector2Int.zero;
            isBlocked = false;
        }
        public override void UpdateBehavior(ref Vector2Int nextIntPoint, Vector2Int curIntPoint)
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
    public class EnemyChasePlayerControl : EnemyBehaviorControl
    {
        public EnemyChasePlayerControl()
        {
            speed = 3f;
        }
        public override void UpdateBehavior(ref Vector2Int nextIntPoint, Vector2Int curIntPoint)
        {
            var path = PathSearcher.FindWayTo(curIntPoint, Blackbroad.playerIntPosition);
            if (path != null && path.Count > 1)
            {
                nextIntPoint.Set(path[1].x, path[1].y);
            }
        }
    }
}
