using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rigid;
    public EntityGeneral entityGeneral;
    private float inputHVec;

    public bool canHmove = true;
    public bool canJump = true;
    public bool canLadder = true;
    public float fallAssistStr = 2f;
    public float airControl = 0.5f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public float ladderClimbSpeed = 0.5f;
    public bool flying = false;

    float velControl = 1f;
    float bufferTimer = 0f;
    bool climbingLadder = false;

    public float accelTime = 2f;
    public float decelTime = 2f;
    public float maxSpeed = 5f;

    public float jumpForce = 400f;

    void Start()
    {
        rigid = this.GetComponent<Rigidbody2D>();
        entityGeneral = this.GetComponent<EntityGeneral>();
    }

    void Update()
    {
        inputHVec = Input.GetAxisRaw("Horizontal");
        FallAsist();
        if (canJump && !flying)
        {
            Jumping();
        }
        if (flying)
        {
            Flying();
        }
        if (canLadder)
        {
            LadderDetection();
            LadderClimbing();
        }
        if (bufferTimer > 0f)
        {
            bufferTimer -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Z)) //should not be here
        {
            AstarPath.active.Scan();
        }
    }

    void FixedUpdate()
    {
        if (canHmove)
        {
            HorizontalMove();
        }
    }
    void HorizontalMove()
    {
        float speedMulter = entityGeneral.entityStatusEffects.GetEntitySpeed();
        //Acceleration
        float desiredSpeed = (inputHVec*maxSpeed*speedMulter);
        if ((inputHVec > 0f && rigid.velocity.x < desiredSpeed) || (inputHVec < 0f && rigid.velocity.x > desiredSpeed))
        {
            float forceToApply = rigid.mass*(maxSpeed*speedMulter/(accelTime*speedMulter*velControl))*inputHVec;
            rigid.AddForce(new Vector2(forceToApply,0f));
        }
        //Deceleration
        if (inputHVec == 0f && entityGeneral.entityStopDecel <= 0f)
        {
            float forceToApply = rigid.mass*(-rigid.velocity.x/(decelTime*velControl));
            rigid.AddForce(new Vector2(forceToApply,0f));
        }
    }
    void Flying() //A bit of repeated code, we might change this.
    {
        float speedMulter = entityGeneral.entityStatusEffects.GetEntitySpeed();
        //rigid.gravityScale = 0f; // DO NOT KEEP THIS!!
        float inputVVec = Input.GetAxisRaw("Vertical");
        //Acceleration
        if ((inputVVec > 0f && rigid.velocity.y < (inputVVec*maxSpeed*speedMulter)) || (inputVVec < 0f && rigid.velocity.y > (inputVVec*maxSpeed*speedMulter)))
        {
            float forceToApply = rigid.mass*(maxSpeed*speedMulter/(accelTime*speedMulter*velControl))*inputVVec;
            rigid.AddForce(new Vector2(0f,forceToApply));
        }
        //Deceleration
        if (inputVVec == 0f && entityGeneral.entityStopDecel <= 0f)
        {
            float forceToApply = rigid.mass*(-rigid.velocity.y/(decelTime*velControl));
            rigid.AddForce(new Vector2(0f,forceToApply));
        }
    }
    void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.W))// || Input.GetAxisRaw("Horizontal") > 0)
        {
            bufferTimer = jumpBufferTime; //This does the jumping
        }
        if (bufferTimer > 0f && (entityGeneral.onGround || (entityGeneral.airTime <= coyoteTime && rigid.velocity.y<0f)))
        {
            ExecuteJump(); //Jump!
            bufferTimer = 0f;
        }

        if (Input.GetKeyUp(KeyCode.W) && rigid.velocity.y > 0f) //PullDown when let go of jump
        {
            rigid.velocity = new Vector2(rigid.velocity.x,rigid.velocity.y/1.5f);
        }

        if (entityGeneral.onGround == false) //Turn down air control on air
        {
            velControl = 1f/airControl;
        }
        else
        {
            velControl = 1f;
        }
    }
    public void ExecuteJump()
    {
        rigid.velocity = new Vector2(rigid.velocity.x,0f); //removes the corner-jump
        rigid.AddForce(new Vector2(0f,jumpForce*entityGeneral.entityStatusEffects.GetEntityJumpBoost()));
    }
    void FallAsist()
    {
        if (rigid.velocity.y<0f && rigid.velocity.y>-6f)//PullDown
        {
            rigid.AddForce(new Vector3(0f,-fallAssistStr,0f));
        }
    }
    void LadderDetection()
    {
        Vector3Int myIntPos = entityGeneral.GetGridPos();
        if (entityGeneral.mapManager.HasBlock(myIntPos))
        {
            if (entityGeneral.mapManager.GetBlock(myIntPos).blockData.tags.Contains("climable"))
            {
                climbingLadder = true;
            }
            else if (climbingLadder)
            {
                StopClimbing();
            }
        }
        else if (climbingLadder)
        {
            StopClimbing();
        }
    }
    void StopClimbing()
    {
        climbingLadder = false;
        if (Input.GetKey(KeyCode.W) && canJump)
        {
            bufferTimer = jumpBufferTime;//Jump
            ExecuteJump();
        }
    }
    void LadderClimbing()
    {
        if (climbingLadder && canLadder)
        {
            rigid.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            bool canClimbUp = CanClimbToHere(this.transform.position + new Vector3(0f,0.5f,0f));
            bool canClimbDown = CanClimbToHere(this.transform.position + new Vector3(0f,-0.5f,0f));
            entityGeneral.airTime = 0f;
            if (Input.GetKey(KeyCode.W) && canClimbUp) //no col block above, is added as a condition
            {
                this.transform.position += new Vector3(0f,ladderClimbSpeed*Time.deltaTime,0f); //climb up
            }
            if (Input.GetKey(KeyCode.S) && canClimbDown)
            {
                this.transform.position -= new Vector3(0f,ladderClimbSpeed*Time.deltaTime,0f); //Climb down
            }
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        }
    }
    bool CanClimbToHere(Vector3 testPosFloat)
    {
        Vector3Int testPos = new Vector3Int(Mathf.RoundToInt(testPosFloat.x),Mathf.RoundToInt(testPosFloat.y),0);
        if (entityGeneral.mapManager.HasBlock(testPos))
        {
            if (entityGeneral.mapManager.GetBlock(testPos).blockData.hasCollision)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }
}
