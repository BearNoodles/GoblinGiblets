using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PlayerScript : MonoBehaviour {

    Vector3 targetPos;
    Vector3 moveVec;
    Vector3 startPos;
    Quaternion startRot;
    public float speed;

    GameManager gameManager;

    Pathfinding pathfinding;
    List<Vector3> pathTargets;
    Vector3 currentTarget;

    Animator animator;

    GameObject heldItem;
    private Vector3 itemOffsetD, itemOffsetL, itemOffsetU, itemOffsetR ;
    private Vector3 itemScale;
    GameObject pickupTarget;
    GameObject touchedStation;
    bool holding;
    bool dropReady;
    bool pickupReady;
    public bool PickupReady { set { pickupReady = value; } }

    bool right, up;

    bool moving;

    public float footstepRate = 0.3f;
    public AK.Wwise.Event footstepSound = new AK.Wwise.Event();
    private float walkCount = 0.0f;

    public AK.Wwise.Event moveClickSound = new AK.Wwise.Event();
    public AK.Wwise.Event pickupSound = new AK.Wwise.Event();

    private float minX, maxX, minY, maxY;
    
    
    private bool moveDelay;
    public bool MoveDelay { set { moveDelay = value; } }

    private bool isTouchingChoppingBoard;
    private bool isTouchingOven;
    private bool isTouchingBlender;
    private bool isTouchingWindow;

	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
        startPos = new Vector3(0, 0, 0);
        startRot = Quaternion.identity;
        moving = false;

        holding = false;
        pickupReady = false;
        dropReady = false;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        //positions for items to appear when held
        itemOffsetD = new Vector3(0.3f, -0.3f, 0.0f);
        itemOffsetL = new Vector3(-0.3f, -0.15f, 0.0f);
        itemOffsetU = new Vector3(-0.3f, -0.3f, 0.0f);
        itemOffsetR = new Vector3(0.3f, -0.15f, 0.0f);
        itemScale = new Vector3(0.05f, 0.05f, 0.05f);

        pathfinding = GetComponent<Pathfinding>();
        pathTargets = new List<Vector3>();
        currentTarget = new Vector3();

        //Hard coded values to set as the players target position
        minX = -9.7f;
        maxX = 9.7f;
        minY = -4.3f;
        maxY = 1.1f;
        

        moveDelay = false;

        isTouchingChoppingBoard = false;
        isTouchingOven = true;
        isTouchingBlender = false;
        isTouchingWindow = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Dont do any update if the game is paused
        if (gameManager.GetState() == GameManager.stateEnum.paused || gameManager.GetTicketPause() || gameManager.GetButtonPause())
        {
            moving = false;
            SetIdleAnimation();
            return;
        }

        //1 frame delay to prevent moving after ticket moves away
        if(moveDelay)
        {
            moveDelay = false;
            moving = false;
            return;
        }

        // footsteps
        if (moving)
        {
            walkCount += Time.deltaTime;
            if (walkCount >= footstepRate)
            {
                footstepSound.Post(gameObject);
                walkCount = 0.0f;
            }
        }

        //Set the max and min target position inside the set bounds
        if (targetPos.x > maxX)
        {
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

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        if (Input.GetMouseButtonUp(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //MoveToLocation();
                targetPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

                moveClickSound.Post(gameObject);

                moving = true;
            }
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 touchPos = Input.GetTouch(0).position;
                targetPos = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, 10));
        
                moveClickSound.Post(gameObject);

                moving = true;
            }
        }
#endif

        if (moving == true)
        {
            transform.SetPositionAndRotation(Vector3.MoveTowards(transform.position, targetPos, speed), startRot);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                moving = false;
                if (heldItem != null)
                {
                    heldItem.transform.localPosition = itemOffsetD;

                    heldItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
                }
                
                transform.SetPositionAndRotation(targetPos, startRot);
            }
        }
        if (Input.touches.Length > 0)
        {
            //MoveToLocation();
        }

        if (!moving)
        {
            SetIdleAnimation();
        }
        
        else if (Mathf.Abs((targetPos - transform.position).x) > Mathf.Abs((targetPos - transform.position).y))
        {
            if (targetPos.x > transform.position.x)
            {
                //Moving Right

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("WalkR"))
                {
                    animator.Play("WalkR");
                    if (heldItem != null)
                    {
                        heldItem.transform.localPosition = itemOffsetR;

                        heldItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
                    }
                }
            }
            else
            {
                //Moving Left
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("WalkL"))
                {
                    animator.Play("WalkL");
                    if (heldItem != null)
                    {
                        heldItem.transform.localPosition = itemOffsetL;

                        heldItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
                    }
                }
            }
        }
        else
        {
            if (targetPos.y > transform.position.y)
            {
                //Moving Up
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("WalkU"))
                {
                    animator.Play("WalkU");
                    if (heldItem != null)
                    {
                        heldItem.transform.localPosition = itemOffsetU;

                        heldItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
                    }
                }
            }
            else
            {
                //Moving Down

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("WalkD"))
                {
                    animator.Play("WalkD");
                    if (heldItem != null)
                    {
                        heldItem.transform.localPosition = itemOffsetD;

                        heldItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
                    }
                }
            }
        }
        
    }

    private void SetIdleAnimation()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("WalkU"))
        {
            animator.Play("IdleU");
            if (heldItem != null)
            {
                heldItem.transform.localPosition = itemOffsetU;

                heldItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("WalkD"))
        {
            animator.Play("IdleD");
            if (heldItem != null)
            {
                heldItem.transform.localPosition = itemOffsetD;
                heldItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("WalkR"))
        {
            animator.Play("IdleR");
            if (heldItem != null)
            {
                heldItem.transform.localPosition = itemOffsetR;

                heldItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("WalkL"))
        {
            animator.Play("IdleL");
            if (heldItem != null)
            {
                heldItem.transform.localPosition = itemOffsetL;

                heldItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
            }
        }
    }

    public void MoveToLocation()
    {
        transform.SetPositionAndRotation(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), startRot);
    }

    //Sets item transform to player transform
    void PickupItem(GameObject item)
    {

        item.transform.parent = transform;
        item.transform.position = transform.position;
        heldItem = item;
        itemScale = heldItem.transform.localScale / 1.6f;
        heldItem.transform.localPosition = itemOffsetD;
        heldItem.transform.localScale = itemScale;
        heldItem.GetComponent<Collider2D>().enabled = false;
        holding = true;
        heldItem.GetComponent<Item>().SetHeld(true);

        pickupSound.Post(gameObject);

        IngredientTracker.CurrentIngredient = heldItem.GetComponent<Item>().Type;
        IngredientTracker.CurrentIngredientSprite = heldItem.GetComponentInChildren<SpriteRenderer>().sprite;

        if (heldItem.GetComponent<Item>().Type == MenuScript.IngredientType.dish)
        {
            gameManager.OpenWindow();
        }

        int targetStation = 0;

        bool stationGlow = true;
        bool ingredientGlow = false;

        if (gameManager.CheckStation(1, heldItem.GetComponent<Item>().Type))
        {
            targetStation = 1;
           
            if (isTouchingChoppingBoard)
            {
                stationGlow = false;
                ingredientGlow = true;
                DropItem(heldItem);
                gameManager.StationUsed(1, heldItem.GetComponent<Item>().Type);
                gameManager.PauseScene();
            }
        }

        else if (gameManager.CheckStation(0, heldItem.GetComponent<Item>().Type))
        {
            targetStation = 0;
            
            if (isTouchingOven)
            {
                stationGlow = false;
                ingredientGlow = true;
                DropItem(heldItem);
                gameManager.StationUsed(0, heldItem.GetComponent<Item>().Type);
                gameManager.PauseScene();
            }
             
        }

        else if (gameManager.CheckStation(2, heldItem.GetComponent<Item>().Type))
        {
            targetStation = 2;

            if (isTouchingBlender)
            {
                stationGlow = false;
                ingredientGlow = true;
                DropItem(heldItem);
                gameManager.StationUsed(2, heldItem.GetComponent<Item>().Type);
                gameManager.PauseScene();
            }
        }

        else if (gameManager.CheckStation(3, heldItem.GetComponent<Item>().Type))
        {
            targetStation = 3;

            if (isTouchingWindow)
            {
                stationGlow = false;
                ingredientGlow = true;
                DropItem(heldItem);
                gameManager.StationUsed(3, heldItem.GetComponent<Item>().Type);
                gameManager.PauseScene();
            }
        }

        gameManager.SetIngredientsGlow(ingredientGlow);
        gameManager.SetStationGlow(targetStation, stationGlow);
        
    }

    //sets transform of held item to station and destroys. Not sure which way to do this yet
    void DropItem(GameObject station)
    {
        heldItem.GetComponent<Item>().SetHeld(false);
        heldItem.transform.parent = station.transform;
        heldItem.transform.position = station.transform.position;
        holding = false;
        Destroy(heldItem.transform.gameObject);
        dropReady = false;
        pickupTarget = null;

        gameManager.SetIngredientsGlow(true);

    }

    public void SetPickupReady()
    {
        pickupReady = true;
    }
    public void SetDropReady()
    {
        dropReady = true;
    }

    //Checks collisions
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Holdable" && !holding)
        {
            PickupItem(col.gameObject);
        }
        else if (col.gameObject.tag == "Oven")
        {
            isTouchingOven = true;
            if (holding && gameManager.CheckStation(0, heldItem.GetComponent<Item>().Type))
            {
                DropItem(col.gameObject);
                gameManager.StationUsed(0, heldItem.GetComponent<Item>().Type);
                gameManager.SetStationGlow(0, false);
                gameManager.PauseScene();
            }
            //
        }
        else if (col.gameObject.tag == "Chopping")
        {
            isTouchingChoppingBoard = true;
            moving = false;
            SetIdleAnimation();
            transform.SetPositionAndRotation(Vector3.MoveTowards(transform.position, -targetPos, speed * 5), startRot);
            if (holding && gameManager.CheckStation(1, heldItem.GetComponent<Item>().Type))
            {
                DropItem(col.gameObject);
                gameManager.StationUsed(1, heldItem.GetComponent<Item>().Type);
                gameManager.SetStationGlow(1, false);
                gameManager.PauseScene();

            }
        }
        else if (col.gameObject.tag == "Blender")
        {
            isTouchingBlender = true;
            if (holding && gameManager.CheckStation(2, heldItem.GetComponent<Item>().Type))
            {
                DropItem(col.gameObject);
                gameManager.StationUsed(2, heldItem.GetComponent<Item>().Type);
                gameManager.SetStationGlow(2, false);
                gameManager.PauseScene();

            }
        }
        else if (col.gameObject.tag == "WindowS")
        {
            isTouchingWindow = true;
            if (holding && gameManager.CheckStation(3, heldItem.GetComponent<Item>().Type))
            {
                DropItem(col.gameObject);
                gameManager.StationUsed(3, heldItem.GetComponent<Item>().Type);
                gameManager.SetStationGlow(3, false);
                gameManager.PauseScene();
                ResetPlayer();
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Chopping")
        {
            isTouchingChoppingBoard = false;
        }
        else if (col.gameObject.tag == "Oven")
        {
            isTouchingOven = false;
        }
        else if (col.gameObject.tag == "Blender")
        {
            isTouchingBlender = false;
        }
        else if (col.gameObject.tag == "WindowS")
        {
            isTouchingWindow = false;
        }
    }

    public void ResetPlayer()
    {
        transform.SetPositionAndRotation(startPos, startRot);
    }

    public bool GetHolding()
    {
        return holding;
    }
    

}
