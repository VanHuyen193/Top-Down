using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 lastMoveVector;

    Rigidbody2D rd;
    PlayerStats player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerStats>();
        rd = GetComponent<Rigidbody2D>();
        lastMoveVector = new Vector2(1, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        InputManagement();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;

        if(moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMoveVector = new Vector2(lastHorizontalVector, 0f);
        }
        if(moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            lastMoveVector = new Vector2(0f, lastVerticalVector);
        }
        if (moveDir.x != 0 && moveDir.y != 0)
        {
            lastMoveVector = new Vector2(lastHorizontalVector, lastVerticalVector);
        }
    }

    void Move()
    {
        rd.velocity = new Vector2(moveDir.x * player.currentMoveSpeed, moveDir.y * player.currentMoveSpeed);
    }
}
