using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An abstract class which includes the base features for describing an "actor",
/// basically something which moves around and may interact with the scene and 
/// other actors
/// </summary>
/// <remarks>
/// It's important to put objects with this class on the right layer, 
/// either "Player" or "Enemies"
/// </remarks>
[System.Serializable]
public abstract class Actor : MonoBehaviour
{
    /// <summary>
    /// The <see cref="Collider2D"/> component that is attached to the game object this is attached to
    /// </summary>
    protected Collider2D collide;

    /// <summary>
    /// The <see cref="Rigidbody2D"/> component that is attached to the game object this is attached to
    /// </summary>
    protected Rigidbody2D rBody;

    /// <summary>
    /// The <see cref="Animator"/> component that is attached to the game object this is attached to
    /// </summary>
    protected Animator animator;

    /// <summary>
    /// The <see cref="Melee"/> hitbox component that is attached to a child of this object
    /// </summary>
    /// <remarks>
    /// Needs to be defined in the Unity editor/inspector
    /// </remarks>
    public Melee meleeHitbox;

    /// <summary>
    /// A <see cref="Vector2"/> which keeps track of how much 
    /// and in what direction it should move
    /// </summary>
    [SerializeField] protected Vector2 movement = Vector2.zero;

    /// <summary>
    /// The layer which contains the level collision
    /// </summary>
    /// <remarks>
    /// It's used to differentiate from the level and anything 
    /// else the <see cref="Actor"/> might be colliding with
    /// </remarks>
    protected LayerMask walls;

    /// <summary>
    /// The variable which describes the <see cref="Actor"/>'s max vertical speed
    /// </summary>
    /// <remarks>
    /// Needs to be defined in the Unity editor/inspector
    /// </remarks>
    [SerializeField]
    protected float speed,
    /// <summary>
    /// The variable which describes the "power" of the <see cref="Actor"/>'s jump
    /// </summary>
    /// <remarks>
    /// Needs to be defined in the Unity editor/inspector
    /// </remarks>
    jumpHeight,
    /// <summary>
    /// The variable which describes the effect of gravity on the <see cref="Actor"/>
    /// </summary>
    /// <remarks>
    /// Needs to be defined in the Unity editor/inspector
    /// </remarks>
    gravity;

    protected float jump;

    /// <summary>
    /// A boolean which stores weather or not the <see cref="Actor"/> has a jump,
    /// </summary>
    /// <remarks>
    /// The <see cref="Player"/> uses it to keep track of being able to double jump, 
    /// since few enemies, if any, should be able to do that and I can't think of a
    /// second use for it, it probably won't be used very often
    /// </remarks>
    /// <value>If it is false, then there is no jump</value>
    [SerializeField]
    protected bool hasJump = false,

    /// <summary>
    /// A boolean which stores weather or not the <see cref="Actor"/> can use it's "ability",
    /// </summary>
    /// <remarks>
    /// The "ability" changes between the different classes which extends <see cref="Actor"/>
    /// </remarks>
    /// <value>If it is false, then it shouldn't be able to use it's "ability"</value>
    hasAbility = false;

    protected bool flies = false;

    /// <summary>
    /// A property which can only be get and is used for seeing if the actor is 
    /// touching the ground or not
    /// </summary>
    /// <remarks>
    /// If <see cref="movement"/>'s y value is practically equal to 0, a byproduct 
    /// of a <see cref="Collider2D"/> and <see cref="Rigidbody2D"/> object having 
    /// a floor beneath it, then it performs a
    /// <see cref="Physics2D.BoxCast(Vector2, Vector2, float, Vector2, float, int)"/>,
    /// the dimensions of which is based on <see cref="collide"/>,
    /// to check for level collision beneath it
    /// </remarks>
    /// <value>If it is standing on ground the it equals true</value>
    protected bool IsGrounded
    {
        get
        {
            if (flies)
            {
                return true;
            }
            if (movement.y >= -0.5f && movement.y <= 0.5f)
            {
                float margin = 0.02f;
                RaycastHit2D ray;

                Vector2 origin = new Vector2(transform.position.x + collide.offset.x * faceDir, transform.position.y + collide.offset.y);
                Vector2 box = new Vector2(collide.bounds.size.x, margin);
                float distance = collide.bounds.extents.y + margin / 2;

                ray = Physics2D.BoxCast(origin, box, 0f, Vector2.down, distance, walls);
                return ray.collider != null;
            }

            return false;
        }
    }

    /// <summary>
    /// Used to keep track of weather or not the <see cref="Actor"/> is grounded
    /// </summary>
    /// <remarks>
    /// It's defined at the beginning by setting it to <see cref="IsGrounded"/>,
    /// due to my fear of it being relatively computation heavy, and because if 
    /// something happens during the update which would make it no longer grounded,
    /// like jumping, then <see cref="IsGrounded"/> wouldn't know until the next 
    /// physics update
    /// </remarks>
    protected bool ground;

    /// <summary>
    /// A property which is used to interact with a boolean in the <see cref="animator"/>
    /// that decides if it should keep playing the current animation or play a new one
    /// </summary>
    /// <remarks>
    /// The primary way it does this is by checking if it's in the same state as it was at the start of the update
    /// </remarks>
    /// <value>If it's true then it should keep playing the last animation</value>
    protected bool AnimationIsPlaying
    {
        get => animator.GetBool("IsPlaying");
        set => animator.SetBool("IsPlaying", value);
    }

    /// <summary>
    /// A timer used to make sure specific things take the amount of time
    /// </summary>
    protected float timer = 0;


    /// <summary>
    /// An int that keeps track of which way the sprite should be facing 
    /// and if the sprite should be flipped or not
    /// </summary>
    /// <value>
    /// If it's -1 then it's facing to the left should be flipped, 
    /// if it's 1 then it's facing to the right and shouldn't be flipped
    /// </value>
    public int faceDir;

    /// <summary>
    /// Max health
    /// </summary>
    /// <remarks>
    /// Needs to be defined in the Unity editor/inspector
    /// </remarks>
    public float maxHealth,
    /// <summary>
    /// Current health
    /// </summary>
    /// <remarks>
    /// Needs to be defined in the Unity editor/inspector
    /// </remarks>
    health;

    /// <summary>
    /// A collection of states which the <see cref="Actor"/> can be in.
    /// Each on should have an animation in the <see cref="animator"/>
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <listheader>Possible values:</listheader>
    /// <item><see cref="States.idle"/></item>
    /// <item><see cref="States.walking"/></item>
    /// <item><see cref="States.falling"/></item>
    /// <item><see cref="States.attack"/></item>
    /// <item><see cref="States.ability"/></item>
    /// <item><see cref="States.stuck"/></item>
    /// <item><see cref="States.dead"/></item>
    /// </list>
    /// </remarks>
    protected enum States
    {
        idle,
        walking,
        falling,
        attack,
        ability,
        stuck,
        dead
    }

    /// <summary>
    /// The state which the <see cref="Actor"/> is currently in
    /// </summary>
    [SerializeField] protected States state = States.idle;

    protected float stuckTimer = 0;

    private GameObject _AttackSource;

    public void SetAttackSource(GameObject o)
    {
        _AttackSource = o;
    }

    public GameObject GetAttackSource()
    {
        GameObject a = _AttackSource;
        _AttackSource = null;
        return a;
    }

    // Start is called before the first frame update
    /// <summary>
    /// Set's the necessary default values 
    /// </summary>
    protected void Start()
    {
        collide = gameObject.GetComponent<Collider2D>();
        rBody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        walls = LayerMask.GetMask("Level");
        faceDir = faceDir == 0 ? 1 : faceDir;

        jump = Mathf.Sqrt((Mathf.Abs(2 * jumpHeight * gravity * 10)));

        SetBaseVariables();
    }

    /// <summary>
    /// Some classes derived from <see cref="Actor"/> may want
    /// to define more things during Start, in which case they do so here
    /// </summary>
    protected abstract void SetBaseVariables();

    // Update is called once per frame
    /// <summary>
    /// Performs the necessary actions for all <see cref="Actor"/> for an update,
    /// like settings velocity to <see cref="movement"/>, flipping the sprite based on 
    /// <see cref="faceDir"/>, giving a value to <see cref="AnimationIsPlaying"/>
    /// and the most important one, calling <see cref="Act()"/>
    /// </summary>
    protected void Update()
    {
        States startState = state;

        movement = rBody.velocity;
        ground = IsGrounded;
        if (!ground)
            movement.y -= gravity * Time.deltaTime * 10;

        Act();

        rBody.velocity = movement;
        transform.localScale = new Vector3(faceDir, 1, 1);

        AnimationIsPlaying = startState == state;
        animator.SetInteger("State", (int)state);
    }

    /// <summary>
    /// All classes want to do more than what's in <see cref="Update()"/>,
    /// since it only has the things which all derived classes want to do.
    /// So if they want to do more they do so here
    /// </summary>
    protected abstract void Act();

    /// <summary>
    /// A function through which the <see cref="animator"/> calls <see cref="Hitbox.ReHit()"/>,
    /// since it can only access the functions of the object it's attached to, and not functions
    /// of children of the object it's attached to
    /// </summary>
    protected void ReHit()
    {
        meleeHitbox.ReHit();
    }

    /// <summary>
    /// A function for dealing damage this <see cref="Actor"/>
    /// </summary>
    /// <remarks>
    /// If health drops below 0 then it sets the state to dead and calls <see cref="BecomeCorpse()"/>
    /// </remarks>
    /// <param name="Damage">The damage the <see cref="Actor"/> should take</param>
    /// <param name="Force">The force to knock it back with</param>
    /// <param name="name"/>The time for which it should be unable to act</param>
    public virtual bool GetHit(float Damage, float Force, float Stun)
    {
        health -= Damage;

        if (health <= 0)
        {
            state = States.dead;
            HandleKnockBack(Force, 0);
            BecomeCorpse();
            return true;
        }
        HandleKnockBack(Force, Stun);
        return false;
    }

    protected virtual void HandleKnockBack(float Force, float Stun) { return; }

    /// <summary>
    /// Create an new instance of <paramref name="Bullet"/> pointing in the direction of <paramref name="Direction"/>
    /// </summary>
    /// <param name="Bullet">The object to create a new instance of</param>
    /// <param name="Direction">The direction it will travel in the x axis, keep it to 1 or -1 please</param>
    /// <param name="speed">Modify it's speed</param>
    protected void Shoot(GameObject Bullet, int Direction, float speed = 1)
    {
        var bullet = Instantiate<GameObject>(Bullet, transform.position, Quaternion.Euler(0, 0, 0));
        bullet.GetComponent<Projectile>().speed *= Direction * speed;
    }

    /// <summary>
    /// Create an new instance of <paramref name="Bullet"/> pointing in the direction of <paramref name="Angle"/>
    /// </summary>
    /// <param name="Bullet">The object to create a new instance of</param>
    /// <param name="Angle">The direction it will travel described as a rotation around the z axis</param>
    /// <param name="speed">Modify it's speed</param>
    protected void Shoot(GameObject Bullet, float Angle, float speed = 1)
    {
        var bullet = Instantiate<GameObject>(Bullet, transform.position, Quaternion.Euler(0, 0, Angle));
        bullet.GetComponent<Projectile>().speed *= speed;
    }

    /// <summary>
    /// A function for doing whatever it is this <see cref="Actor"/> does
    /// once it's "dead"
    /// </summary>
    protected abstract void BecomeCorpse();

    public float DistanceTo(GameObject target)
    {
        return (target.transform.position - transform.position).magnitude;
    }

    /// <summary>
    /// Figures out the angel to the <paramref name="target"/>
    /// </summary>
    /// <param name="target">The object to face</param>
    /// <returns>a float describing the rotation requierd to face <paramref name="target"/></returns>
    public float AngelTo(GameObject target)
    {
        Vector2 difference = target.transform.position - transform.position;
        float angel = Mathf.Atan2(difference.x, difference.y) * Mathf.Rad2Deg;
        return angel;
    }

    /// <summary>
    /// Figures out the direction to <paramref name="target"/>
    /// </summary>
    /// <param name="target">The object to find the direction to</param>
    /// <returns>A normalized Vector2 describing the direction nedded to move to reach <paramref name="target"/></returns>
    public Vector2 DirectionTo(GameObject target)
    {
        Vector2 difference = target.transform.position - transform.position;
        difference = Vector3.Normalize(difference);
        return difference;
    }

    /// <summary>
    /// Checks if there are walls between this object and the target
    /// </summary>
    /// <param name="target">The object to check line of sight to</param>
    /// <returns>if(thereIsAWall)</returns>
    public bool LineOfSightTo(GameObject target)
    {
        Vector2 direction = target.transform.position - transform.position;
        float distance = DistanceTo(target);
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, distance, walls);
        if (hits.Length > 0)
        {
            return false;
        }
        return true;
    }
}
