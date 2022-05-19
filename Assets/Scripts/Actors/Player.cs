using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The behaviour of the player character
/// </summary>
[System.Serializable]
public class Player : Actor
{
    /// <summary>
    /// A game object to act as a projectile which is shot when <see cref="Shoot()"/> is called
    /// </summary>
    /// <remarks>
    /// Needs to be defined in the Unity editor/inspector
    /// </remarks>
    [SerializeField] private GameObject bul;

    /// <summary>
    /// The <see cref="P_HealthBar"/> component of a game object which acts as a health bar
    /// </summary>
    /// <remarks>
    /// Needs to be defined in the Unity editor/inspector
    /// </remarks>
    public P_HealthBar healthBar;

    public bool canHit;

    // Is called before the first frame update 
    protected override void SetBaseVariables()
    {
        maxHealth = maxHealth == 0 ? 10 : maxHealth;
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Is called once per frame
    // This shit is confusing as fuck, I need a flow chart
    override protected void Act()
    {
        float xMovement = Input.GetAxisRaw("Horizontal") * speed;
        if (Input.GetKeyDown(KeyCode.P))
            GetHit(1, 1, 1);

        if (state == States.stuck)
        {
            return;
        }
        if (state != States.ability)
            movement.x = xMovement;

        if (state == States.attack)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                state = States.idle;
            else
            {
                if (Input.GetKeyDown(KeyCode.J) && canHit)
                {
                    animator.SetBool("NextAttack", true);
                } 
                else
                {
                    animator.SetBool("NextAttack", false);
                }
                return;
            }
        }

        if (state != States.ability)
        {
            if (ground)
            {
                hasJump = true;
                hasAbility = true;

                state = movement.x == 0 ? States.idle : States.walking;
            }
            else
            {
                state = States.falling;
            }
            if (xMovement != 0)
            {
                faceDir = xMovement > 0 ? 1 : -1;
            }
            Walk();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (ground || hasAbility)
            {
                if (!ground)
                {
                    hasAbility = false;
                }
                timer = 0.1f;
                movement.x = 1;
                state = States.ability;
            }
        }
        if (state == States.ability)
        {
            Dash();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Shoot();
        }
    }

    /// <summary>
    /// Sets the state to attack
    /// </summary>
    protected void Attack()
    {
        state = States.attack;
    }
    
    /// <summary>
    /// Crates a clone of <see cref="bul"/>
    /// </summary>
    void Shoot ()
    {
        var bullet = Instantiate<GameObject>(bul, transform.position, Quaternion.Euler(0, 0, 0));
        bullet.GetComponent<Projectile>().speed *= faceDir;
    }

    /// <summary>
    /// Walks
    /// </summary>
    void Walk()
    {
        float xMovement = Input.GetAxisRaw("Horizontal") * 3 * speed;
        movement.x = xMovement;
    }

    /// <summary>
    /// Checks if the <see cref="Player"/> can jump, and does if can
    /// </summary>
    void Jump()
    {
        if (ground || hasJump)
        {
            if (!ground)
            {
                hasJump = false;
            }
            state = States.falling;
            movement.y = jump;
        }
    }

    /// <summary>
    /// Executes the "ability" of the <see cref="Player"/>
    /// </summary>
    void Dash()
    {
        if (timer <= 0 || movement.x == 0)
        {
            state = States.falling;
            return;
        }
        timer -= Time.deltaTime;
        movement = new Vector2(20 * faceDir, 0);
    }

    protected override void BecomeCorpse()
    {
        throw new System.NotImplementedException();
    }

    public override bool GetHit(float Damage, float Force, float Stun)
    {
        if (base.GetHit(Damage, Force, Stun))
        {
            healthBar.SetHealth(health);
            Debug.Log("You died lmao");
            return true;
        }
        healthBar.SetHealth(health);
        Debug.Log("You got hit lmao");
        return false;
    }
}
