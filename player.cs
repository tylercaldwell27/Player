using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {

    Rigidbody2D rb;
    public Rigidbody2D rb2;
    
    public float speed;//players movement speed
    public float jumpForce;//how high the player can jump
    public float fallSpeed;//how fast the player falls
    public float health = 100;// the player's health

    public Animator anim; // for calling toe animations

    public bool powerUp;//for the power up
   
    //for the projectile
    public Rigidbody2D projectile;
    public float projectileForce;
    public Transform projectileSpawnPoint;

    
    public bool isFacingRight;// To see what way the player is facing
    //for seeing if hte player is on the ground
    public bool isGrounded;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask isGroundLayer;

    public float speedBoostTime = 5.0f;
    // Use this for initialization
    void Start () {
        //creates a rigidbody in the player
        rb = GetComponent<Rigidbody2D>();
        rb.mass = 1.0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        //if the player doesn't have a rb than it adds one
        if (!rb2)
        {
            Debug.LogWarning("Rigidbody not found" + name + ". Adding");
            rb2 = gameObject.AddComponent<Rigidbody2D>();
        }

        // checks if the player has any speed
        if (speed <= 0 || speed > 5.0f)
        {
            speed = 5.0f;
            Debug.LogWarning("speed not set on" + name + ". Defaulting to 5");
        }
        // checks if the player has jump force
        if (jumpForce <= 0 || jumpForce > 20.0f)
        {
            jumpForce = 10.0f;
            Debug.LogWarning("jumpForce not set on" + name + ". Defaulting to 10");
        }
        
        // checks for groundcheck
        if (!groundCheck)
        {
            groundCheck = GameObject.Find("GroundCheck").GetComponent<Transform>();
        }
        if (groundCheckRadius <= 0)
        {
            groundCheckRadius = 0.1f;
        }

        // if the is no projectile
        if (!projectile)
        {
            Debug.LogError("projectile not found on" + name);
        }

        if (!projectileSpawnPoint)
        {
            Debug.LogError("projectilleSpawnPoint not found on" + name);
        }
        if (projectileForce <= 0)
        {
            projectileForce = 20.0f;
        }
    }
	
	// Update is called once per frame
	void Update () {
        float moveValue = Input.GetAxisRaw("Horizontal");

        anim.SetFloat("speed", Mathf.Abs(moveValue));

        //checks if the player is on the ground
        if (groundCheck)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, isGroundLayer);
        }

        //if the player is on the ground
        if (isGrounded)
        {
            //if the jump button is pressed while on the ground
            if (Input.GetButtonDown("Jump")) rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetBool("jump",false);
        }
        
        //if the fire button is pressed
        if (Input.GetButtonDown("Fire1"))
        {
            fire();// calls the fire merthod
            anim.SetBool("shoot",true);
        }

        // the the fire button isn't pressed
        if (!Input.GetButtonDown("Fire1"))
        {
            anim.SetBool("shoot", false);
        }

        // the the player is in the air
        if (!isGrounded)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallSpeed - 1) * Time.deltaTime;
            anim.SetBool("jump",true);
        }
        rb.velocity = new Vector2(moveValue * speed, rb.velocity.y);
        if (health <= 0)
        {
            anim.SetBool("death", true);
            
        }
       // for switching the way the player is looking
        if ((moveValue < 0 && isFacingRight) ||
           (moveValue > 0 && !isFacingRight))
            flip();//calls the flip method

        // when the power up is in use
        if(powerUp == true)
        {
            speed = 15;
        }
        if (powerUp == false)
        {
            speed = 5;
        }

    }
    //checking collision with the enemy
    void OnCollisionEnter2D(Collision2D collision)
    {

        
        if (collision.gameObject.tag == "Enemy")
        {
          
                health -= 10;
                anim.SetBool("hit", true);
           
            StartCoroutine(hiteffect());
          

            
        }  
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if 'Character' collides with something tagged as 'Collectible'
        if (collision.gameObject.tag == "SpeedBoost")
        {
            powerUp = true;
            anim.SetBool("powerup", true);
            StartCoroutine(StopPowerUp());
            Destroy(collision.gameObject);
        }
    }
    //the fire gun method
          void fire() {
        Debug.Log("Pew Pew ");
        if (projectile && projectileSpawnPoint)
        {
            //for having the bullet shoot the way the play is facing
            Rigidbody2D temp = Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), temp.GetComponent<Collider2D>(), true);
            if (isFacingRight)
                temp.AddForce(projectileSpawnPoint.right * projectileForce, ForceMode2D.Impulse);
            else
                temp.AddForce(-projectileSpawnPoint.right * projectileForce, ForceMode2D.Impulse);
        }
    }

    void flip()
    {
        // Toggle variable
        isFacingRight = !isFacingRight;

        // Keep a copy of 'localScale' because scale cannot be changed directly
        Vector3 scaleFactor = transform.localScale;

        // Change sign of scale in 'x'
        scaleFactor.x *= -1; // or - -scaleFactor.x

        // Assign updated value back to 'localScale'
        transform.localScale = scaleFactor;
    }

    IEnumerator hiteffect()
    {
      
        yield return new WaitForEndOfFrame();//wait for the frame to end
        anim.SetBool("hit", false);
       
    }

    IEnumerator StopPowerUp()
    {

        yield return new WaitForSeconds(speedBoostTime);
        anim.SetBool("powerup", false);
        powerUp = false;

    }


}
