using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script for scrolling the sceneCam at the start of the serving minigame and following the food once it is flung
// starts at the top of the scene, waits for a short amount of time then scrolls down, follows the food back up after firing
// no longer used, built for an older design of the  serving minigame
public class CameraScroll : MonoBehaviour {

    // food object to follow
    public GameObject toFollow;

    private Transform followPosition;
    private bool follow = false;
    public float timeSinceStart = -1.0f; // negative amount of time game will wait before it starts scrolling down

	// Use this for initialization
	void Start () {
        followPosition = toFollow.GetComponent<Transform>();
        transform.position = new Vector3(transform.position.x, 10.0f, transform.position.z); // start position 10 units up from center
    }
	
	// Update is called once per frame
	void Update () {
        // scrolling screen
        if (!follow)
        {
            // scroll down from starting position to center of scene
            timeSinceStart += Time.deltaTime;
            if (timeSinceStart >= 0)
            {
                transform.position = new Vector3(transform.position.x, 10.0f - (timeSinceStart * 5), transform.position.z);
            }
            if (transform.position.y <= 0) // once center is reached stop scrolling and enable following the food
            {
                transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
                follow = true;
            }
        }
        // following food
        if (follow)
        {
            if (followPosition.position.y >= 0.0f) // follow
            {
                transform.position = new Vector3(transform.position.x, followPosition.position.y, transform.position.z);
            }
            if (followPosition.position.y >= 10.0f) // dont go higher than 10 above center
            {
                transform.position = new Vector3(transform.position.x, 10.0f, transform.position.z);
            }
        }
	}
}
