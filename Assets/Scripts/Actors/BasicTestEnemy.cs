using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTestEnemy : Enemy
{
    [SerializeField] private GameObject bullet;

    // SetMoreBaseVariables is called before the first frame update
    protected override void SetMoreBaseVariables()
    {
        base.SetMoreBaseVariables();
    }

    private float wait = 0;
    // Act is called once per frame
    protected override void Act()
    {
        faceDir = ToFacePlayer();
        if (wait <= 0 )
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.right * faceDir, 5);
            foreach(RaycastHit2D hit in hits)
            {
                Player target = hit.transform.GetComponent<Player>();
                if (target != null && LineOfSightToPlayer())
                {
                    Shoot(bullet, faceDir);
                    wait = 5;
                    break;
                }
            }
        }
        else
        {
            wait -= Time.deltaTime;
        }
    }
}
