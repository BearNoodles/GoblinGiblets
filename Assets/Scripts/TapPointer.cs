using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script controlling the pointer in the tapping/fire stoking/cooking minigame
public class TapPointer : MonoBehaviour {

    public float travelTime = 2.0f; // time taken to travel from one end of the bar to the other
    public float travelDistance = 8.0f; // distance the pointer will travel over
    private float elapsedTime = 0.0f;
    
    private float startTargetX;
    private Vector3 target;
    private Vector3 startingPos;

    // Use this for initialization
    void Start () {
                
        // get target position at opposite end of bar
        target = new Vector3(GetComponent<Transform>().position.x + travelDistance, GetComponent<Transform>().position.y, 0.0f);
        startTargetX = travelDistance / 2.0f;
        // swap start side
        startingPos = new Vector3(-startTargetX, transform.position.y, 0.0f);
    }
	
	// Update is called once per frame
	void Update () {
        elapsedTime += Time.fixedDeltaTime;

        // when its moved all the way to the end
        if (elapsedTime >= travelTime)
        {
            elapsedTime = 0.0f; // reset timer
            travelDistance *= -1; // point distance in opposite direction
            startTargetX *= -1; // target is in opposite direction

            // get target position at other end of bar
            target = new Vector3((-travelDistance / 2) + travelDistance, transform.position.y, 0.0f);
            startingPos = new Vector3(-startTargetX, transform.position.y, 0.0f);
        }
        // move
        positionObject(startingPos, target);
    }

    // move pointer to position over time, no longer used, more simple implementation below
    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds) // borrowed from peters ticket code
    {
        float elapsedTime = 0;

        // set start position to clean position
        Vector3 startingPos = new Vector3(-travelDistance / 2.0f, GetComponent<Transform>().position.y, 0.0f);

        // loop until finish time reached
        while (elapsedTime < seconds)
        {
            GetComponent<Transform>().position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds)); // lerp position
            elapsedTime += Time.deltaTime; // increment time
            yield return new WaitForEndOfFrame(); // only do once a frame
        }
        // set position to clean end spot
        GetComponent<Transform>().position = end;
    }

    // no longer used collision check
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("tap_target"))
        {
            //Debug.Log("entered");
            //collision.IsTouching
        }
        //Debug.Log("entered");
    }
  
    // no longer used collision check
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("tap_target"))
        {
            //Debug.Log("exited");
        }
    }
    // simpler movement function
    private void positionObject(Vector3 startingPos, Vector3 target)
    {
        if (elapsedTime < travelTime)
        {
            GetComponent<Transform>().position = Vector3.Lerp(startingPos, target, (elapsedTime / travelTime)); // lerp position from start to target
            elapsedTime += Time.deltaTime / 2; // increment time, this is going too fast so ive halved delta time?
        }
    }
}
