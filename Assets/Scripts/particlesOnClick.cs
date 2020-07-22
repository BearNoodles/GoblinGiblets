using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script used to spawn a prefab at the location of a touch
public class particlesOnClick : MonoBehaviour {

    // prefab to spawn, in this case will be a mess particle effect
    public GameObject particlePrefab;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0) // if there is a touch
        {
            Touch myTouch = Input.touches[0]; // only check 1st touch, ignore multi touches

            if (myTouch.phase == TouchPhase.Began) // when the touch begins
            {
                // instantiate prefab at touch location
                Instantiate(particlePrefab, Camera.main.ScreenToWorldPoint(myTouch.position), Quaternion.identity);
            }
        }
    }    
}
