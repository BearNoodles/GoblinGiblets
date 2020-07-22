using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// proof of concept swipe detection, intended for use on opening and closing the menu in the kitchen
// not implemented in the final build
public class SwipeDetection : MonoBehaviour {

    public float minimumSwipeDistanceInPixels = 100.0f;

    Vector3 touchStartPosition;
    bool drag = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // when touch starts
        if (Input.GetMouseButtonDown(0))
        {
            // record start position
            touchStartPosition = Input.mousePosition;
            // record drag has started
            drag = true;
        }
        // if a touch finishes and the drag had started
        if (Input.GetMouseButtonUp(0) && drag)
        {
            // make vector representing change in touch position between start and finish
            Vector2 dragVector = Input.mousePosition - touchStartPosition; //positive value is up, negative is down
            // if the vertical drag was long enough to count in the up direction
            if (dragVector.y >= minimumSwipeDistanceInPixels)
            {
                // up swipe
                Debug.Log("up swipe");
            }
            // if the vertical drag was long enough to count in the down direction
            else if (dragVector.y <= minimumSwipeDistanceInPixels * -1)
            {
                // down swipe
                Debug.Log("down swipe");
            }
        }
	}
}
