using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It contains the essentials of what an Enemy object needs
/// </summary>
/// <remarks>
/// Is derived from the <see cref="Actor"/> class.
/// Primarily, what it adds is functions that help determine the position of the <see cref="Player"/> relative to itself.
/// </remarks>
public abstract class  Enemy : Actor
{
    protected Player player;
    // SetBaseVariables is called before the first frame update
    protected override void SetBaseVariables()
    {
        player = WorldMaster.GetPlayer();
        maxHealth = maxHealth == 0 ? 10 : maxHealth;
        health = maxHealth;
        state = States.idle;
        SetMoreBaseVariables();
    }

    /// <summary>
    /// <see cref="Actor.SetBaseVariables"/> 2: Enemy edition
    /// </summary>
    protected virtual void SetMoreBaseVariables() { return; }

    public override bool GetHit(float Damage, float Force, float Stun)
    {
        return base.GetHit(Damage, Force, Stun);
    }

    /// <summary>
    /// Get knocked back with appropriate force
    /// </summary>
    /// <param name="Force">The force to get knocked by</param>
    /// <param name="Stun">The time it's unable to act for</param>
    override protected void HandleKnockBack (float Force, float Stun)
    {
        movement = -DirectionToPlayer() * Force * 2;
        faceDir = ToFacePlayer();
        rBody.velocity = movement;

        if (state != States.dead)
        {
            stuckTimer = Stun;
            state = States.stuck;
            animator.Play(States.stuck.ToString());
        }
    }
    protected override void BecomeCorpse()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Where should it face?
    /// </summary>
    /// <returns>The value <see cref="Actor.faceDir"/> needs to have in order to face the player</returns>
    protected int ToFacePlayer()
    {
        // The player is to the right 
        if (player.transform.position.x > transform.position.x)
        {
            return 1;
        }
        // The player is to the left
        else if (player.transform.position.x < transform.position.x)
        {
            return -1;
        }
        // The player is in the same x coordinate. Since It can't really do anything else just keep facing the way.
        else
        {
            return faceDir;
        }
    }

    /// <summary>
    /// Is the player above or below
    /// </summary>
    /// <returns>1: above. -1: below. 0: same y</returns>
    protected int PlayerAboveOrBelow ()
    {
        // The player is above
        if (player.transform.position.y > transform.position.y)
        {
            return 1;
        }
        // The player is below
        else if (player.transform.position.y < transform.position.y)
        {
            return -1;
        }
        // The player is in the same y coordinate. Time to have a panic attack!
        else
        {
            return 0;
        }
    }

    // I am so smart. These will definitely make this more efficient.
    protected float DistanceToPlayer() => DistanceTo(player.gameObject);
    protected float AngelToPlayer() => AngelTo(player.gameObject);
    protected Vector2 DirectionToPlayer() => DirectionTo(player.gameObject);
    protected bool LineOfSightToPlayer() => LineOfSightTo(player.gameObject);
}
