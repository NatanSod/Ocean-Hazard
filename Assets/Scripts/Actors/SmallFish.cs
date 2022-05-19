using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallFish : Enemy
{
        float baseSpeed;
    protected override void SetMoreBaseVariables()
    {
        baseSpeed = speed;
        flies = true;
    }
    public Vector2 direction = Vector2.zero;
    protected override void Act()
    {
        switch (state)
        {
            case States.idle:
            case States.walking:
                speed = baseSpeed;
                // If it's able to move and can see the player, face and move towards them
                if (LineOfSightToPlayer())
                {
                    state = States.walking;
                    direction = DirectionToPlayer();
                    faceDir = ToFacePlayer();

                    movement = direction * speed;
                    // If the player also is close enough, attack
                    if (DistanceToPlayer() <= 2)
                    {
                        state = States.attack;
                    }
                }
                // If it can't see the player, don't
                else
                {
                    movement = Vector2.zero;
                    state = States.idle;
                }
                break;

            // Keep moving in the same direction until the animation is finished
            case States.attack:
                movement = direction * speed;
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                {
                    state = States.idle;
                }
                break;

            case States.stuck:
                // Get knocked back until the time given to HandleKnockBack has passed
                if (stuckTimer <= 0)
                {
                    state = States.idle;
                    break;
                }
                stuckTimer -= Time.deltaTime;

                // If the momentum is 0, leave it alone, it should not be raised
                if (movement.magnitude <= 0) break;

                // Bring the x and y momentum closer to 0 but make sure to not pass it
                float drag = 5;
                if (Mathf.Abs(movement.x) < Mathf.Abs(direction.x * Time.deltaTime * drag))
                {
                    movement.x = 0;
                } 
                else 
                {
                    movement.x += direction.x * Time.deltaTime * drag;
                }

                if (Mathf.Abs(movement.y) < Mathf.Abs(direction.y * Time.deltaTime * drag))
                {
                    movement.y = 0;
                }
                else
                {
                    movement.y += direction.y * Time.deltaTime * drag;
                }
                break;

            case States.dead:
                //If the death animation has finished playing, destroy this object
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                {
                    Destroy(gameObject);
                }
                break;
        }
    }
    
    public override bool GetHit(float Damage, float Force, float Stun)
    {
        return base.GetHit(Damage, Force, Stun);
    }

    protected override void BecomeCorpse()
    {
        flies = false;
        state = States.dead;
    }
}
