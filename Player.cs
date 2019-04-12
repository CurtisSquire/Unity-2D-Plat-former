using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Player : MonoBehaviour
{
    //sets bool variables needed to toggle flip and attack functions. 
    public bool isLookingRight;
    public static bool firePower; 
    //sets variables for movement. 
    [SerializeField] float JumpForce;
    [SerializeField] float speed;
    [SerializeField] float GravityMultiplier;
    [SerializeField] float lowerJump; 
    [SerializeField] Animator anim; 
    private Rigidbody2D rb;
    private Transform myTransform; 
    private float moveValue;
    private bool onGround;
    private bool GravityFlip = false; 
    //shooting variables for attack function.
    [SerializeField] float shootTimer; 
    [SerializeField] float rateOfFire;
    [SerializeField] float forceOfShot; 
    [SerializeField] GameObject ball;
    [SerializeField] Transform shotSpawn; 
    private Rigidbody2D instanceOfShot;
    private bool shoot = false;
    float horizontal; 

    private void Awake()
    {
        //makes sure script has ridgidbody component and that mass is set. 
        rb = GetComponent<Rigidbody2D>();
        rb.GetComponent<Rigidbody2D>();
        rb.mass = 1.0f;
        myTransform = GetComponent<Transform>(); 
    }

    private void Attack()
    {
        // if they click they will fire a ball at the spawnpoint. 
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject fireBall = Instantiate(ball, shotSpawn.position, Quaternion.identity);
            instanceOfShot = fireBall.GetComponent<Rigidbody2D>(); 
            //adds force to the ball based on where the player is facing. 
            //Note: requires further bug fixes. 
            if(isLookingRight)
            {
                instanceOfShot.AddForce(shotSpawn.right * forceOfShot, ForceMode2D.Impulse); 
            }
            else
            {
                instanceOfShot.AddForce(-shotSpawn.right * forceOfShot, ForceMode2D.Impulse); 
            }
            //sets shoot bool to true then uses a IEnumerator called one second timer to make sure animation stops. 
            anim.SetBool("shoot", true);
            shoot = true; 
            if(shoot)
            {
                OneSecondTimer();
                shoot = false;
                anim.SetBool("shoot", false); 
            }
        }
    }
    //flips character based on where the player faces. 
    void Flip(float horizontal)
    {
        if (horizontal > 0 && !isLookingRight || horizontal < 0 && isLookingRight)
        {
            isLookingRight = !isLookingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale; 
        }
    }
    //if player colldes with ground set animator bool and script bool to true. 
    private void OnCollisionEnter2D(Collision2D c)
    {
        if(c.gameObject.tag == "Ground")
        {
            onGround = true;
            anim.SetBool("onGround", true); 
        }
        //loads gameOver scene when the player dies. 
        if(c.gameObject.tag == "Death")
        {
            //I will be adding a life counter later
            Debug.Log("collided with death"); 
            SceneManager.LoadScene("GameOver"); 
        }
        //sets the firepower attack bool true. 
        if(c.gameObject.tag == "StrawBerry")
        {
            firePower = true; 
        }
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        //if the player collides with RGravity they will flip upside down allowing the to walk upside down. 
        //gravity will also be pushing the player up instead of down. 
       if(c.gameObject.tag == "RGravity")
        {
            myTransform.Rotate(0, 0, 540);
            rb.gravityScale = -1;
            GravityFlip = true;
        }
       //Ngravity will flip the player back to normal as well as set gravity back to normal. 
       if(c.gameObject.tag == "NGravity")
        {
            myTransform.Rotate(0, 0, -540);  
            rb.gravityScale = 1;
            GravityFlip = false; 
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.tag == "RGravity")
        {
            Flip(-horizontal); 
        }
        if(col.gameObject.tag == "NGravity")
        {
            Flip(horizontal); 
        }
    }

    private IEnumerator OneSecondTimer()
    {
        yield return new WaitForSeconds(1);
    }
    //on ground bools are always true when the player is onGround. 
    private void OnCollisionStay2D(Collision2D collision)
    {
        onGround = true;
        anim.SetBool("onGround", true); 
    }
    //on ground bools are flase if player leaves the ground.
    private void OnCollisionExit2D(Collision2D c)
    {
        if(c.gameObject.tag == "Ground")
        {
            onGround = false;
            anim.SetBool("onGround", false); 
        }
    }

    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        if(!GravityFlip)
        {
            Flip(horizontal); 
        }
        else
        {
            Flip(-horizontal); 
        }
        moveValue = Input.GetAxisRaw("Horizontal");
        //Debug.Log(moveValue); 
        if (onGround)
        {
            //controls attack.
            if (Time.timeScale == 1.0f && firePower == true)
            {
                Attack();
            }

            anim.SetBool("onGround", true); 
            //controls movement. 
            if(Input.GetButtonDown("Jump") && !GravityFlip)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.up * JumpForce;
                Debug.Log(JumpForce); 
            }
            else if(Input.GetButtonDown("Jump") && GravityFlip)
            {
                GetComponent<Rigidbody2D>().velocity = -Vector2.up * JumpForce;
                Debug.Log("upsidedown jump"); 
            }
        }
        if (rb)
        {
            rb.velocity = new Vector2(moveValue * speed, rb.velocity.y);
            anim.SetFloat("Moveing", Mathf.Abs(moveValue)); 
        }
        else
        {
            Debug.Log("this script does not have the rb");
        }

    }
}