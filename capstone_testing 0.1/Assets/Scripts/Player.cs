using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

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
    Animator animator;
	Controller2D controller;
    SpriteRenderer sprite;

	void Start() {
		controller = GetComponent<Controller2D> ();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		print ("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
	}

	void Update() {

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}
        if (controller.collisions.below)
        {
            climbing = false;
            animator.SetBool("falling", false);
            maxjumps = 2;
        }
        else if(velocity.y < 0 && climbing == false)
        {
            animator.SetBool("falling", true);
            animator.SetBool("jumping", false);
        }
        else if(velocity.y > 0 && climbing == false)
        {
            animator.SetBool("jumping", true);
        }
		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
        if (input.x != 0)
        {
            //              temporary corner grab system

            //Dosent account for turning around when facing a wall.
            if(velocity.x != 0 || velocity.y != 0)
            {
                if(controller.directionX < 0 && velocity.x < 0)
                {
                    if(controller.canClimbUp < 0)
                    {
                        animator.SetTrigger("pullup");
                        animator.SetBool("jumping", false);
                        climbing = true;
                        Debug.Log("left");
                        velocity.y = 2;
                    }
                }
                if (controller.directionX > 0 && velocity.x > 0)
                {
                    if (controller.canClimbUp > 0)
                    {
                        
                        animator.SetTrigger("pullup");
                        animator.SetBool("jumping", false);
                        climbing = true;
                        Debug.Log("right");
                        velocity.y = 2;
                    }
                }
            }
           


            //need to account for special moves, rolling, dashing
            animator.SetBool("running", true);
            if (input.x < 0)
            {
                sprite.flipX = true;
               
            }
            else
            {
                sprite.flipX = false;
            }
        }
        else
        {
            animator.SetBool("running", false);
        }
        if ((Input.GetKeyDown (KeyCode.Space) || (Input.GetKeyDown(KeyCode.W))) && (controller.collisions.below ||  maxjumps > 0)) {
            
            maxjumps--;
			velocity.y = jumpVelocity;
		}
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) 
        {
            //Debug.Log(attack1Index.ToString());
            //need to pause movement and have a cooldown
            if (controller.collisions.below)
            {
                velocity.x = 0;
            }
            //set a timer
            //possibly replace with an array
            if (attack1Index == 0)
            {
                animator.SetTrigger("attack1");
                attack1Index++;
            }
            else if (attack1Index == 1)
            {
                animator.SetTrigger("attack1_2");
                attack1Index++;
            }
            else if (attack1Index == 2)
            {
                animator.SetTrigger("attack1_3");
                attack1Index++;
            }
            else if (attack1Index == 3)
            {
                animator.SetTrigger("attack1special");
                attack1Index = 0;
            }
           
            //make attack always take precident over falling anim


        }
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1))
        {
            //need to pause movement and have a cooldown
            //make attack always take precident over falling anim
            if (controller.collisions.below)
            {
                velocity.x = 0;
            }
            
            animator.SetTrigger("attack2");
        }
            if (Input.GetKeyDown(KeyCode.S))
        {
            animator.SetTrigger("slide");
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            animator.SetTrigger("crouch");
        }
        float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;
		controller.Move (velocity * Time.deltaTime);
	}
}
