using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script is attatched to the catapult object in the serving minigame, and is where most or all of the work happens for it.
// the trajectory point code is adapted from http://www.theappguruz.com/blog/display-projectile-trajectory-path-in-unity#more-601
public class CatapultScript : MonoBehaviour {

    // balancing variables
    [Header("Balancing Variables")]
    public float pointTimeInterval = 0.05f; // gap between trajectory points
    public int numOfTrajectoryPoints = 15; // how many points are displayed
    public float power = 25; // power the food is launched with
    public float minimumDragDistance = 0.4f; // min amount the touch needs to be dragged to fire, to avoid accidental fire on tapping screen
    public int shotsToFire = 3; // how many shots are fired before the game ends, the best single score is used

    // attatched objects
    [Header("Dont touch this.")]
    public GameObject TrajectoryPointPrefab;
    public GameObject ProjectileFoodPrefab;
    public GameObject ProjectileHolder;
    //public GameObject targetTable;
    public GameObject table1;
    public GameObject table2;
    public GameObject table3;

    // Public objects for the gameover UI
    public GameObject gameOverPanel;
    public Text gameOverText;
    
    private GameObject food; // the object to be fired
    private List<GameObject> trajectoryPoints; // List of points to be displayed

    private GameObject targetTable; // selected target table

    // track state of game
    private bool isPressed, isFoodFired; 
    private bool gameFinished = false;

    private int shotsComplete = 0;
    private int topScore = 0;

    public bool tutorialActive = false;

    // for starting the touch anywhere, not needing to start the touch on the catapult
    private Vector3 touchStartPos;

    // the target score changes based on difficulty
    int targetScore;

    // objects in scene to be moved by the script
    public GameObject dummyFood; // food in catapult
    public GameObject dummyElastic; // elastic of catapult

    public GameObject arrow; // arrow above target table

    // sounds
    public AK.Wwise.Event tensionSound = new AK.Wwise.Event();
    public AK.Wwise.Event releaseSound = new AK.Wwise.Event();

    // Use this for initialization
    void Start () {

        // save functionality test - remove later
        SaveLoad.initialiseValues();

        // target score is based on difficulty, if this is being played in the editor 
        // without going through level select then 100 is used as the target score
        if (MinigameScores.ScoreTarget > 0)
        {
            // score target is for a whole meal, a single minigame is 1/4 of that
            targetScore = MinigameScores.ScoreTarget / 4;
        } else
        {
            targetScore = 100;
        }

        // list for points
        trajectoryPoints = new List<GameObject>();
        // these start false
        isPressed = isFoodFired = false;

        // set target table, 1, 2 or 3
        int choice = Random.Range(1, 4);
        if (choice == 1) { targetTable = table1; }
        if (choice == 2) { targetTable = table2; }
        if (choice == 3) { targetTable = table3; }

        // position arrow over table
        arrow.transform.position = targetTable.transform.position + new Vector3(0, 2, 0);

        // Hide the game over panel
        gameOverPanel.SetActive(false);

        // instantiate points
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            GameObject point = (GameObject)Instantiate(TrajectoryPointPrefab, gameObject.transform); // create point
            point.GetComponent<Renderer>().enabled = false; // dont display it
            trajectoryPoints.Insert(i, point); // add to list
        }
	}
	
	// Update is called once per frame
	void Update () {
        // no gameplay on tutorial screen
        if (tutorialActive)
        {
            return;
        }

        // check to see if the minigame is finished and if the food has been fired and it has stopped moving.  
        // It is possible that this should be checked for more than one frame of no velocity, no problems seen yet in game
        if (isFoodFired && food.GetComponent<Rigidbody2D>().velocity == new Vector2(0.0f, 0.0f) && !gameFinished)
        {
            Score(); // record the shots score
            shotsComplete++; // increment number of shots taken
            if (shotsComplete < shotsToFire) // check whether more shots need to be taken
            {
                Reload(); // if so prep for next shot
                return;
            }

            // get target area
            Collider2D targetArea = targetTable.transform.GetChild(0).GetComponent<Collider2D>();

            Victory();
            gameFinished = true;
        }


        // if the food has been fired nothing after this in the function needs to be run
        if (isFoodFired || gameFinished)
        {
            return;
        }

        // when the drag is started
        if (Input.GetMouseButtonDown(0))
        {
            isPressed = true;
            touchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        // if the drag is not being held
        else if (Input.GetMouseButtonUp(0))
        {
            isPressed = false;

            // if drag not long enough to trigger shot dont fire
            if (Vector3.Distance(touchStartPos, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < minimumDragDistance)
            {
                return;
            }

            // if the food has not been fired
            if (!isFoodFired)
            {
                fireFood();
            }
        }

        // if drag is being held, draw trajectory points
        if (isPressed)
        {
            // check distance of drag, if the drag is very short the shot wont fire so hide the points
            if (Vector3.Distance(touchStartPos, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < minimumDragDistance)
            {
                HideTrajectoryPoints();
                return;
            }

            // spawn food if it hasnt been yet
            if (!food)
            {
                spawnFood();

                // play tension sound
                tensionSound.Post(gameObject);
            }

            // get velocity to fire food at from drag distance
            Vector3 vel = GetForceFrom(touchStartPos, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            // get angle
            float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
            // rotate this object (catapult elastic and food in it)
            transform.eulerAngles = new Vector3(0, 0, angle);
            // position the trajectory points
            setTrajectoryPoints(transform.position, vel / food.GetComponent<Rigidbody2D>().mass);

            // elastic stretch
            // position food
            dummyFood.transform.position = (transform.position - vel / 5);
            // position elastic
            dummyElastic.transform.position = transform.position - vel / 10;
            // stretch the elastic
            dummyElastic.GetComponent<SpriteRenderer>().size = new Vector2(vel.magnitude / 5, dummyElastic.GetComponent<SpriteRenderer>().size.y);
        }
        // once fired stop drawing
        else if (isFoodFired)
        {
            HideTrajectoryPoints();
        }
        
    }

    // prep for the next shot
    private void Reload()
    {
        isFoodFired = false; // reset bool
        food = null; // clear the reference to the previous food fired

        transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f); // reset catapult rotation
        dummyElastic.GetComponent<SpriteRenderer>().size = new Vector2(0.0f, dummyElastic.GetComponent<SpriteRenderer>().size.y); // reset elastic stretch
        dummyFood.transform.position = (transform.position); // reset food displayed in catapult position

        dummyFood.SetActive(true); // display food again
        dummyElastic.SetActive(true); // display elastic again
    }

    private void HideTrajectoryPoints()
    {
        // loop through points
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            trajectoryPoints[i].GetComponent<Renderer>().enabled = false; // dont display it
        }
    }

    // spawn food object from prefab
    private void spawnFood()
    {
        //food = (GameObject)Instantiate(ProjectileFoodPrefab, gameObject.transform); // instantiate as child of catapult
        food = (GameObject)Instantiate(ProjectileFoodPrefab, ProjectileHolder.transform); // instantiate as not child of catapult?
        Vector3 pos = transform.position; // get catapults position
        food.transform.position = pos; // set position to catapults
        
        food.SetActive(false); // dont display yet
    }

    // fire the food
    private void fireFood()
    {
        food.transform.rotation = transform.rotation * Quaternion.Euler(0.0f, 0.0f, 270.0f); // rotate sprite to face the way it will be going
        food.SetActive(true); // display it
        food.GetComponent<Rigidbody2D>().gravityScale = 1; // set gravity scale (needs to match whats used by the trajectory points)
        // fire in correct direction
        food.GetComponent<Rigidbody2D>().AddForce(GetForceFrom(touchStartPos, Camera.main.ScreenToWorldPoint(Input.mousePosition)), ForceMode2D.Impulse);

        isFoodFired = true; // state tracking bool

        // hide elastic and food in catapult
        dummyFood.SetActive(false);
        dummyElastic.SetActive(false);
        
        // play release sound
        releaseSound.Post(gameObject);
    }

    // get force of shot based on length of drag
    private Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos)
    {
        return -(new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power;
    }

    // position trajectory points
    void setTrajectoryPoints(Vector3 pStartPosition, Vector3 pVelocity)
    {
        float velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));
        float angle = Mathf.Rad2Deg * (Mathf.Atan2(pVelocity.y, pVelocity.x));
        float fTime = 0;

        fTime += pointTimeInterval; // draw points as if a certain amount of time has passed after being fired
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            // determine position of point after amount of time
            float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad); // x position after time
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude * fTime * fTime / 2.0f); // y position after time
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 2);
            trajectoryPoints[i].transform.position = pos; // position point
            trajectoryPoints[i].GetComponent<Renderer>().enabled = true; // draw point
            // angle the point, more necessary if the points arent circles
            trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (Physics2D.gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);
            fTime += pointTimeInterval; // increment time for next point
        }
    }

    // upon successfully completing level
    public void Victory()
    {
        gameOverPanel.SetActive(true); // display end game panel
        
        // pass score out of scene
        MinigameScores.ServingScore = topScore;

        // display score on screen
        gameOverText.text = "" + MinigameScores.ServingScore;

        // test implementation of saving scores between instances of the game, no longer needed
        //SaveLoad.setServingHiScore(MinigameScores.ServingScore);
        //Debug.Log(MinigameScores.ServingScore);

        // call exit scene script
        StartCoroutine(SceneSwitcher.exitScene()); 
    }

    // upon unsuccessfully completing level
    public void Failure()
    {
        gameOverPanel.SetActive(true); // display endgame panel

        // pass score out of scene
        MinigameScores.ServingScore = 0;

        // display score on screen
        gameOverText.text = "" + MinigameScores.ServingScore;

        // test implementation of saving scores between instances of the game, no longer needed
        //SaveLoad.setServingHiScore(MinigameScores.ServingScore);
        //Debug.Log(MinigameScores.ServingScore);

        // call exit scene script
        StartCoroutine(SceneSwitcher.exitScene()); 
    }

    // calculate score based on proximity to center of table, only uses x value, height above table does not matter
    private void Score()
    {
        // get target area
        Collider2D targetArea = targetTable.transform.GetChild(0).GetComponent<Collider2D>();

        // check if food is in target area on top of the table
        if (targetArea.bounds.Contains(food.transform.position))
        {
            // score 100% close to the middle, then less as it gets out to the sides
            float offTarget = Mathf.Abs(targetTable.transform.position.x - food.transform.position.x) * 50;
            int tempScore = Mathf.Clamp(105 - (int)offTarget, 0, 100);

            // convert score from out of 100 to out of target score
            tempScore *= targetScore / 100;
            // if this is the best score so far record it
            if (tempScore > topScore)
            {
                topScore = tempScore;
            }
        }        
    }
}
