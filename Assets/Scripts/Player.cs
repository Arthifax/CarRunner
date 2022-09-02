using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject playerSpark;
    [Header("Player Physics")]
    public float gravity;
    public float acceleration = 10;
    public float maxAcceleration = 10;
    public Vector2 velocity;
    public float jumpVelocity = 20;
    public float maxXVelocity = 100;
    [Header("Checks")]
    public float groundHeight = 0;
    [SerializeField] bool isGrounded = false;
    [SerializeField] bool onRails = false;
    [SerializeField] private bool isHoldingJump = false;
    [SerializeField] public float maxHoldJumpTime = 0.4f;
    public float holdJumpTimer = 0.0f;
    public float jumpGroundThreshhold = 1;
    public float distance = 0;

    private void Awake()
    {
        playerSpark = GameObject.Find("PlayerSparkVFX");
    }

    void Update()
    {
        Vector2 pos = transform.position;
        float groundDistance = Mathf.Abs(pos.y - groundHeight);

        //Check if player is grounded OR close to the ground
        if (isGrounded || groundDistance <= jumpGroundThreshhold)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isGrounded = false;
                onRails = false;
                velocity.y = jumpVelocity;
                isHoldingJump = true;
                holdJumpTimer = 0;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isHoldingJump = false;
        }
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;

        if (!isGrounded)
        {
            if (isHoldingJump)
            {
                holdJumpTimer += Time.fixedDeltaTime;
                if (holdJumpTimer >= maxHoldJumpTime)
                {
                    isHoldingJump = false;
                }
            }
            
            pos.y += velocity.y * Time.fixedDeltaTime;
            if (!isHoldingJump)
            {
                velocity.y += gravity * Time.fixedDeltaTime;
            }
            
            //Raycasting components
            Vector2 rayOrigin = new Vector2(pos.x + 0.7f, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
            if (hit2D.collider != null)
            {
                Ground ground = hit2D.collider.GetComponent<Ground>();
                if (ground != null)
                {
                    groundHeight = ground.groundHeight;
                    pos.y = groundHeight;
                    isGrounded = true;
                }

                if (hit2D.collider.tag == "rail" && onRails == false)
                {
                    onRails = true;
                }
                else
                {
                    onRails = false;
                }
            }
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);
        }

        //only gain speed when on the ground
        if (isGrounded)
        {
            //accelerate slower over time
            float velocityRatio = velocity.x / maxXVelocity;
            acceleration = maxAcceleration * (1 - velocityRatio);
            
            velocity.x += acceleration * Time.fixedDeltaTime;
            if (velocity.x >= maxXVelocity)
            {
                velocity.x = maxXVelocity;
            }
            
            //Raycasting components
            Vector2 rayOrigin = new Vector2(pos.x - 0.7f, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
            if (hit2D.collider == null)
            {
                isGrounded = false;
            }
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.yellow);
        }

        if (onRails == true)
        {
            playerSpark.SetActive(true);
        }
        else
        {
            playerSpark.SetActive(false);
        }

        //count distance traveled over time
        distance += velocity.x * Time.fixedDeltaTime;
        
        transform.position = pos;
    }

    /*private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("ground"))
        {
            if (isGrounded == false)
            {
                isGrounded = true;
            }
        }
    }*/
}
