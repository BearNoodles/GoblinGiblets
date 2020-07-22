using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script for target area of bar on tapping/fire stoking/cooking minigame
public class TapTarget : MonoBehaviour {
    // width of bar, reset in start function
    private float travelDistance = 8.0f;

    // width of the bar can be changed in editor for difficulty balance
    public float targetBarWidth = 1.0f;

    // movement and colour change variables
    private float travelTime = 0.0f;
    private float elapsedTime = 0.0f;
    private float startTargetX;
    private float elapsedColourTime = 0.0f;

    // balance for travel time of target
    public float minTravelTime = 1;
    public float maxTravelTime = 3;

    public float colourSwapTime = 3.0f; // time before changing colour
    public int colourState = 2; // starting colour of target
    Color orange = new Color(1.0f, 0.65f, 0.0f); // definition of orange (other colours have default values already)

    private Vector3 target;
    private Vector3 startingPos;

    // Use this for initialization
    void Start () {
        travelDistance = 8.0f;
        transform.localScale = new Vector3(targetBarWidth, 1.0f, 1.0f);
        startTargetX = (travelDistance - targetBarWidth) / 2.0f; // target x position is half of travel distance, moving bar is variable width

        // set initial colour based on starting state
        if (colourState == 0)
        {
            GetComponent<SpriteRenderer>().color = Color.red; // change colour
        }
        else if (colourState == 1)
        {
            GetComponent<SpriteRenderer>().color = orange; // change colour
        }
        else if (colourState == 2)
        {
            GetComponent<SpriteRenderer>().color = Color.yellow; // change colour
        }
        else if (colourState == 3)
        {
            GetComponent<SpriteRenderer>().color = Color.green; // change colour
        }
        else if (colourState == 4)
        {
            GetComponent<SpriteRenderer>().color = Color.yellow; // change colour
        }
        else if (colourState == 5)
        {
            GetComponent<SpriteRenderer>().color = orange; // change colour
        }
    }
	
	// Update is called once per frame
	void Update () {
        elapsedTime += Time.fixedDeltaTime;
        elapsedColourTime += Time.fixedDeltaTime;

        // if move time reached
        if (elapsedTime >= travelTime)
        {
            elapsedTime = 0.0f; // reset timer
            travelTime = Random.Range(minTravelTime, maxTravelTime); // set random time to travel
            travelDistance *= -1; // travel opposite direction
            startTargetX *= -1; // target is in opposite direction
            target = new Vector3(startTargetX, transform.position.y, 0.0f); // set new target

            // set start position to side the target is currently on
            startingPos = new Vector3(-startTargetX, transform.position.y, 0.0f);            
        }

        // move
        positionObject(startingPos, target);

        // change colour of target every so often
        if (elapsedColourTime >= colourSwapTime)
        {
            elapsedColourTime = 0.0f; // reset timer

            if (colourState == 0)
            {
                GetComponent<SpriteRenderer>().color = orange; // change colour
                colourState = 1; // track state
            } else if (colourState == 1)
            {
                GetComponent<SpriteRenderer>().color = Color.yellow; // change colour
                colourState = 2; // track state
            } else if (colourState == 2)
            {
                GetComponent<SpriteRenderer>().color = Color.green; // change colour
                colourState = 3; // track state
            } else if (colourState == 3)
            {
                GetComponent<SpriteRenderer>().color = Color.yellow; // change colour
                colourState = 4; // track state
            } else if (colourState == 4)
            {
                GetComponent<SpriteRenderer>().color = orange; // change colour
                colourState = 5; // track state
            } else if (colourState == 5)
            {
                GetComponent<SpriteRenderer>().color = Color.red; // change colour
                colourState = 0; // track state
            }
        }
    }

    // move object thread, no longer used
    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds) // borrowed from peters ticket code
    {
        float elapsedTime = 0;
        // set start position
        Vector3 startingPos = new Vector3(-startTargetX, GetComponent<Transform>().position.y, 0.0f);

        // while time still running
        while (elapsedTime < seconds)
        {
            GetComponent<Transform>().position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds)); // lerp position from start to target
            elapsedTime += Time.deltaTime; // increment time
            yield return new WaitForEndOfFrame(); // only once a frame
        }
        GetComponent<Transform>().position = end; // set end position to a clean point
    }

    // new simpler function for movement
    private void positionObject(Vector3 startingPos, Vector3 target)
    {
        if (elapsedTime < travelTime)
        {
            GetComponent<Transform>().position = Vector3.Lerp(startingPos, target, (elapsedTime / travelTime)); // lerp position from start to target
            elapsedTime += Time.deltaTime / 2;  // increment time, this is going too fast so ive halved delta time?
        }
    }
}
