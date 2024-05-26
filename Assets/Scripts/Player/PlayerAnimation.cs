using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator animator;
    PlayerMovement playermove;
    SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        // Reference to PlayerMovement (script)
        GameObject obj = GameObject.FindWithTag("Player");
        playermove = obj.GetComponent<PlayerMovement>();

        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveStatus();
    }

    void MoveStatus()
    {
        if(playermove.moveDir.x != 0 || playermove.moveDir.y != 0)
        {
            animator.SetBool("Move", true);
            SpriteDirectionChecker();
        }
        else
        {
            animator.SetBool("Move", false);
        }
    }

    void SpriteDirectionChecker()
    {
        if(playermove.lastHorizontalVector < 0)
        {
            sprite.flipX = true;
        }
        else
        {
            sprite.flipX = false;
        }
    }
}
