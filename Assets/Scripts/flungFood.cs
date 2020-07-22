using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script for firing the food around the screen using angry birds style drag controls
// technical test, not currently used, replaced by single shot controls with trajectory points showing target
public class flungFood : MonoBehaviour {

    private Vector2 touchOrigin = -Vector2.one; // initialise touch position to a point off screen

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

        // mouse controls go here

#else

        // touch controls go here

        if (Input.touchCount > 0) // if any number of touches are registered
        {
            Touch myTouch = Input.touches[0]; // work with the 1st touch

            if(myTouch.phase == TouchPhase.Began) // if the touch began this trigger/frame
            {
                touchOrigin = myTouch.position; // set the start position of the touch
            } else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0) // if the touch ended this frame and the position is inside the screen
            {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                Vector2 flingDirection = touchOrigin - touchEnd; // way too big?
                flingDirection.x /= Screen.width;
                flingDirection.y /= Screen.height;
                flingDirection *= 60;
                GetComponent<Rigidbody2D>().velocity = flingDirection;
            }
        }

#endif
        // keep food on screen?
        if (transform.position.x > 10.0f) // 10 is too far on my pixel screen, hopefully right on the s9?  we need to scale this somehow?
        {
            transform.position = new Vector3(10.0f, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -10.0f)
        {
            transform.position = new Vector3(-10.0f, transform.position.y, transform.position.z);
        }
        if (transform.position.y < -4.6f)
        {
            transform.position = new Vector3(transform.position.x, -4.6f, transform.position.z);
        }
        if (transform.position.y > 14.6f)
        {
            transform.position = new Vector3(transform.position.x, 14.6f, transform.position.z);
        }
    }
}
