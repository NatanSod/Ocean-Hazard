using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// The base hitbox class from which all others are derived. <br/>
/// </summary>
/// 
/// <remarks>
/// <para>
/// A "Hitbox" is the thing that hits. The "Hurtbox" is the box that is hit, remember the difference
/// </para>
/// <para>
/// classes derived from <see cref="Hitbox"/>: <br/>
/// <seealso cref="Melee"/> <br/>
/// <seealso cref="Projectile"/>
/// </para>
/// </remarks>
public abstract class Hitbox : MonoBehaviour
{
    /// <summary> The damage that a hit actor will take if they are hit by the box </summary>
    /// <remarks> It can and should be changed during an animation using the inspector </remarks>
    [SerializeField]
    protected float attackPower = 2,
    /// <summary> How much force the hit actor will be knocked back with </summary>    
    /// <remarks> It can and should be changed during an animation using the inspector </remarks>
    force = 1,
    /// /// <summary> How long will the hit actor will be stunned for (seconds) </summary>    
    /// <remarks> It can and should be changed during an animation using the inspector </remarks>
    stun = 1;

    /// <summary>
    /// A list of everything the hitbox has previously collided with
    /// so that it doesn't hit the same thing twice when it isn't supposed to
    /// </summary>
    /// <remarks>
    /// It can be cleared by disabling and enabeling the object it's attached to
    /// </remarks>
    protected List<Actor> haveHit = new List<Actor>();

    /// <summary>
    /// The collider that is used for the <see cref="Hitbox"/>
    /// </summary>
    protected Collider2D collide;

    /// <summary>
    /// It's Start, do I need to explain it? It sets values and references to components
    /// </summary>
    protected void Start()
    {
        collide = gameObject.GetComponent<Collider2D>();
        if (collide == null)
        {
            throw new MissingComponentException($"You need to attach a collider to : {gameObject.name}");
        }
        SetOtherComponents();
    }

    /// <summary>
    /// Used to set components used by classes derived from 
    /// <see cref="Hitbox"/>
    /// </summary>
    /// <remarks>
    /// It's basically <see cref="Start"/> part 2
    /// </remarks>
    protected virtual void SetOtherComponents() { }

    /// <summary>
    /// When the object the <see cref="Hitbox"/> is activated to is enabled the <see cref="haveHit"/>
    /// is cleared so that a new hitbox won't have to be created when 
    /// starting an attack or letting one attack hit multiple times
    /// </summary>
    /// <remarks>
    /// It also checks if it currently colliding with anything
    /// </remarks>
    private void OnEnable()
    {
        haveHit.Clear();
    }

    public void ReHit()
    {
        haveHit.Clear();
        collide.enabled = false;
        collide.enabled = true;
    }

    /// <summary>
    /// It checks if the object that <paramref name="collision"/> 
    /// is attached to also has a <see cref="Actor"/> attached to it.
    /// If there isn't one it will assume that it hit a wall or similar
    /// </summary>
    /// 
    /// <remarks>
    /// <list type="bullet">
    /// <item> 
    ///     If it's a wall it will call: <see cref="HitWall"/> 
    /// </item>
    /// <item> 
    ///     If it's an <see cref="Actor"/> and it's not present in 
    ///     <see cref="haveHit"/> it will call: <see cref="HitActor(Actor)"/>
    ///     and then <see cref="OnHit"/>
    /// </item>
    /// </list>
    /// </remarks>
    /// 
    /// <param name="collision">The Collider2D that the hitbox collided with</param>
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody == null)
        {
            return;
        }

        Actor target = collision.gameObject.GetComponent<Actor>();

        if (target == null)
        {
            Debug.Log($"Hit a wall : {collision.gameObject.name}");
            HitWall();
        }
        else if (!haveHit.Contains(target))
        {
            Debug.Log($"Hit an actor : {collision.gameObject.name}");
            HitActor(target);
            OnHit();
        }
    }

    /// <summary>
    /// Call's the <paramref name="target"/>'s 
    /// <see cref="Actor.GetHit(float, float, float)"/> function,
    /// and adds them to <see cref="haveHit"/>
    /// </summary>
    /// <param name="target">The target</param>
    void HitActor(Actor target)
    {
        target.SetAttackSource(gameObject);
        target.GetHit(attackPower, force, stun);
        haveHit.Add(target);
    }

    /// <summary>
    /// Is called at the end of <see cref="OnTriggerEnter2D(Collider2D)"/> 
    /// if it collided with an <see cref="Actor"/>
    /// </summary>
    /// <remarks>
    /// Some derived classes may want to do more things after hitting a target, and in such cases they will override this
    /// </remarks>
    virtual protected void OnHit() { }

    /// <summary>
    /// Is called at the end of <see cref="OnTriggerEnter2D(Collider2D)"/> 
    /// if it collided with a wall
    /// </summary>
    /// <remarks>
    /// Some derived classes may want to do more things after hitting a wall, and in such cases they will override this
    /// </remarks>
    virtual protected void HitWall() { }
}
