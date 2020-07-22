using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour {

    //public variable for setting the speed of the cube
    private float speed;
    private float initSpeed;

    private bool gotClose;
    
    private float maxSpeed;

    //Position that the cub will aim for
    Vector3 targetPos;
    GameObject targetObject;

    //Reference to game manager script
    GameManager gameManager;

    //Checks if cube is currently moving
    bool moving;

    Rigidbody2D body;

    enum CubeState { moving, waiting ,sliding};
    CubeState state;
    bool isEating;
    bool isHolding;

    GameObject toDestroy;
    GameObject toPickup;

    int waitCount, waitTimer;
    float changeTimer;
    int changeTimeMax;
    //Screen bounds
    private float minX, maxX, minY, maxY;

    private float slowDist, distError, slowDown;

    public float size, sizeIncrease;

    private float kickForce;

    BoxCollider2D col;


    public float footstepRate = 1.3f;
    public AK.Wwise.Event footstepSound = new AK.Wwise.Event();
    private float walkCount = 0.0f;

    Animator animator;

    // Use this for initialization
    void Start ()
    {
        //Gets the collider componenet
        col = GetComponent<BoxCollider2D>();

        //Gets the animator componenet
        animator = GetComponentInChildren<Animator>();

        //Cube start in the waiting state
        state = CubeState.waiting;
        isEating = false;

        waitCount = 1;
        waitTimer = 0;

        changeTimeMax = 10;
        changeTimer = 0;

        //Gets gamemanager script by searching for the gamemanager objects tag
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        //Hard coded values for cubes movement range
        minX = -9.8f;
        maxX = 9.8f;
        minY = -4.3f;
        maxY = -0.1f;

        //initial size of cube and how much it increases which each mess it eats
        size = (transform.GetChild(0).localScale.x + transform.GetChild(0).localScale.y) / 2.0f;
        sizeIncrease = 0.05f;

        //Set an initial target position
        targetPos = Camera.main.ScreenToWorldPoint(new Vector3(1, 0, 0));

        //rigidbody component
        body = GetComponent<Rigidbody2D>();

        //acceleration of slime movement
        speed = 9.5f;

        initSpeed = speed;
        maxSpeed = 2;

        //how close slime should get to target before slowing down/stopping
        distError = 0.25f;
        slowDist = 1.5f;

        slowDown = 0.005f;

        gotClose = false;

        //how hard the slime should be kicked
        kickForce = 500;
    }
	
	// Update is called once per frame
	void Update()
    {
        //Dont do any update if the game is paused
        if (gameManager.GetState() == GameManager.stateEnum.paused || gameManager.GetTicketPause() || gameManager.GetButtonPause())
        {
            moving = false;
            body.velocity = Vector3.zero;
            return;
        }
        

        UpdateAnimation();
       
        //checks the target position is within the level bounds
        if (targetPos.x > maxX)
        {
            Debug.Log("max check");
            targetPos.x = maxX;
        }
        else if (targetPos.x < minX)
        {
            targetPos.x = minX;
        }
        else if (targetPos.y > maxY)
        {
            targetPos.y = maxY;
        }
        else if (targetPos.y < minY)
        {
            targetPos.y = minY;
        }


        Vector3 moveVec = targetPos - transform.position;
        moveVec.Normalize();

        if (state == CubeState.moving)
        {
            
            walkCount += Time.deltaTime;
            if (walkCount >= footstepRate)
            {
                footstepSound.Post(gameObject);
                walkCount = 0.0f;
            }

            //if slime spends to long trying to get to one position, then pick another
            changeTimer += Time.deltaTime;
            if (changeTimer > changeTimeMax)
            {
                NewDirection();
            }

            //checks if the target object still exists
            if (targetObject != null)
            {
                targetPos = targetObject.transform.position;

                //Forgets target if it is held by the player
                if (targetObject.GetComponent<Item>() && targetObject.GetComponent<Item>().GetHeld())
                {
                    targetObject = null;
                    toPickup = null;
                    state = CubeState.waiting;
                    return;
                }
                else if(targetObject.GetComponent<Item>())
                {
                    targetPos = targetObject.transform.position;
                }
                else
                {
                    targetPos = targetObject.transform.position;
                }
            }

            //moves slime
            body.AddForce(moveVec * speed);

            //keeps slime speed under set max speed
            if (body.velocity.x > maxSpeed)
            {
                body.velocity = new Vector2(maxSpeed, body.velocity.y);
            }
            else if (body.velocity.x < -maxSpeed)
            {
                body.velocity = new Vector2(-maxSpeed, body.velocity.y);
            }
            if (body.velocity.y > maxSpeed)
            {
                body.velocity = new Vector2(body.velocity.x, maxSpeed);
            }
            else if (body.velocity.y < -maxSpeed)
            {
                body.velocity = new Vector2(body.velocity.x, -maxSpeed);
            }

            //if (almost) at target position then stop moving
            if (Vector3.Distance(transform.position, targetPos) < slowDist)
            {
                if(!gotClose)
                {
                    body.velocity = Vector3.zero;
                    gotClose = true;
                }

                if(Vector3.Distance(transform.position, targetPos) < distError)
                {
                    body.velocity = Vector3.zero;
                    state = CubeState.waiting;
                    transform.SetPositionAndRotation(targetPos, transform.rotation);
                    speed = initSpeed;
                    gotClose = false;
                }
                
            }
            else if(gotClose)
            {
                body.velocity = Vector3.zero;
                speed = initSpeed;
                gotClose = false;
            }
        }

        //keep sliding until it slows down enough
        else if(state == CubeState.sliding)
        {
            body.velocity /= 1.01f;
            if (body.velocity.magnitude < 0.5f)
            {
                NewDirection();
            }
        }

        //if slime isnt moving or sliding
        else
        {
            changeTimer = 0;
            waitTimer++;
            //continue waiting
            if (waitTimer <= waitCount)
            {
                return;
            }

            waitTimer = 0;

            //checks if nothing is currently held
            if(transform.childCount == 0)
            {
                isHolding = false;
            }

            //destroys mess and increases size
            if (toDestroy != null)
            {
                Destroy(toDestroy);

                UpdateScale();
            }

            //picks up an ingredient
            else if (toPickup != null)
            {
                toPickup.transform.parent = transform;
                toPickup.transform.localPosition = Vector3.zero;
                toPickup.GetComponent<Item>().SetHeld(true);
                isHolding = true;
                targetObject = null;
                toPickup = null;
            }

            GameObject mess;
            if (!isHolding)
            {
                GameObject[] ingredients;
                ingredients = GameObject.FindGameObjectsWithTag("Holdable");
                foreach (GameObject i in ingredients)
                {
                    if (!i.GetComponent<Item>().GetHeld())
                    {
                        targetObject = i;
                    }
                }
            }
            mess = GameObject.FindGameObjectWithTag("Mess");

            if(targetObject != null)
            {
                targetPos = targetObject.transform.position;
                toPickup = targetObject;
            }
            else if (mess != null)
            {
                targetObject = mess;
                
                targetPos = targetObject.transform.position;
                toDestroy = mess;
            }

            else
            {
                //If not moving then set a target position within the specified ranges and start moving again
                targetPos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
            }
            state = CubeState.moving;
        }
    }

    private void NewDirection()
    {
        changeTimer = 0;
        state = CubeState.waiting;
        body.velocity = Vector3.zero;
        targetObject = null;
        toPickup = null;
        toDestroy = null;
    }

    private void BounceX()
    {
        body.AddForce(new Vector2(-body.velocity.x * 2, 0));
    }
    private void BounceY()
    {
        body.AddForce(new Vector2(0, -body.velocity.y * 2));
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            changeTimer = 0;
            state = CubeState.sliding;
            targetObject = null;
            toPickup = null;
            toDestroy = null;
            body.AddForce((transform.position - col.gameObject.transform.position).normalized * kickForce);
        }
        else if (col.gameObject.tag == "X")
        {
            BounceX();
        }
        else if (col.gameObject.tag == "Y")
        {
            BounceY();
        }
        else if (col.gameObject.tag == "Chopping")
        {
            BounceX();
            BounceY();
            NewDirection();
        }
        else if(state == CubeState.moving)
        {

        }
    }

    void UpdateAnimation()
    {
        if (state != CubeState.moving)
        {
            //animator.Play("FrontSlime");
        }

        //CODE UPDATED TO ONLY SHOW FRONT AND BACK ANIMATIONS

        //if (Mathf.Abs((currentTarget - transform.position).x) > Mathf.Abs((currentTarget - transform.position).y))
        //else if(false)// (Mathf.Abs((targetPos - transform.position).x) > Mathf.Abs((targetPos - transform.position).y))
        //{
        //    if (targetPos.x > transform.position.x && !animator.GetCurrentAnimatorStateInfo(0).IsName("RightSlime"))
        //    {
        //        //Moving Right
        //        animator.Play("RightSlime");
        //    }
        //    else if(targetPos.x <= transform.position.x && !animator.GetCurrentAnimatorStateInfo(0).IsName("LeftSlime"))
        //    {
        //        //Moving Left
        //        animator.Play("LeftSlime");
        //    }
        //}
        else
        {
            if (targetPos.y > transform.position.y && !animator.GetCurrentAnimatorStateInfo(0).IsName("BackSlime"))
            {
                //Moving Up
                animator.Play("BackSlime");
            }
            else if(targetPos.y <= transform.position.y && !animator.GetCurrentAnimatorStateInfo(0).IsName("FrontSlime"))
            {
                //Moving Down
                animator.Play("FrontSlime");
            }
        }
    }

    void UpdateScale()
    {
        size += sizeIncrease;
        transform.GetChild(0).localScale = new Vector3(size, size, 1);
    }

}
