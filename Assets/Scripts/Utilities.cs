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
}
