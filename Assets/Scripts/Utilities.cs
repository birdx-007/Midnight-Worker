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
            rb.velocity = (position - rb.position) / Time.deltaTime;
            rb.MovePosition(position);
        }
        public void MoveTo(Rigidbody2D rb, Vector2 target)
        {
            Vector2 position = rb.position;
            float distance = Vector2.Distance(position, target);
            position.x = Mathf.MoveTowards(position.x, target.x, Mathf.Min(speed * Time.deltaTime, distance));
            position.y = Mathf.MoveTowards(position.y, target.y, Mathf.Min(speed * Time.deltaTime, distance));
            rb.velocity = (position - rb.position) / Time.deltaTime;
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
            waypoints = null;
            currentTargetWaypointIndex = 0;
            isBlocked = false;
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
                optionalDirections.Remove(-latestChoice);// backward is the least preferred
                int chosenDirectionIndex = Random.Range(0, 3);
                target = curIntPoint + optionalDirections[chosenDirectionIndex];
                while (!Blackbroad.map.isReachable(target.x, target.y))
                {
                    optionalDirections.RemoveAt(chosenDirectionIndex);
                    if (optionalDirections.Count == 0)
                    {
                        target = curIntPoint - latestChoice;
                        if (!Blackbroad.map.isReachable(target.x, target.y))
                        {
                            isBlocked = true;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    chosenDirectionIndex = Random.Range(0, optionalDirections.Count);
                    target = curIntPoint + optionalDirections[chosenDirectionIndex];
                }
                if (!isBlocked)
                {
                    nextIntPoint.Set(target.x, target.y);
                    latestChoice = target - curIntPoint;
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
    public class EnemyAI_ChasePlayerInSky : EnemyAI
    {
        public override void GetNextIntPoint(ref Vector2Int nextIntPoint, Vector2Int curIntPoint)
        {
            var path = PathSearcher.FindWayTo(curIntPoint, Blackbroad.playerIntPosition, true);
            if (path != null && path.Count > 1)
            {
                nextIntPoint.Set(path[1].x, path[1].y);
            }
        }
    }
    public class EnemyAI_RushtoPlayer : EnemyAI
    {
        public bool isRushing = false;
        public float rushSpeed = 5f;
        public Vector2Int rushDirection = Vector2Int.zero;
        EnemyAI_RushtoPlayer(float rushSpeed)
        {
            this.rushSpeed = rushSpeed;
        }
        public override void GetNextIntPoint(ref Vector2Int nextIntPoint, Vector2Int curIntPoint)
        {
            if (isRushing)
            {
                Vector2Int target = curIntPoint + rushDirection;
                if (Blackbroad.map.isReachable(target.x, target.y))
                {
                    nextIntPoint.Set(target.x, target.y);
                }
                else
                {
                    isRushing = false;
                    rushDirection = Vector2Int.zero;
                }
            }
            else
            {
                Vector2Int latestFaceDirection = nextIntPoint - curIntPoint;
                Vector2Int? direction = PathSearcher.FindLineDirectionTo(curIntPoint, Blackbroad.playerIntPosition);
                if (direction != null && direction.Value != -latestFaceDirection)
                {
                    isRushing = true;
                    rushDirection = direction.Value;
                    Vector2Int target = curIntPoint + rushDirection;
                    nextIntPoint.Set(target.x, target.y);
                }
            }
        }
    }
}
