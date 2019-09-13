using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemyScript : MonoBehaviour
{

    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;
    public int maxjumps = 2;

    bool grounded;
    int attack1Index = 0;
    bool climbing;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;
    Controller2D controller;

    public GameObject player1;
    public GameObject player2;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
    }

    void Update()
    {

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
        if (controller.collisions.below)
        {
            climbing = false;
            maxjumps = 2;
        }

        
        float targetVelocityX = 0.1f * moveSpeed;
        GameObject playerChase;
        if(Vector3.Distance(player1.transform.position, transform.position) <
            Vector3.Distance(player2.transform.position, transform.position))
        {
            playerChase = player1;
        }
        else
        {
            playerChase = player2;
        }
    
        if(playerChase.transform.position.x < transform.position.x)
        {
            targetVelocityX *= -1;
        }


        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
