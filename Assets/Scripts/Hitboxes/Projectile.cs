using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class derived from <see cref="Hitbox"/> that handels the behaviour of a projectile and and it's hitbox
/// </summary>
public class Projectile : Hitbox
{
    /// <summary>
    /// The <see cref="Animator"/> that's animating it's parent
    /// </summary>
    Animator animator;
    Rigidbody2D rigidbody2;

    [SerializeField]
    public float speed;

    [SerializeField]
    int pierce = 0;
    bool isDissolving = false;

    protected override void SetOtherComponents()
    {
        animator = gameObject.GetComponent<Animator>();
        if (animator == null)
        {
            throw new MissingComponentException($"You need to attach an animator to : {gameObject.name}");
        }
        rigidbody2 = gameObject.GetComponent<Rigidbody2D>();
        if (rigidbody2 == null)
        {
            throw new MissingComponentException($"You need to attach a rigidbody2D to : {gameObject.name}");
        }
    }

    // Update is called once per frame
    /// <summary>
    /// Sets the speed and checks if the projectile should cease to exist,
    /// and if it should then it calls <see cref="Object.Destroy(Object)"/>
    /// </summary>
    /// <remarks>
    /// It checks if it should be destroyed by checking if the destruction 
    /// animation is playing and if it's played to the end
    /// </remarks>
    void Update()
    {
        rigidbody2.velocity = transform.right * speed;
        if (isDissolving && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && animator.GetCurrentAnimatorStateInfo(0).IsName("Dissolve"))
        {
            Destroy(gameObject.gameObject);
        }
    }

    /// <remarks>
    /// If the <see cref="pierce"/> variable is 1 or less then it calls
    /// <see cref="StartDissolve"/>
    /// </remarks>
    /// <inheritdoc cref="Hitbox.OnHit"/>
    protected override void OnHit()
    {
        pierce--;
        if (pierce < 0)
        {
            StartDissolve();
        }
    }

    protected override void HitWall()
    {
        StartDissolve();

    }

    /// <summary>
    /// Starts the process of dissapearing
    /// </summary>
    void StartDissolve()
    {
        isDissolving = true;
        collide.enabled = false;
        animator.Play("Dissolve");
        speed = 0;
    }
}
